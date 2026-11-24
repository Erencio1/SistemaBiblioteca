using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SistemaBiblioteca.Formularios;

namespace SistemaBiblioteca.Datos
{
    /// <summary>
    /// Prepara la base de datos automáticamente al iniciar el sistema:
    ///   1) Busca un SQL Server disponible en la PC (el de App.config,
    ///      LocalDB, SQL Express o la instancia por defecto).
    ///   2) Si la base "Biblioteca" no existe, la crea ejecutando el script
    ///      BibliotecaDB.sql, que va incluido dentro del ejecutable.
    ///      (Si en la PC quedaron los archivos Biblioteca.mdf/.ldf de antes,
    ///      los vuelve a adjuntar en lugar de crearla de nuevo.)
    ///   3) Si no encuentra ningún servidor, pregunta cuál usar.
    /// Así el sistema funciona con solo abrir la solución y presionar F5.
    /// </summary>
    public static class ConfiguradorBD
    {
        private const string NombreBD = "Biblioteca";
        private const string RecursoScript = "BibliotecaDB.sql";

        /// <summary>Cadena de conexión que usa todo el sistema (null hasta inicializar).</summary>
        public static string CadenaConexion { get; private set; }

        private static string ArchivoCache
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SistemaBiblioteca", "conexion.txt");
            }
        }

        /// <summary>Devuelve true si la base quedó lista para usarse.</summary>
        public static bool Inicializar()
        {
            Form aviso = CrearAviso("Conectando con SQL Server...\nLa primera vez puede tardar unos segundos.");
            try
            {
                aviso.Show();
                Application.DoEvents();
                string cadena = BuscarServidor();
                aviso.Hide();

                // No se encontró ningún servidor: se le pregunta al usuario.
                while (cadena == null)
                {
                    using (FrmServidor frm = new FrmServidor())
                    {
                        if (frm.ShowDialog() != DialogResult.OK)
                            return false;

                        aviso.Show();
                        Application.DoEvents();
                        cadena = ProbarServidor(frm.CadenaMaster);
                        aviso.Hide();

                        if (cadena == null)
                            Ui.Advertencia("No se pudo conectar a ese servidor. Verifique el nombre y los datos de acceso.");
                    }
                }

                if (!TablasExisten(cadena))
                {
                    CambiarTexto(aviso, "Creando la base de datos \"Biblioteca\"...");
                    aviso.Show();
                    Application.DoEvents();

                    bool adjuntada = IntentarAdjuntar(cadena) && TablasExisten(cadena);
                    if (!adjuntada)
                        EjecutarScript(cadena);

                    aviso.Hide();
                    if (!adjuntada)
                        Ui.Info("Se creó la base de datos \"Biblioteca\" con datos de ejemplo.\n\n" +
                                "Usuario: admin\nContraseña: admin123");
                }

                SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(cadena);
                csb.InitialCatalog = NombreBD;
                csb.ConnectTimeout = 15;
                CadenaConexion = csb.ConnectionString;
                GuardarCache(cadena);
                return true;
            }
            catch (Exception ex)
            {
                aviso.Hide();
                MessageBox.Show("No se pudo preparar la base de datos:\n\n" + ex.Message +
                                "\n\nComo alternativa, ejecute manualmente el script BaseDatos\\BibliotecaDB.sql " +
                                "(ver README.md).",
                                "Sistema de Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                aviso.Close();
                aviso.Dispose();
            }
        }

        // ------------------------------------------------------------------
        // Búsqueda del servidor
        // ------------------------------------------------------------------

        private static string BuscarServidor()
        {
            List<string> candidatas = new List<string>();

            // 1) La última conexión que funcionó en esta PC.
            string cache = LeerCache();
            if (cache != null) candidatas.Add(cache);

            // 2) La de App.config.
            ConnectionStringSettings cfg = ConfigurationManager.ConnectionStrings["Biblioteca"];
            if (cfg != null && !string.IsNullOrWhiteSpace(cfg.ConnectionString))
                candidatas.Add(cfg.ConnectionString);

            // 3) Los servidores más comunes, con autenticación de Windows.
            string[] comunes = { @"(localdb)\MSSQLLocalDB", @".\SQLEXPRESS", "localhost" };
            foreach (string servidor in comunes)
            {
                SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
                b.DataSource = servidor;
                b.IntegratedSecurity = true;
                candidatas.Add(b.ConnectionString);
            }

            HashSet<string> probadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string c in candidatas)
            {
                string clave;
                try { clave = new SqlConnectionStringBuilder(c).DataSource; }
                catch { continue; }
                if (!probadas.Add(clave)) continue;   // no probar dos veces el mismo servidor

                string ok = ProbarServidor(c);
                if (ok != null) return ok;
            }
            return null;
        }

        /// <summary>
        /// Intenta conectarse a "master" del servidor indicado.
        /// Devuelve la cadena (apuntando a master) si funciona; si no, null.
        /// </summary>
        private static string ProbarServidor(string cadena)
        {
            try
            {
                SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(cadena);
                b.InitialCatalog = "master";
                // LocalDB tarda más en arrancar la primera vez.
                b.ConnectTimeout = b.DataSource.ToLower().Contains("localdb") ? 30 : 5;

                using (SqlConnection cn = new SqlConnection(b.ConnectionString))
                    cn.Open();

                return b.ConnectionString;
            }
            catch
            {
                return null;
            }
        }

        // ------------------------------------------------------------------
        // Creación de la base
        // ------------------------------------------------------------------

        private static bool TablasExisten(string cadenaMaster)
        {
            using (SqlConnection cn = new SqlConnection(cadenaMaster))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT CASE WHEN DB_ID(@bd) IS NOT NULL AND OBJECT_ID(@bd + '.dbo.Usuario') IS NOT NULL " +
                "THEN 1 ELSE 0 END", cn))
            {
                cmd.Parameters.AddWithValue("@bd", NombreBD);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }

        /// <summary>
        /// Si la base no está registrada pero sus archivos .mdf/.ldf siguen en la
        /// carpeta de datos del servidor (pasa con LocalDB), los vuelve a adjuntar.
        /// </summary>
        private static bool IntentarAdjuntar(string cadenaMaster)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(cadenaMaster))
                {
                    cn.Open();

                    using (SqlCommand existe = new SqlCommand("SELECT DB_ID(@bd)", cn))
                    {
                        existe.Parameters.AddWithValue("@bd", NombreBD);
                        if (existe.ExecuteScalar() != DBNull.Value) return false; // ya está registrada
                    }

                    string carpeta;
                    using (SqlCommand ruta = new SqlCommand(
                        "SELECT CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS NVARCHAR(4000))", cn))
                    {
                        object r = ruta.ExecuteScalar();
                        carpeta = r == null || r == DBNull.Value ? null : r.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(carpeta)) return false;

                    string mdf = Path.Combine(carpeta, NombreBD + ".mdf");
                    string ldf = Path.Combine(carpeta, NombreBD + "_log.ldf");
                    if (!File.Exists(mdf)) return false;

                    string sql = File.Exists(ldf)
                        ? "CREATE DATABASE [" + NombreBD + "] ON (FILENAME = N'" + mdf.Replace("'", "''") + "'), " +
                          "(FILENAME = N'" + ldf.Replace("'", "''") + "') FOR ATTACH;"
                        : "CREATE DATABASE [" + NombreBD + "] ON (FILENAME = N'" + mdf.Replace("'", "''") + "') " +
                          "FOR ATTACH_REBUILD_LOG;";

                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.CommandTimeout = 120;
                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void EjecutarScript(string cadenaMaster)
        {
            string script = LeerScript();

            // "GO" no es SQL: lo entiende SSMS, no el servidor. Se separa el script en lotes.
            string[] lotes = Regex.Split(script, @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using (SqlConnection cn = new SqlConnection(cadenaMaster))
            {
                cn.Open();
                foreach (string lote in lotes)
                {
                    if (string.IsNullOrWhiteSpace(lote)) continue;
                    using (SqlCommand cmd = new SqlCommand(lote, cn))
                    {
                        cmd.CommandTimeout = 120;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static string LeerScript()
        {
            // El script va incrustado en el .exe (ver SistemaBiblioteca.csproj).
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream s = asm.GetManifestResourceStream(RecursoScript))
            {
                if (s != null)
                    using (StreamReader r = new StreamReader(s, Encoding.UTF8, true))
                        return r.ReadToEnd();
            }

            // Alternativa: buscar BaseDatos\BibliotecaDB.sql en las carpetas superiores.
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null)
            {
                string ruta = Path.Combine(dir.FullName, "BaseDatos", RecursoScript);
                if (File.Exists(ruta)) return File.ReadAllText(ruta, Encoding.UTF8);
                dir = dir.Parent;
            }
            throw new FileNotFoundException("No se encontró el script " + RecursoScript);
        }

        // ------------------------------------------------------------------
        // Utilidades
        // ------------------------------------------------------------------

        private static string LeerCache()
        {
            try
            {
                if (File.Exists(ArchivoCache))
                {
                    string c = File.ReadAllText(ArchivoCache).Trim();
                    if (c.Length > 0) return c;
                }
            }
            catch { }
            return null;
        }

        private static void GuardarCache(string cadena)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArchivoCache));
                File.WriteAllText(ArchivoCache, cadena);
            }
            catch { }
        }

        private static Form CrearAviso(string texto)
        {
            Form f = new Form
            {
                Text = "Sistema de Biblioteca",
                Size = new Size(420, 140),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ControlBox = false,
                BackColor = Color.White
            };
            f.Controls.Add(new Label
            {
                Name = "lblTexto",
                Text = texto,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10)
            });
            return f;
        }

        private static void CambiarTexto(Form aviso, string texto)
        {
            Control[] c = aviso.Controls.Find("lblTexto", false);
            if (c.Length > 0) c[0].Text = texto;
        }
    }
}
