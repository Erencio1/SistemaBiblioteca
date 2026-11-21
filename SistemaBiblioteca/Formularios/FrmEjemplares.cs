using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;
using SistemaBiblioteca.Modelos;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>ABM de Ejemplares (copias físicas de cada libro).</summary>
    public partial class FrmEjemplares : Form
    {
        private readonly EjemplarDatos datos = new EjemplarDatos();
        private List<Ejemplar> lista = new List<Ejemplar>();
        private int idActual;
        private string estadoActual; // estado guardado del ejemplar seleccionado

        public FrmEjemplares()
        {
            InitializeComponent();
            Ui.PrepararGrilla(dgvDatos);
        }

        private void FrmEjemplares_Load(object sender, EventArgs e)
        {
            try
            {
                cboLibro.DataSource = new LibroDatos().Listar();
                cboLibro.DisplayMember = "Titulo";
                cboLibro.ValueMember = "IdLibro";
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
                || (x.TituloLibro ?? "").ToLower().Contains(b)
                || (x.CodigoBarras ?? "").ToLower().Contains(b)).ToList();
            Ui.OcultarColumnas(dgvDatos, "IdEjemplar", "IdLibro");
            Ui.Encabezados(dgvDatos, "TituloLibro", "Libro", "CodigoBarras", "Código", "Ubicacion", "Ubicación");
        }

        /// <summary>
        /// El estado "Prestado" lo asigna solo el sistema al registrar un préstamo,
        /// por eso el usuario elige únicamente entre Disponible y Baja.
        /// </summary>
        private void CargarEstados(bool prestado)
        {
            cboEstado.Items.Clear();
            if (prestado)
                cboEstado.Items.Add(Ejemplar.Prestado);
            else
                cboEstado.Items.AddRange(new object[] { Ejemplar.Disponible, Ejemplar.Baja });
            cboEstado.SelectedIndex = 0;
            cboEstado.Enabled = !prestado;
        }

        private void Limpiar()
        {
            idActual = 0;
            estadoActual = null;
            if (cboLibro.Items.Count > 0) cboLibro.SelectedIndex = 0;
            txtCodigo.Clear();
            txtUbicacion.Clear();
            CargarEstados(false);
            dgvDatos.ClearSelection();
            txtCodigo.Focus();
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var x = (Ejemplar)dgvDatos.Rows[e.RowIndex].DataBoundItem;
            idActual = x.IdEjemplar;
            estadoActual = x.Estado;
            cboLibro.SelectedValue = x.IdLibro;
            txtCodigo.Text = x.CodigoBarras;
            txtUbicacion.Text = x.Ubicacion;
            CargarEstados(x.Estado == Ejemplar.Prestado);
            cboEstado.SelectedItem = x.Estado;
        }

        private void btnNuevo_Click(object sender, EventArgs e) { Limpiar(); }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cboLibro.SelectedValue == null) { Ui.Advertencia("Primero debe registrar libros."); return; }
            if (!Ui.Requeridos((txtCodigo, "Código de barras"))) return;

            var x = new Ejemplar
            {
                IdEjemplar = idActual,
                IdLibro = (int)cboLibro.SelectedValue,
                CodigoBarras = txtCodigo.Text,
                Ubicacion = txtUbicacion.Text,
                Estado = (string)cboEstado.SelectedItem
            };
            try
            {
                if (idActual == 0) datos.Insertar(x); else datos.Actualizar(x);
                Ui.Info("Ejemplar guardado correctamente.");
                Cargar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idActual == 0) { Ui.Advertencia("Seleccione un ejemplar de la lista."); return; }
            if (estadoActual == Ejemplar.Prestado) { Ui.Advertencia("No se puede eliminar un ejemplar que está prestado."); return; }
            if (!Ui.Confirmar("¿Eliminar el ejemplar \"" + txtCodigo.Text + "\"?\n" +
                              "Si ya tuvo préstamos, márquelo como \"Baja\" en lugar de eliminarlo.")) return;
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
