using System;
using System.Collections.Generic;
using System.Data;
using SistemaBiblioteca.Modelos;
using static SistemaBiblioteca.Datos.Conexion;

namespace SistemaBiblioteca.Datos
{
    
    public class CategoriaDatos
    {
        public List<Categoria> Listar()
        {
            var lista = new List<Categoria>();
            foreach (DataRow f in Consultar("SELECT IdCategoria, Nombre, Descripcion FROM Categoria ORDER BY Nombre").Rows)
            {
                lista.Add(new Categoria
                {
                    IdCategoria = Convert.ToInt32(f["IdCategoria"]),
                    Nombre = Texto(f, "Nombre"),
                    Descripcion = Texto(f, "Descripcion")
                });
            }
            return lista;
        }

        public void Insertar(Categoria c)
        {
            Ejecutar("INSERT INTO Categoria (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)",
                P("@Nombre", c.Nombre.Trim()), P("@Descripcion", Opcional(c.Descripcion)));
        }

        public void Actualizar(Categoria c)
        {
            Ejecutar("UPDATE Categoria SET Nombre = @Nombre, Descripcion = @Descripcion WHERE IdCategoria = @Id",
                P("@Nombre", c.Nombre.Trim()), P("@Descripcion", Opcional(c.Descripcion)), P("@Id", c.IdCategoria));
        }

        public void Eliminar(int id)
        {
            Ejecutar("DELETE FROM Categoria WHERE IdCategoria = @Id", P("@Id", id));
        }
    }

    
    public class EditorialDatos
    {
        public List<Editorial> Listar()
        {
            var lista = new List<Editorial>();
            foreach (DataRow f in Consultar("SELECT IdEditorial, Nombre, Telefono, Email, Direccion FROM Editorial ORDER BY Nombre").Rows)
            {
                lista.Add(new Editorial
                {
                    IdEditorial = Convert.ToInt32(f["IdEditorial"]),
                    Nombre = Texto(f, "Nombre"),
                    Telefono = Texto(f, "Telefono"),
                    Email = Texto(f, "Email"),
                    Direccion = Texto(f, "Direccion")
                });
            }
            return lista;
        }

        public void Insertar(Editorial e)
        {
            Ejecutar("INSERT INTO Editorial (Nombre, Telefono, Email, Direccion) VALUES (@Nombre, @Telefono, @Email, @Direccion)",
                P("@Nombre", e.Nombre.Trim()), P("@Telefono", Opcional(e.Telefono)),
                P("@Email", Opcional(e.Email)), P("@Direccion", Opcional(e.Direccion)));
        }

        public void Actualizar(Editorial e)
        {
            Ejecutar(@"UPDATE Editorial SET Nombre = @Nombre, Telefono = @Telefono, Email = @Email, Direccion = @Direccion
                       WHERE IdEditorial = @Id",
                P("@Nombre", e.Nombre.Trim()), P("@Telefono", Opcional(e.Telefono)),
                P("@Email", Opcional(e.Email)), P("@Direccion", Opcional(e.Direccion)), P("@Id", e.IdEditorial));
        }

        public void Eliminar(int id)
        {
            Ejecutar("DELETE FROM Editorial WHERE IdEditorial = @Id", P("@Id", id));
        }
    }

    
    public class AutorDatos
    {
        public List<Autor> Listar()
        {
            var lista = new List<Autor>();
            foreach (DataRow f in Consultar("SELECT IdAutor, Nombres, Apellidos, Nacionalidad FROM Autor ORDER BY Apellidos, Nombres").Rows)
            {
                lista.Add(new Autor
                {
                    IdAutor = Convert.ToInt32(f["IdAutor"]),
                    Nombres = Texto(f, "Nombres"),
                    Apellidos = Texto(f, "Apellidos"),
                    Nacionalidad = Texto(f, "Nacionalidad")
                });
            }
            return lista;
        }

        public void Insertar(Autor a)
        {
            Ejecutar("INSERT INTO Autor (Nombres, Apellidos, Nacionalidad) VALUES (@Nombres, @Apellidos, @Nacionalidad)",
                P("@Nombres", a.Nombres.Trim()), P("@Apellidos", a.Apellidos.Trim()), P("@Nacionalidad", Opcional(a.Nacionalidad)));
        }

        public void Actualizar(Autor a)
        {
            Ejecutar("UPDATE Autor SET Nombres = @Nombres, Apellidos = @Apellidos, Nacionalidad = @Nacionalidad WHERE IdAutor = @Id",
                P("@Nombres", a.Nombres.Trim()), P("@Apellidos", a.Apellidos.Trim()),
                P("@Nacionalidad", Opcional(a.Nacionalidad)), P("@Id", a.IdAutor));
        }

        public void Eliminar(int id)
        {
            Ejecutar("DELETE FROM Autor WHERE IdAutor = @Id", P("@Id", id));
        }
    }
}
