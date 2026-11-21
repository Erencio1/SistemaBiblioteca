using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SistemaBiblioteca.Formularios
{
    public static class Ui
    {
        private const string Titulo = "Sistema de Biblioteca";

        public static void Info(string mensaje)
        {
            MessageBox.Show(mensaje, Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Advertencia(string mensaje)
        {
            MessageBox.Show(mensaje, Titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static bool Confirmar(string mensaje)
        {
            return MessageBox.Show(mensaje, Titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public static void Error(Exception ex)
        {
            MessageBox.Show(TraducirError(ex), Titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string TraducirError(Exception ex)
        {
            if (ex is SqlException sql)
            {
                switch (sql.Number)
                {
                    case 547:   
                        return "No se puede completar la operación porque el registro está relacionado con otros datos " +
                               "(por ejemplo, tiene libros, ejemplares o préstamos asociados).";
                    case 2627:  
                    case 2601:  
                        return "Ya existe un registro con ese valor (dato duplicado). Verifique nombre, documento, código o ISBN.";
                    case -1:
                    case 2:
                    case 53:
                    case -2:
                    case 4060:  
                    case 18456: 
                        return "No se pudo conectar con la base de datos.\n\n" +
                               "Verifique que SQL Server esté instalado y en ejecución, que se haya ejecutado el script " +
                               "BaseDatos\\BibliotecaDB.sql y que la cadena de conexión de App.config sea correcta.\n\n" +
                               "Detalle: " + sql.Message;
                }
                return "Error de base de datos: " + sql.Message;
            }
            return ex.Message;
        }

        public static bool Requeridos(params (TextBox caja, string nombre)[] campos)
        {
            foreach (var c in campos)
            {
                if (string.IsNullOrWhiteSpace(c.caja.Text))
                {
                    Advertencia("El campo \"" + c.nombre + "\" es obligatorio.");
                    c.caja.Focus();
                    return false;
                }
            }
            return true;
        }

        public static void PrepararGrilla(DataGridView dgv)
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = System.Drawing.SystemColors.Window;
        }

        public static void OcultarColumnas(DataGridView dgv, params string[] columnas)
        {
            foreach (var nombre in columnas)
                if (dgv.Columns.Contains(nombre))
                    dgv.Columns[nombre].Visible = false;
        }

        public static void Encabezados(DataGridView dgv, params string[] pares)
        {
            for (int i = 0; i + 1 < pares.Length; i += 2)
                if (dgv.Columns.Contains(pares[i]))
                    dgv.Columns[pares[i]].HeaderText = pares[i + 1];
        }
    }
}
