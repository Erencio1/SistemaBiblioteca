using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>
    /// Se muestra solo si el sistema no encuentra automáticamente un SQL Server.
    /// Permite escribir el nombre del servidor y, opcionalmente, usuario y contraseña.
    /// </summary>
    public class FrmServidor : Form
    {
        private const string UrlDescarga = "https://www.microsoft.com/sql-server/sql-server-downloads";

        private readonly TextBox txtServidor;
        private readonly TextBox txtUsuario;
        private readonly TextBox txtClave;

        /// <summary>Cadena de conexión (a master) armada con los datos ingresados.</summary>
        public string CadenaMaster { get; private set; }

        public FrmServidor()
        {
            Text = "Conexión a SQL Server";
            ClientSize = new Size(430, 330);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9);

            Label info = new Label
            {
                Text = "No se encontró un SQL Server en esta PC.\n" +
                       "Si tiene uno, escriba su nombre (ej: .\\SQLEXPRESS, localhost, (localdb)\\MSSQLLocalDB).",
                Location = new Point(20, 15),
                Size = new Size(390, 45)
            };

            Label lblServidor = new Label { Text = "Servidor:", Location = new Point(20, 68), AutoSize = true };
            txtServidor = new TextBox { Location = new Point(20, 88), Size = new Size(390, 25), Text = @".\SQLEXPRESS" };

            Label lblUsuario = new Label
            {
                Text = "Usuario SQL (vacío = autenticación de Windows):",
                Location = new Point(20, 122),
                AutoSize = true
            };
            txtUsuario = new TextBox { Location = new Point(20, 142), Size = new Size(390, 25) };

            Label lblClave = new Label { Text = "Contraseña:", Location = new Point(20, 176), AutoSize = true };
            txtClave = new TextBox { Location = new Point(20, 196), Size = new Size(390, 25), UseSystemPasswordChar = true };

            LinkLabel lnkDescargar = new LinkLabel
            {
                Text = "¿No tiene SQL Server? Descargue SQL Server Express (gratis) e instálelo con la opción \"Básica\".",
                Location = new Point(20, 232),
                Size = new Size(390, 36)
            };
            lnkDescargar.LinkClicked += (s, e) =>
            {
                try { Process.Start(UrlDescarga); }
                catch { Clipboard.SetText(UrlDescarga); Ui.Info("Enlace copiado al portapapeles:\n" + UrlDescarga); }
            };

            Button btnConectar = new Button { Text = "Conectar", Location = new Point(220, 282), Size = new Size(90, 30) };
            btnConectar.Click += BtnConectar_Click;

            Button btnSalir = new Button
            {
                Text = "Salir",
                Location = new Point(320, 282),
                Size = new Size(90, 30),
                DialogResult = DialogResult.Cancel
            };

            Controls.AddRange(new Control[]
            {
                info, lblServidor, txtServidor, lblUsuario, txtUsuario,
                lblClave, txtClave, lnkDescargar, btnConectar, btnSalir
            });
            AcceptButton = btnConectar;
            CancelButton = btnSalir;
        }

        private void BtnConectar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServidor.Text))
            {
                Ui.Advertencia("Ingrese el nombre del servidor.");
                return;
            }

            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
            b.DataSource = txtServidor.Text.Trim();
            b.InitialCatalog = "master";
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                b.IntegratedSecurity = true;
            }
            else
            {
                b.UserID = txtUsuario.Text.Trim();
                b.Password = txtClave.Text;
            }

            CadenaMaster = b.ConnectionString;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
