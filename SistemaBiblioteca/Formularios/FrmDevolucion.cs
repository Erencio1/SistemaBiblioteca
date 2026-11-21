using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;

namespace SistemaBiblioteca.Formularios
{
    /// <summary>CU-03: Registrar Devolución.</summary>
    public partial class FrmDevolucion : Form
    {
        private readonly PrestamoDatos prestamos = new PrestamoDatos();

        public FrmDevolucion()
        {
            InitializeComponent();
            Ui.PrepararGrilla(dgvPendientes);
            dgvPendientes.CellFormatting += dgvPendientes_CellFormatting;
        }

        private void FrmDevolucion_Load(object sender, EventArgs e) { Cargar(); }

        private void Cargar()
        {
            try
            {
                prestamos.ActualizarAtrasados();
                dgvPendientes.DataSource = prestamos.ListarPendientes();
                Ui.OcultarColumnas(dgvPendientes, "IdDetalle");
                dgvPendientes.ClearSelection();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        /// <summary>Resalta en rojo los préstamos atrasados.</summary>
        private void dgvPendientes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var fila = dgvPendientes.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (fila != null && Convert.ToInt32(fila["Días de atraso"]) > 0)
                e.CellStyle.ForeColor = Color.Firebrick;
        }

        private void btnDevolver_Click(object sender, EventArgs e)
        {
            if (dgvPendientes.SelectedRows.Count == 0)
            {
                Ui.Advertencia("Seleccione el ejemplar que se devuelve.");
                return;
            }

            var fila = (DataRowView)dgvPendientes.SelectedRows[0].DataBoundItem;
            int idDetalle = Convert.ToInt32(fila["IdDetalle"]);
            string libro = Convert.ToString(fila["Libro"]);
            string socio = Convert.ToString(fila["Socio"]);
            int atraso = Convert.ToInt32(fila["Días de atraso"]);

            string mensaje = "¿Confirmar la devolución de \"" + libro + "\" por " + socio + "?";
            if (atraso > 0)
                mensaje += "\n\nAtención: la devolución tiene " + atraso + " día(s) de atraso.";
            if (!Ui.Confirmar(mensaje)) return;

            try
            {
                bool completo = prestamos.RegistrarDevolucion(idDetalle);
                Ui.Info(completo
                    ? "Devolución registrada. El préstamo quedó marcado como \"Devuelto\"."
                    : "Devolución registrada. El préstamo aún tiene otros ejemplares pendientes.");
            }
            catch (Exception ex) { Ui.Error(ex); }
            Cargar();
        }

        private void btnActualizar_Click(object sender, EventArgs e) { Cargar(); }

        private void btnCerrar_Click(object sender, EventArgs e) { Close(); }
    }
}
