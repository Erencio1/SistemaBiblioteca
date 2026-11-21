namespace SistemaBiblioteca.Formularios
{
    partial class FrmPrincipal
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
            this.menuPrincipal = new System.Windows.Forms.MenuStrip();
            this.mnuMantenimientos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCategorias = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditoriales = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAutores = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLibros = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEjemplares = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSocios = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeparador1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuUsuarios = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMovimientos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPrestamo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDevolucion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSistema = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCerrarSesion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSalir = new System.Windows.Forms.ToolStripMenuItem();
            this.barraEstado = new System.Windows.Forms.StatusStrip();
            this.lblUsuario = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuPrincipal.SuspendLayout();
            this.barraEstado.SuspendLayout();
            this.SuspendLayout();
            //
            // menuPrincipal
            //
            this.menuPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMantenimientos,
            this.mnuMovimientos,
            this.mnuSistema});
            this.menuPrincipal.Location = new System.Drawing.Point(0, 0);
            this.menuPrincipal.Name = "menuPrincipal";
            this.menuPrincipal.Size = new System.Drawing.Size(984, 24);
            this.menuPrincipal.TabIndex = 0;
            //
            // mnuMantenimientos
            //
            this.mnuMantenimientos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCategorias,
            this.mnuEditoriales,
            this.mnuAutores,
            this.mnuLibros,
            this.mnuEjemplares,
            this.mnuSocios,
            this.mnuSeparador1,
            this.mnuUsuarios});
            this.mnuMantenimientos.Name = "mnuMantenimientos";
            this.mnuMantenimientos.Text = "&Mantenimientos";
            //
            this.mnuCategorias.Name = "mnuCategorias";
            this.mnuCategorias.Text = "Categorías";
            this.mnuCategorias.Click += new System.EventHandler(this.mnuCategorias_Click);
            //
            this.mnuEditoriales.Name = "mnuEditoriales";
            this.mnuEditoriales.Text = "Editoriales";
            this.mnuEditoriales.Click += new System.EventHandler(this.mnuEditoriales_Click);
            //
            this.mnuAutores.Name = "mnuAutores";
            this.mnuAutores.Text = "Autores";
            this.mnuAutores.Click += new System.EventHandler(this.mnuAutores_Click);
            //
            this.mnuLibros.Name = "mnuLibros";
            this.mnuLibros.Text = "Libros";
            this.mnuLibros.Click += new System.EventHandler(this.mnuLibros_Click);
            //
            this.mnuEjemplares.Name = "mnuEjemplares";
            this.mnuEjemplares.Text = "Ejemplares";
            this.mnuEjemplares.Click += new System.EventHandler(this.mnuEjemplares_Click);
            //
            this.mnuSocios.Name = "mnuSocios";
            this.mnuSocios.Text = "Socios";
            this.mnuSocios.Click += new System.EventHandler(this.mnuSocios_Click);
            //
            this.mnuSeparador1.Name = "mnuSeparador1";
            //
            this.mnuUsuarios.Name = "mnuUsuarios";
            this.mnuUsuarios.Text = "Usuarios";
            this.mnuUsuarios.Click += new System.EventHandler(this.mnuUsuarios_Click);
            //
            // mnuMovimientos
            //
            this.mnuMovimientos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPrestamo,
            this.mnuDevolucion});
            this.mnuMovimientos.Name = "mnuMovimientos";
            this.mnuMovimientos.Text = "M&ovimientos";
            //
            this.mnuPrestamo.Name = "mnuPrestamo";
            this.mnuPrestamo.Text = "Registrar Préstamo";
            this.mnuPrestamo.Click += new System.EventHandler(this.mnuPrestamo_Click);
            //
            this.mnuDevolucion.Name = "mnuDevolucion";
            this.mnuDevolucion.Text = "Registrar Devolución";
            this.mnuDevolucion.Click += new System.EventHandler(this.mnuDevolucion_Click);
            //
            // mnuSistema
            //
            this.mnuSistema.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCerrarSesion,
            this.mnuSalir});
            this.mnuSistema.Name = "mnuSistema";
            this.mnuSistema.Text = "&Sistema";
            //
            this.mnuCerrarSesion.Name = "mnuCerrarSesion";
            this.mnuCerrarSesion.Text = "Cerrar sesión";
            this.mnuCerrarSesion.Click += new System.EventHandler(this.mnuCerrarSesion_Click);
            //
            this.mnuSalir.Name = "mnuSalir";
            this.mnuSalir.Text = "Salir";
            this.mnuSalir.Click += new System.EventHandler(this.mnuSalir_Click);
            //
            // barraEstado
            //
            this.barraEstado.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUsuario});
            this.barraEstado.Location = new System.Drawing.Point(0, 639);
            this.barraEstado.Name = "barraEstado";
            this.barraEstado.Size = new System.Drawing.Size(984, 22);
            this.barraEstado.TabIndex = 1;
            //
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Text = "Usuario:";
            //
            // FrmPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.barraEstado);
            this.Controls.Add(this.menuPrincipal);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuPrincipal;
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sistema de Gestión de Biblioteca";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            this.menuPrincipal.ResumeLayout(false);
            this.menuPrincipal.PerformLayout();
            this.barraEstado.ResumeLayout(false);
            this.barraEstado.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuPrincipal;
        private System.Windows.Forms.ToolStripMenuItem mnuMantenimientos;
        private System.Windows.Forms.ToolStripMenuItem mnuCategorias;
        private System.Windows.Forms.ToolStripMenuItem mnuEditoriales;
        private System.Windows.Forms.ToolStripMenuItem mnuAutores;
        private System.Windows.Forms.ToolStripMenuItem mnuLibros;
        private System.Windows.Forms.ToolStripMenuItem mnuEjemplares;
        private System.Windows.Forms.ToolStripMenuItem mnuSocios;
        private System.Windows.Forms.ToolStripSeparator mnuSeparador1;
        private System.Windows.Forms.ToolStripMenuItem mnuUsuarios;
        private System.Windows.Forms.ToolStripMenuItem mnuMovimientos;
        private System.Windows.Forms.ToolStripMenuItem mnuPrestamo;
        private System.Windows.Forms.ToolStripMenuItem mnuDevolucion;
        private System.Windows.Forms.ToolStripMenuItem mnuSistema;
        private System.Windows.Forms.ToolStripMenuItem mnuCerrarSesion;
        private System.Windows.Forms.ToolStripMenuItem mnuSalir;
        private System.Windows.Forms.StatusStrip barraEstado;
        private System.Windows.Forms.ToolStripStatusLabel lblUsuario;
    }
}
