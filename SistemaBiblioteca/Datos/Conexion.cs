using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SistemaBiblioteca.Datos
{
    
    public static class Conexion
    {
        public static string Cadena
        {
            get
            {
                // Cadena detectada al iniciar el sistema (ver ConfiguradorBD).
                if (ConfiguradorBD.CadenaConexion != null)
                    return ConfiguradorBD.CadenaConexion;

                var cs = ConfigurationManager.ConnectionStrings["Biblioteca"];
                if (cs == null)
                    throw new ConfigurationErrorsException("No se encontró la cadena de conexión 'Biblioteca' en App.config.");
                return cs.ConnectionString;
            }
        }

        public static SqlConnection Obtener()
        {
            return new SqlConnection(Cadena);
        }

        public static SqlParameter P(string nombre, object valor)
        {
            return new SqlParameter(nombre, valor ?? DBNull.Value);
        }

        public static DataTable Consultar(string sql, params SqlParameter[] parametros)
        {
            using (var cn = Obtener())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(parametros);
                var tabla = new DataTable();
                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(tabla);
                return tabla;
            }
        }

        public static int Ejecutar(string sql, params SqlParameter[] parametros)
        {
            using (var cn = Obtener())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(parametros);
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object Escalar(string sql, params SqlParameter[] parametros)
        {
            using (var cn = Obtener())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(parametros);
                cn.Open();
                return cmd.ExecuteScalar();
            }
        }


        public static string Texto(DataRow fila, string columna)
        {
            return fila[columna] == DBNull.Value ? null : Convert.ToString(fila[columna]);
        }

        public static int? EnteroNulo(DataRow fila, string columna)
        {
            return fila[columna] == DBNull.Value ? (int?)null : Convert.ToInt32(fila[columna]);
        }

        public static DateTime? FechaNula(DataRow fila, string columna)
        {
            return fila[columna] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(fila[columna]);
        }

        public static object Opcional(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor.Trim();
        }
    }
}
