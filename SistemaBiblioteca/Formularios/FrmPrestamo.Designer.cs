namespace SistemaBiblioteca.Formularios
{
    partial class FrmPrestamo
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.lblSocio = new System.Windows.Forms.Label();
            this.cboSocio = new System.Windows.Forms.ComboBox();
            this.lblDisponibles = new System.Windows.Forms.Label();
            this.dgvDisponibles = new System.Windows.Forms.DataGridView();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpDevolucion = new System.Windows.Forms.DateTimePicker();
            this.lblObservacion = new System.Windows.Forms.Label();
            this.txtObservacion = new System.Windows.Forms.TextBox();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.lblHistorial = new System.Windows.Forms.Label();
            this.dgvPrestamos = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisponibles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrestamos)).BeginInit();
            this.SuspendLayout();
            //
            // lblSocio
            //
            this.lblSocio.AutoSize = true;
            this.lblSocio.Location = new System.Drawing.Point(12, 15);
            this.lblSocio.Name = "lblSocio";
            this.lblSocio.Text = "Socio:";
            //
            // cboSocio
            //
            this.cboSocio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSocio.Location = new System.Drawing.Point(140, 12);
            this.cboSocio.Name = "cboSocio";
            this.cboSocio.Size = new System.Drawing.Size(400, 21);
            this.cboSocio.TabIndex = 0;
            //
            // lblDisponibles
            //
            this.lblDisponibles.AutoSize = true;
            this.lblDisponibles.Location = new System.Drawing.Point(12, 45);
            this.lblDisponibles.Name = "lblDisponibles";
            this.lblDisponibles.Text = "Ejemplares disponibles (Ctrl + clic para seleccionar varios):";
            //
            // dgvDisponibles
            //
            this.dgvDisponibles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDisponibles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDisponibles.Location = new System.Drawing.Point(12, 65);
            this.dgvDisponibles.Name = "dgvDisponibles";
            this.dgvDisponibles.Size = new System.Drawing.Size(876, 180);
            this.dgvDisponibles.TabIndex = 1;
            //
            // lblFecha
            //
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(12, 260);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Text = "Devolución prevista:";
            //
            // dtpDevolucion
            //
            this.dtpDevolucion.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDevolucion.Location = new System.Drawing.Point(140, 256);
            this.dtpDevolucion.Name = "dtpDevolucion";
            this.dtpDevolucion.Size = new System.Drawing.Size(150, 20);
            this.dtpDevolucion.TabIndex = 2;
            //
            // lblObservacion
            //
            this.lblObservacion.AutoSize = true;
            this.lblObservacion.Location = new System.Drawing.Point(310, 260);
            this.lblObservacion.Name = "lblObservacion";
            this.lblObservacion.Text = "Observación:";
            //
            // txtObservacion
            //
            this.txtObservacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtObservacion.Location = new System.Drawing.Point(390, 256);
            this.txtObservacion.MaxLength = 200;
            this.txtObservacion.Name = "txtObservacion";
            this.txtObservacion.Size = new System.Drawing.Size(498, 20);
            this.txtObservacion.TabIndex = 3;
            //
            // btnRegistrar
            //
            this.btnRegistrar.Location = new System.Drawing.Point(12, 290);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(160, 32);
            this.btnRegistrar.TabIndex = 4;
            this.btnRegistrar.Text = "Registrar Préstamo";
            this.btnRegistrar.UseVisualStyleBackColor = true;
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(182, 290);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 32);
            this.btnCerrar.TabIndex = 5;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // lblHistorial
            //
            this.lblHistorial.AutoSize = true;
            this.lblHistorial.Location = new System.Drawing.Point(12, 335);
            this.lblHistorial.Name = "lblHistorial";
            this.lblHistorial.Text = "Préstamos registrados:";
            //
            // dgvPrestamos
            //
            this.dgvPrestamos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPrestamos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrestamos.Location = new System.Drawing.Point(12, 355);
            this.dgvPrestamos.Name = "dgvPrestamos";
            this.dgvPrestamos.Size = new System.Drawing.Size(876, 253);
            this.dgvPrestamos.TabIndex = 6;
            //
            // FrmPrestamo
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 620);
            this.Controls.Add(this.lblSocio);
            this.Controls.Add(this.cboSocio);
            this.Controls.Add(this.lblDisponibles);
            this.Controls.Add(this.dgvDisponibles);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.dtpDevolucion);
            this.Controls.Add(this.lblObservacion);
            this.Controls.Add(this.txtObservacion);
            this.Controls.Add(this.btnRegistrar);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lblHistorial);
            this.Controls.Add(this.dgvPrestamos);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "FrmPrestamo";
            this.Text = "Registrar Préstamo";
            this.Load += new System.EventHandler(this.FrmPrestamo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisponibles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrestamos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblSocio;
        private System.Windows.Forms.ComboBox cboSocio;
        private System.Windows.Forms.Label lblDisponibles;
        private System.Windows.Forms.DataGridView dgvDisponibles;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpDevolucion;
        private System.Windows.Forms.Label lblObservacion;
        private System.Windows.Forms.TextBox txtObservacion;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Label lblHistorial;
        private System.Windows.Forms.DataGridView dgvPrestamos;
    }
}
