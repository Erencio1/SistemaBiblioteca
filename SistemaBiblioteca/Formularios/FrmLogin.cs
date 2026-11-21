using System;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>CU-01: Iniciar sesión.</summary>
    public partial class FrmLogin : Form
    {
        private readonly UsuarioDatos usuarios = new UsuarioDatos();

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requeridos((txtUsuario, "Usuario"), (txtClave, "Contraseña")))
                return;

            try
            {
                Cursor = Cursors.WaitCursor;
                var usuario = usuarios.Login(txtUsuario.Text, txtClave.Text);
                if (usuario == null)
                {
                    Ui.Advertencia("Usuario o contraseña incorrectos.");
                    txtClave.Clear();
                    txtClave.Focus();
                    return;
                }

                Sesion.UsuarioActual = usuario;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
