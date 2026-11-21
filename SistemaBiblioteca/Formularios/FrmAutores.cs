using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;
using SistemaBiblioteca.Modelos;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>ABM de Autores.</summary>
    public partial class FrmAutores : Form
    {
        private readonly AutorDatos datos = new AutorDatos();
        private List<Autor> lista = new List<Autor>();
        private int idActual;

        public FrmAutores()
        {
            InitializeComponent();
            Ui.PrepararGrilla(dgvDatos);
        }

        private void FrmAutores_Load(object sender, EventArgs e) { Cargar(); }

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
            Ui.OcultarColumnas(dgvDatos, "IdAutor", "NombreCompleto");
        }

        private void Limpiar()
        {
            idActual = 0;
            txtNombres.Clear();
            txtApellidos.Clear();
            txtNacionalidad.Clear();
            dgvDatos.ClearSelection();
            txtNombres.Focus();
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var a = (Autor)dgvDatos.Rows[e.RowIndex].DataBoundItem;
            idActual = a.IdAutor;
            txtNombres.Text = a.Nombres;
            txtApellidos.Text = a.Apellidos;
            txtNacionalidad.Text = a.Nacionalidad;
        }

        private void btnNuevo_Click(object sender, EventArgs e) { Limpiar(); }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requeridos((txtNombres, "Nombres"), (txtApellidos, "Apellidos"))) return;

            var a = new Autor
            {
                IdAutor = idActual,
                Nombres = txtNombres.Text,
                Apellidos = txtApellidos.Text,
                Nacionalidad = txtNacionalidad.Text
            };
            try
            {
                if (idActual == 0) datos.Insertar(a); else datos.Actualizar(a);
                Ui.Info("Autor guardado correctamente.");
                Cargar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idActual == 0) { Ui.Advertencia("Seleccione un autor de la lista."); return; }
            if (!Ui.Confirmar("¿Eliminar el autor \"" + txtNombres.Text + " " + txtApellidos.Text + "\"?")) return;
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
