using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;
using SistemaBiblioteca.Modelos;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>ABM de Libros (con categoría, editorial y autores).</summary>
    public partial class FrmLibros : Form
    {
        private readonly LibroDatos datos = new LibroDatos();
        private List<Libro> lista = new List<Libro>();
        private int idActual;

        public FrmLibros()
        {
            InitializeComponent();
            Ui.PrepararGrilla(dgvDatos);
        }

        private void FrmLibros_Load(object sender, EventArgs e)
        {
            try
            {
                cboCategoria.DataSource = new CategoriaDatos().Listar();
                cboCategoria.DisplayMember = "Nombre";
                cboCategoria.ValueMember = "IdCategoria";

                cboEditorial.DataSource = new EditorialDatos().Listar();
                cboEditorial.DisplayMember = "Nombre";
                cboEditorial.ValueMember = "IdEditorial";

                clbAutores.DataSource = new AutorDatos().Listar();
                clbAutores.DisplayMember = "NombreCompleto";
                clbAutores.ValueMember = "IdAutor";
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
                || (x.Titulo ?? "").ToLower().Contains(b)
                || (x.Autores ?? "").ToLower().Contains(b)
                || (x.ISBN ?? "").ToLower().Contains(b)).ToList();
            Ui.OcultarColumnas(dgvDatos, "IdLibro", "IdCategoria", "IdEditorial", "IdsAutores");
            Ui.Encabezados(dgvDatos, "Titulo", "Título", "Anio", "Año",
                "NombreCategoria", "Categoría", "NombreEditorial", "Editorial");
        }

        private void Limpiar()
        {
            idActual = 0;
            txtTitulo.Clear();
            txtISBN.Clear();
            txtAnio.Clear();
            if (cboCategoria.Items.Count > 0) cboCategoria.SelectedIndex = 0;
            if (cboEditorial.Items.Count > 0) cboEditorial.SelectedIndex = 0;
            for (int i = 0; i < clbAutores.Items.Count; i++) clbAutores.SetItemChecked(i, false);
            dgvDatos.ClearSelection();
            txtTitulo.Focus();
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var l = (Libro)dgvDatos.Rows[e.RowIndex].DataBoundItem;
            idActual = l.IdLibro;
            txtTitulo.Text = l.Titulo;
            txtISBN.Text = l.ISBN;
            txtAnio.Text = l.Anio.HasValue ? l.Anio.Value.ToString() : "";
            cboCategoria.SelectedValue = l.IdCategoria;
            cboEditorial.SelectedValue = l.IdEditorial;

            try
            {
                var ids = datos.ObtenerAutores(l.IdLibro);
                for (int i = 0; i < clbAutores.Items.Count; i++)
                    clbAutores.SetItemChecked(i, ids.Contains(((Autor)clbAutores.Items[i]).IdAutor));
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnNuevo_Click(object sender, EventArgs e) { Limpiar(); }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requeridos((txtTitulo, "Título"))) return;
            if (cboCategoria.SelectedValue == null || cboEditorial.SelectedValue == null)
            {
                Ui.Advertencia("Debe cargar al menos una categoría y una editorial antes de registrar libros.");
                return;
            }

            int? anio = null;
            if (txtAnio.Text.Trim() != "")
            {
                if (!int.TryParse(txtAnio.Text.Trim(), out int a) || a < 0 || a > DateTime.Today.Year)
                {
                    Ui.Advertencia("El año debe ser un número válido (no mayor al año actual).");
                    txtAnio.Focus();
                    return;
                }
                anio = a;
            }

            var libro = new Libro
            {
                IdLibro = idActual,
                Titulo = txtTitulo.Text,
                ISBN = txtISBN.Text,
                Anio = anio,
                IdCategoria = (int)cboCategoria.SelectedValue,
                IdEditorial = (int)cboEditorial.SelectedValue,
                IdsAutores = clbAutores.CheckedItems.Cast<Autor>().Select(x => x.IdAutor).ToList()
            };

            try
            {
                if (idActual == 0) datos.Insertar(libro); else datos.Actualizar(libro);
                Ui.Info("Libro guardado correctamente.");
                Cargar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idActual == 0) { Ui.Advertencia("Seleccione un libro de la lista."); return; }
            if (!Ui.Confirmar("¿Eliminar el libro \"" + txtTitulo.Text + "\"?")) return;
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
