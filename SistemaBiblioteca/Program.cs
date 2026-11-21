using System;
using System.Windows.Forms;
using SistemaBiblioteca.Datos;
using SistemaBiblioteca.Formularios;

namespace SistemaBiblioteca
{
    internal static class Program
    {
        
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                using (var login = new FrmLogin())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                        return;
                }

                var principal = new FrmPrincipal();
                Application.Run(principal);

                if (!principal.CerrarSesion)
                    return;

                Sesion.UsuarioActual = null;
            }
        }
    }
}
