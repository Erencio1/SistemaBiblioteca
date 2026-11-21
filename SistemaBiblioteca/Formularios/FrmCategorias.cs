using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;
using SistemaBiblioteca.Modelos;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>ABM de Categorías.</summary>
    public partial class FrmCategorias : Form
    {
        private readonly CategoriaDatos datos = new CategoriaDatos();
        private List<Categoria> lista = new List<Categoria>();
        private int idActual; // 0 = registro nuevo

        public FrmCategorias()
        {
            InitializeComponent();
            Ui.PrepararGrilla(dgvDatos);
        }

        private void FrmCategorias_Load(object sender, EventArgs e) { Cargar(); }

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
            Ui.OcultarColumnas(dgvDatos, "IdCategoria");
            Ui.Encabezados(dgvDatos, "Descripcion", "Descripción");
        }

        private void Limpiar()
        {
            idActual = 0;
            txtNombre.Clear();
            txtDescripcion.Clear();
            dgvDatos.ClearSelection();
            txtNombre.Focus();
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var c = (Categoria)dgvDatos.Rows[e.RowIndex].DataBoundItem;
            idActual = c.IdCategoria;
            txtNombre.Text = c.Nombre;
            txtDescripcion.Text = c.Descripcion;
        }

        private void btnNuevo_Click(object sender, EventArgs e) { Limpiar(); }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requeridos((txtNombre, "Nombre"))) return;

            var c = new Categoria { IdCategoria = idActual, Nombre = txtNombre.Text, Descripcion = txtDescripcion.Text };
            try
            {
                if (idActual == 0) datos.Insertar(c); else datos.Actualizar(c);
                Ui.Info("Categoría guardada correctamente.");
                Cargar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idActual == 0) { Ui.Advertencia("Seleccione una categoría de la lista."); return; }
            if (!Ui.Confirmar("¿Eliminar la categoría \"" + txtNombre.Text + "\"?")) return;
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
