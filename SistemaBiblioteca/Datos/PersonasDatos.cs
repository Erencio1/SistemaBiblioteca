using System;
using System.Collections.Generic;
using System.Data;
using SistemaBiblioteca.Modelos;
using static SistemaBiblioteca.Datos.Conexion;

namespace SistemaBiblioteca.Datos
{
    public class RolDatos
    {
        public List<Rol> Listar()
        {
            var lista = new List<Rol>();
            foreach (DataRow f in Consultar("SELECT IdRol, Nombre, Descripcion FROM Rol ORDER BY Nombre").Rows)
            {
                lista.Add(new Rol
                {
                    IdRol = Convert.ToInt32(f["IdRol"]),
                    Nombre = Texto(f, "Nombre"),
                    Descripcion = Texto(f, "Descripcion")
                });
            }
            return lista;
        }
    }

    public class UsuarioDatos
    {
        private const string SelectBase = @"
            SELECT u.IdUsuario, u.NombreUsuario, u.Clave, u.NombreCompleto, u.IdRol, r.Nombre AS NombreRol,
                   u.Activo, u.FechaCreacion
            FROM Usuario u INNER JOIN Rol r ON r.IdRol = u.IdRol ";

        
        public Usuario Login(string nombreUsuario, string clave)
        {
            var tabla = Consultar(SelectBase + "WHERE u.NombreUsuario = @Usuario AND u.Clave = @Clave AND u.Activo = 1",
                P("@Usuario", nombreUsuario.Trim()), P("@Clave", Seguridad.Hash(clave)));
            return tabla.Rows.Count == 0 ? null : Mapear(tabla.Rows[0]);
        }

        public List<Usuario> Listar()
        {
            var lista = new List<Usuario>();
            foreach (DataRow f in Consultar(SelectBase + "ORDER BY u.NombreUsuario").Rows)
                lista.Add(Mapear(f));
            return lista;
        }

        public void Insertar(Usuario u)
        {
            Ejecutar(@"INSERT INTO Usuario (NombreUsuario, Clave, NombreCompleto, IdRol, Activo, FechaCreacion)
                       VALUES (@Usuario, @Clave, @Nombre, @IdRol, @Activo, GETDATE())",
                P("@Usuario", u.NombreUsuario.Trim()), P("@Clave", Seguridad.Hash(u.Clave)),
                P("@Nombre", u.NombreCompleto.Trim()), P("@IdRol", u.IdRol), P("@Activo", u.Activo));
        }

        public void Actualizar(Usuario u)
        {
            if (string.IsNullOrEmpty(u.Clave))
            {
                Ejecutar(@"UPDATE Usuario SET NombreUsuario = @Usuario, NombreCompleto = @Nombre, IdRol = @IdRol, Activo = @Activo
                           WHERE IdUsuario = @Id",
                    P("@Usuario", u.NombreUsuario.Trim()), P("@Nombre", u.NombreCompleto.Trim()),
                    P("@IdRol", u.IdRol), P("@Activo", u.Activo), P("@Id", u.IdUsuario));
            }
            else
            {
                Ejecutar(@"UPDATE Usuario SET NombreUsuario = @Usuario, Clave = @Clave, NombreCompleto = @Nombre,
                                  IdRol = @IdRol, Activo = @Activo
                           WHERE IdUsuario = @Id",
                    P("@Usuario", u.NombreUsuario.Trim()), P("@Clave", Seguridad.Hash(u.Clave)),
                    P("@Nombre", u.NombreCompleto.Trim()), P("@IdRol", u.IdRol), P("@Activo", u.Activo),
                    P("@Id", u.IdUsuario));
            }
        }

        public void Eliminar(int id)
        {
            Ejecutar("DELETE FROM Usuario WHERE IdUsuario = @Id", P("@Id", id));
        }

        private static Usuario Mapear(DataRow f)
        {
            return new Usuario
            {
                IdUsuario = Convert.ToInt32(f["IdUsuario"]),
                NombreUsuario = Texto(f, "NombreUsuario"),
                Clave = Texto(f, "Clave"),
                NombreCompleto = Texto(f, "NombreCompleto"),
                IdRol = Convert.ToInt32(f["IdRol"]),
                NombreRol = Texto(f, "NombreRol"),
                Activo = Convert.ToBoolean(f["Activo"]),
                FechaCreacion = Convert.ToDateTime(f["FechaCreacion"])
            };
        }
    }

    public class SocioDatos
    {
        private const string SelectBase =
            "SELECT IdSocio, Nombres, Apellidos, Documento, Telefono, Email, Direccion, FechaRegistro, Activo FROM Socio ";

        public List<Socio> Listar()
        {
            return Mapear(Consultar(SelectBase + "ORDER BY Apellidos, Nombres"));
        }

        public List<Socio> ListarActivos()
        {
            return Mapear(Consultar(SelectBase + "WHERE Activo = 1 ORDER BY Apellidos, Nombres"));
        }

        public void Insertar(Socio s)
        {
            Ejecutar(@"INSERT INTO Socio (Nombres, Apellidos, Documento, Telefono, Email, Direccion, FechaRegistro, Activo)
                       VALUES (@Nombres, @Apellidos, @Documento, @Telefono, @Email, @Direccion, GETDATE(), @Activo)",
                P("@Nombres", s.Nombres.Trim()), P("@Apellidos", s.Apellidos.Trim()), P("@Documento", s.Documento.Trim()),
                P("@Telefono", Opcional(s.Telefono)), P("@Email", Opcional(s.Email)),
                P("@Direccion", Opcional(s.Direccion)), P("@Activo", s.Activo));
        }

        public void Actualizar(Socio s)
        {
            Ejecutar(@"UPDATE Socio SET Nombres = @Nombres, Apellidos = @Apellidos, Documento = @Documento,
                              Telefono = @Telefono, Email = @Email, Direccion = @Direccion, Activo = @Activo
                       WHERE IdSocio = @Id",
                P("@Nombres", s.Nombres.Trim()), P("@Apellidos", s.Apellidos.Trim()), P("@Documento", s.Documento.Trim()),
                P("@Telefono", Opcional(s.Telefono)), P("@Email", Opcional(s.Email)),
                P("@Direccion", Opcional(s.Direccion)), P("@Activo", s.Activo), P("@Id", s.IdSocio));
        }

        public void Eliminar(int id)
        {
            Ejecutar("DELETE FROM Socio WHERE IdSocio = @Id", P("@Id", id));
        }

        private static List<Socio> Mapear(DataTable tabla)
        {
            var lista = new List<Socio>();
            foreach (DataRow f in tabla.Rows)
            {
                lista.Add(new Socio
                {
                    IdSocio = Convert.ToInt32(f["IdSocio"]),
                    Nombres = Texto(f, "Nombres"),
                    Apellidos = Texto(f, "Apellidos"),
                    Documento = Texto(f, "Documento"),
                    Telefono = Texto(f, "Telefono"),
                    Email = Texto(f, "Email"),
                    Direccion = Texto(f, "Direccion"),
                    FechaRegistro = Convert.ToDateTime(f["FechaRegistro"]),
                    Activo = Convert.ToBoolean(f["Activo"])
                });
            }
            return lista;
        }
    }
}
