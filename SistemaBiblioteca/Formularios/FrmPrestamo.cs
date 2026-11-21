using System;
using System.Linq;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;
using SistemaBiblioteca.Modelos;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>CU-02: Registrar Préstamo (cabecera + detalle).</summary>
    public partial class FrmPrestamo : Form
    {
        private readonly PrestamoDatos prestamos = new PrestamoDatos();

        public FrmPrestamo()
        {
            InitializeComponent();
            Ui.PrepararGrilla(dgvDisponibles);
            dgvDisponibles.MultiSelect = true;
            Ui.PrepararGrilla(dgvPrestamos);
        }

        private void FrmPrestamo_Load(object sender, EventArgs e)
        {
            try
            {
                cboSocio.DataSource = new SocioDatos().ListarActivos();
                cboSocio.DisplayMember = "NombreCompleto";
                cboSocio.ValueMember = "IdSocio";
            }
            catch (Exception ex) { Ui.Error(ex); }
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                dgvDisponibles.DataSource = new EjemplarDatos().ListarDisponibles();
                Ui.OcultarColumnas(dgvDisponibles, "IdEjemplar", "IdLibro", "Estado");
                Ui.Encabezados(dgvDisponibles, "TituloLibro", "Libro", "CodigoBarras", "Código", "Ubicacion", "Ubicación");
                dgvDisponibles.ClearSelection();

                dgvPrestamos.DataSource = prestamos.ListarPrestamos();
            }
            catch (Exception ex) { Ui.Error(ex); }

            dtpDevolucion.MinDate = DateTime.Today;
            dtpDevolucion.Value = DateTime.Today.AddDays(7); // por defecto, 7 días
            txtObservacion.Clear();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (cboSocio.SelectedValue == null)
            {
                Ui.Advertencia("Seleccione el socio que retira el libro.");
                return;
            }
            if (dgvDisponibles.SelectedRows.Count == 0)
            {
                Ui.Advertencia("Seleccione al menos un ejemplar disponible.");
                return;
            }

            var p = new Prestamo
            {
                IdSocio = (int)cboSocio.SelectedValue,
                IdUsuario = Sesion.UsuarioActual.IdUsuario,
                FechaDevolucionPrevista = dtpDevolucion.Value.Date,
                Observacion = txtObservacion.Text
            };
            p.Detalles = dgvDisponibles.SelectedRows.Cast<DataGridViewRow>()
                .Select(r => new DetallePrestamo { IdEjemplar = ((Ejemplar)r.DataBoundItem).IdEjemplar })
                .ToList();

            try
            {
                int id = prestamos.Registrar(p);
                Ui.Info("Préstamo N° " + id + " registrado con " + p.Detalles.Count + " ejemplar(es).\n" +
                        "Devolución prevista: " + p.FechaDevolucionPrevista.ToShortDateString());
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
                Cargar();
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e) { Close(); }
    }
}
