using System;
using System.Linq;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>Menú principal (contenedor MDI).</summary>
    public partial class FrmPrincipal : Form
    {
        /// <summary>true si el usuario eligió "Cerrar sesión" (vuelve al login).</summary>
        public bool CerrarSesion { get; private set; }

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            var u = Sesion.UsuarioActual;
            lblUsuario.Text = "Usuario: " + u.NombreCompleto + " (" + u.NombreUsuario + ")  |  Rol: " + u.NombreRol;

            // Solo el Administrador gestiona usuarios del sistema.
            mnuUsuarios.Visible = u.EsAdministrador;
            mnuSeparador1.Visible = u.EsAdministrador;

            // Mantiene al día el estado "Atrasado" de los préstamos vencidos.
            try { new PrestamoDatos().ActualizarAtrasados(); }
            catch (Exception ex) { Ui.Error(ex); }
        }

        /// <summary>Abre un formulario hijo; si ya está abierto lo trae al frente.</summary>
        private void Abrir<T>() where T : Form, new()
        {
            var abierto = MdiChildren.OfType<T>().FirstOrDefault();
            if (abierto != null)
            {
                abierto.WindowState = FormWindowState.Normal;
                abierto.Activate();
                return;
            }
            var frm = new T { MdiParent = this };
            frm.Show();
        }

        private void mnuCategorias_Click(object sender, EventArgs e) { Abrir<FrmCategorias>(); }
        private void mnuEditoriales_Click(object sender, EventArgs e) { Abrir<FrmEditoriales>(); }
        private void mnuAutores_Click(object sender, EventArgs e) { Abrir<FrmAutores>(); }
        private void mnuLibros_Click(object sender, EventArgs e) { Abrir<FrmLibros>(); }
        private void mnuEjemplares_Click(object sender, EventArgs e) { Abrir<FrmEjemplares>(); }
        private void mnuSocios_Click(object sender, EventArgs e) { Abrir<FrmSocios>(); }
        private void mnuUsuarios_Click(object sender, EventArgs e) { Abrir<FrmUsuarios>(); }
        private void mnuPrestamo_Click(object sender, EventArgs e) { Abrir<FrmPrestamo>(); }
        private void mnuDevolucion_Click(object sender, EventArgs e) { Abrir<FrmDevolucion>(); }

        private void mnuCerrarSesion_Click(object sender, EventArgs e)
        {
            if (!Ui.Confirmar("¿Desea cerrar la sesión?"))
                return;
            CerrarSesion = true;
            Close();
        }

        private void mnuSalir_Click(object sender, EventArgs e)
        {
            if (Ui.Confirmar("¿Desea salir del sistema?"))
                Close();
        }
    }
}
