using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;
using SistemaBiblioteca.Modelos;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>ABM de Editoriales.</summary>
    public partial class FrmEditoriales : Form
    {
        private readonly EditorialDatos datos = new EditorialDatos();
        private List<Editorial> lista = new List<Editorial>();
        private int idActual;

        public FrmEditoriales()
        {
            InitializeComponent();
            Ui.PrepararGrilla(dgvDatos);
        }

        private void FrmEditoriales_Load(object sender, EventArgs e) { Cargar(); }

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
            dgvDatos.DataSource = lista.Where(x => b == "" || (x.Nombre ?? "").ToLower().Contains(b)).ToList();
            Ui.OcultarColumnas(dgvDatos, "IdEditorial");
            Ui.Encabezados(dgvDatos, "Telefono", "Teléfono", "Direccion", "Dirección");
        }

        private void Limpiar()
        {
            idActual = 0;
            txtNombre.Clear();
            txtTelefono.Clear();
            txtEmail.Clear();
            txtDireccion.Clear();
            dgvDatos.ClearSelection();
            txtNombre.Focus();
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var x = (Editorial)dgvDatos.Rows[e.RowIndex].DataBoundItem;
            idActual = x.IdEditorial;
            txtNombre.Text = x.Nombre;
            txtTelefono.Text = x.Telefono;
            txtEmail.Text = x.Email;
            txtDireccion.Text = x.Direccion;
        }

        private void btnNuevo_Click(object sender, EventArgs e) { Limpiar(); }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requeridos((txtNombre, "Nombre"))) return;

            var x = new Editorial
            {
                IdEditorial = idActual,
                Nombre = txtNombre.Text,
                Telefono = txtTelefono.Text,
                Email = txtEmail.Text,
                Direccion = txtDireccion.Text
            };
            try
            {
                if (idActual == 0) datos.Insertar(x); else datos.Actualizar(x);
                Ui.Info("Editorial guardada correctamente.");
                Cargar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idActual == 0) { Ui.Advertencia("Seleccione una editorial de la lista."); return; }
            if (!Ui.Confirmar("¿Eliminar la editorial \"" + txtNombre.Text + "\"?")) return;
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
