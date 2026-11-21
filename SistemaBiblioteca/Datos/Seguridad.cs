using System.Security.Cryptography;
using System.Text;
using SistemaBiblioteca.Modelos;

namespace SistemaBiblioteca.Datos
{
    public static class Seguridad
    {
        
        public static string Hash(string texto)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(texto ?? string.Empty));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes)
                    sb.Append(b.ToString("X2"));
                return sb.ToString();
            }
        }
    }

    public static class Sesion
    {
        public static Usuario UsuarioActual { get; set; }
    }
}
