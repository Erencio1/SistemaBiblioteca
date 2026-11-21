using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;
using SistemaBiblioteca.Modelos;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>ABM de Socios.</summary>
    public partial class FrmSocios : Form
    {
        private readonly SocioDatos datos = new SocioDatos();
        private List<Socio> lista = new List<Socio>();
        private int idActual;

        public FrmSocios()
        {
            InitializeComponent();
            Ui.PrepararGrilla(dgvDatos);
        }

        private void FrmSocios_Load(object sender, EventArgs e) { Cargar(); }

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
            dgvDatos.DataSource = lista.Where(x => b == "" || x.NombreCompleto.ToLower().Contains(b)).ToList();
            Ui.OcultarColumnas(dgvDatos, "IdSocio", "NombreCompleto");
            Ui.Encabezados(dgvDatos, "Telefono", "Teléfono", "Direccion", "Dirección", "FechaRegistro", "Fecha de registro");
        }

        private void Limpiar()
        {
            idActual = 0;
            txtNombres.Clear();
            txtApellidos.Clear();
            txtDocumento.Clear();
            txtTelefono.Clear();
            txtEmail.Clear();
            txtDireccion.Clear();
            chkActivo.Checked = true;
            dgvDatos.ClearSelection();
            txtNombres.Focus();
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var s = (Socio)dgvDatos.Rows[e.RowIndex].DataBoundItem;
            idActual = s.IdSocio;
            txtNombres.Text = s.Nombres;
            txtApellidos.Text = s.Apellidos;
            txtDocumento.Text = s.Documento;
            txtTelefono.Text = s.Telefono;
            txtEmail.Text = s.Email;
            txtDireccion.Text = s.Direccion;
            chkActivo.Checked = s.Activo;
        }

        private void btnNuevo_Click(object sender, EventArgs e) { Limpiar(); }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requeridos((txtNombres, "Nombres"), (txtApellidos, "Apellidos"), (txtDocumento, "Documento"))) return;

            var s = new Socio
            {
                IdSocio = idActual,
                Nombres = txtNombres.Text,
                Apellidos = txtApellidos.Text,
                Documento = txtDocumento.Text,
                Telefono = txtTelefono.Text,
                Email = txtEmail.Text,
                Direccion = txtDireccion.Text,
                Activo = chkActivo.Checked
            };
            try
            {
                if (idActual == 0) datos.Insertar(s); else datos.Actualizar(s);
                Ui.Info("Socio guardado correctamente.");
                Cargar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idActual == 0) { Ui.Advertencia("Seleccione un socio de la lista."); return; }
            if (!Ui.Confirmar("¿Eliminar el socio \"" + txtNombres.Text + " " + txtApellidos.Text + "\"?\n" +
                              "Si ya tiene préstamos, desmarque \"Activo\" en lugar de eliminarlo.")) return;
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
