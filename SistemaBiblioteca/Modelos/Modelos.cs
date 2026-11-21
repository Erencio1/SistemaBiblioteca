using System;
using System.Collections.Generic;


namespace SistemaBiblioteca.Modelos
{
    public class Rol
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public string NombreCompleto { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; }          
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        public bool EsAdministrador =>
            string.Equals(NombreRol, "Administrador", StringComparison.OrdinalIgnoreCase);
    }

    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

    public class Editorial
    {
        public int IdEditorial { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
    }

    public class Autor
    {
        public int IdAutor { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Nacionalidad { get; set; }

        public string NombreCompleto => Apellidos + ", " + Nombres;
    }

    public class Libro
    {
        public int IdLibro { get; set; }
        public string Titulo { get; set; }
        public string ISBN { get; set; }
        public int? Anio { get; set; }
        public int IdCategoria { get; set; }
        public int IdEditorial { get; set; }
        public string NombreCategoria { get; set; }    
        public string NombreEditorial { get; set; }    
        public string Autores { get; set; }            

        public List<int> IdsAutores { get; set; } = new List<int>();
    }

    public class Ejemplar
    {
        public const string Disponible = "Disponible";
        public const string Prestado = "Prestado";
        public const string Baja = "Baja";

        public int IdEjemplar { get; set; }
        public int IdLibro { get; set; }
        public string TituloLibro { get; set; }        
        public string CodigoBarras { get; set; }
        public string Ubicacion { get; set; }
        public string Estado { get; set; }
    }

    public class Socio
    {
        public int IdSocio { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Documento { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }

        public string NombreCompleto => Apellidos + ", " + Nombres + " (" + Documento + ")";
    }

    public class Prestamo
    {
        public const string Activo = "Activo";
        public const string Devuelto = "Devuelto";
        public const string Atrasado = "Atrasado";

        public int IdPrestamo { get; set; }
        public int IdSocio { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucionPrevista { get; set; }
        public string Estado { get; set; }
        public string Observacion { get; set; }

        public List<DetallePrestamo> Detalles { get; set; } = new List<DetallePrestamo>();
    }

    public class DetallePrestamo
    {
        public int IdDetalle { get; set; }
        public int IdPrestamo { get; set; }
        public int IdEjemplar { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
    }
}
