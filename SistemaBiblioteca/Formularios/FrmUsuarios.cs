using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;
using SistemaBiblioteca.Modelos;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>ABM de Usuarios del sistema (solo Administrador).</summary>
    public partial class FrmUsuarios : Form
    {
        private readonly UsuarioDatos datos = new UsuarioDatos();
        private List<Usuario> lista = new List<Usuario>();
        private int idActual;

        public FrmUsuarios()
        {
            InitializeComponent();
            Ui.PrepararGrilla(dgvDatos);
        }

        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
            try
            {
                cboRol.DataSource = new RolDatos().Listar();
                cboRol.DisplayMember = "Nombre";
                cboRol.ValueMember = "IdRol";
            }
            catch (Exception ex) { Ui.Error(ex); }
            Cargar();
        }

        private void Cargar()
        {
            try { lista = datos.Listar(); }
            catch (Exception ex) { Ui.Error(ex); }
            Filtrar();
            Limpiar();
        }

        private void Filtrar()
        {
            string b = txtBuscar.Text.Trim().ToLower();
            dgvDatos.DataSource = lista.Where(x => b == ""
                || (x.NombreUsuario ?? "").ToLower().Contains(b)
                || (x.NombreCompleto ?? "").ToLower().Contains(b)).ToList();
            // La clave (hash) nunca se muestra.
            Ui.OcultarColumnas(dgvDatos, "IdUsuario", "Clave", "IdRol", "EsAdministrador");
            Ui.Encabezados(dgvDatos, "NombreUsuario", "Usuario", "NombreCompleto", "Nombre completo",
                "NombreRol", "Rol", "FechaCreacion", "Fecha de creación");
        }

        private void Limpiar()
        {
            idActual = 0;
            txtUsuario.Clear();
            txtNombreCompleto.Clear();
            txtClave.Clear();
            if (cboRol.Items.Count > 0) cboRol.SelectedIndex = 0;
            chkActivo.Checked = true;
            dgvDatos.ClearSelection();
            txtUsuario.Focus();
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var u = (Usuario)dgvDatos.Rows[e.RowIndex].DataBoundItem;
            idActual = u.IdUsuario;
            txtUsuario.Text = u.NombreUsuario;
            txtNombreCompleto.Text = u.NombreCompleto;
            txtClave.Clear();
            cboRol.SelectedValue = u.IdRol;
            chkActivo.Checked = u.Activo;
        }

        private void btnNuevo_Click(object sender, EventArgs e) { Limpiar(); }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requeridos((txtUsuario, "Usuario"), (txtNombreCompleto, "Nombre completo"))) return;
            if (idActual == 0 && !Ui.Requeridos((txtClave, "Contraseña"))) return;
            if (txtClave.Text.Length > 0 && txtClave.Text.Length < 6)
            {
                Ui.Advertencia("La contraseña debe tener al menos 6 caracteres.");
                txtClave.Focus();
                return;
            }
            if (cboRol.SelectedValue == null) { Ui.Advertencia("Seleccione un rol."); return; }

            bool esUsuarioActual = idActual == Sesion.UsuarioActual.IdUsuario;
            if (esUsuarioActual && !chkActivo.Checked)
            {
                Ui.Advertencia("No puede desactivar su propio usuario.");
                return;
            }

            var u = new Usuario
            {
                IdUsuario = idActual,
                NombreUsuario = txtUsuario.Text,
                NombreCompleto = txtNombreCompleto.Text,
                Clave = txtClave.Text,           // se cifra en la capa de datos
                IdRol = (int)cboRol.SelectedValue,
                Activo = chkActivo.Checked
            };
            try
            {
                if (idActual == 0) datos.Insertar(u); else datos.Actualizar(u);
                Ui.Info("Usuario guardado correctamente.");
                Cargar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idActual == 0) { Ui.Advertencia("Seleccione un usuario de la lista."); return; }
            if (idActual == Sesion.UsuarioActual.IdUsuario) { Ui.Advertencia("No puede eliminar el usuario con el que inició sesión."); return; }
            if (!Ui.Confirmar("¿Eliminar el usuario \"" + txtUsuario.Text + "\"?\n" +
                              "Si ya registró préstamos, desmarque \"Activo\" en lugar de eliminarlo.")) return;
            try
            {
                datos.Eliminar(idActual);
                Cargar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnCerrar_Click(object sender, EventArgs e) { Close(); }

        private void txtBuscar_TextChanged(object sender, EventArgs e) { Filtrar(); }
    }
}
