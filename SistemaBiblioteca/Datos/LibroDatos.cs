using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaBiblioteca.Modelos;
using static SistemaBiblioteca.Datos.Conexion;

namespace SistemaBiblioteca.Datos
{
    public class LibroDatos
    {
        public List<Libro> Listar()
        {
            const string sql = @"
                SELECT l.IdLibro, l.Titulo, l.ISBN, l.Anio, l.IdCategoria, l.IdEditorial,
                       c.Nombre AS NombreCategoria, e.Nombre AS NombreEditorial,
                       STUFF((SELECT ', ' + a.Nombres + ' ' + a.Apellidos
                              FROM LibroAutor la INNER JOIN Autor a ON a.IdAutor = la.IdAutor
                              WHERE la.IdLibro = l.IdLibro
                              ORDER BY a.Apellidos
                              FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Autores
                FROM Libro l
                INNER JOIN Categoria c ON c.IdCategoria = l.IdCategoria
                INNER JOIN Editorial e ON e.IdEditorial = l.IdEditorial
                ORDER BY l.Titulo";

            var lista = new List<Libro>();
            foreach (DataRow f in Consultar(sql).Rows)
            {
                lista.Add(new Libro
                {
                    IdLibro = Convert.ToInt32(f["IdLibro"]),
                    Titulo = Texto(f, "Titulo"),
                    ISBN = Texto(f, "ISBN"),
                    Anio = EnteroNulo(f, "Anio"),
                    IdCategoria = Convert.ToInt32(f["IdCategoria"]),
                    IdEditorial = Convert.ToInt32(f["IdEditorial"]),
                    NombreCategoria = Texto(f, "NombreCategoria"),
                    NombreEditorial = Texto(f, "NombreEditorial"),
                    Autores = Texto(f, "Autores")
                });
            }
            return lista;
        }

        public List<int> ObtenerAutores(int idLibro)
        {
            var ids = new List<int>();
            foreach (DataRow f in Consultar("SELECT IdAutor FROM LibroAutor WHERE IdLibro = @Id", P("@Id", idLibro)).Rows)
                ids.Add(Convert.ToInt32(f["IdAutor"]));
            return ids;
        }

        public void Insertar(Libro l)
        {
            using (var cn = Obtener())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        var cmd = new SqlCommand(@"
                            INSERT INTO Libro (Titulo, ISBN, Anio, IdCategoria, IdEditorial)
                            VALUES (@Titulo, @ISBN, @Anio, @IdCategoria, @IdEditorial);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);", cn, tx);
                        AgregarParametros(cmd, l);
                        l.IdLibro = (int)cmd.ExecuteScalar();

                        GuardarAutores(cn, tx, l);
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Actualizar(Libro l)
        {
            using (var cn = Obtener())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        var cmd = new SqlCommand(@"
                            UPDATE Libro SET Titulo = @Titulo, ISBN = @ISBN, Anio = @Anio,
                                   IdCategoria = @IdCategoria, IdEditorial = @IdEditorial
                            WHERE IdLibro = @IdLibro", cn, tx);
                        AgregarParametros(cmd, l);
                        cmd.Parameters.Add(P("@IdLibro", l.IdLibro));
                        cmd.ExecuteNonQuery();

                        var borrar = new SqlCommand("DELETE FROM LibroAutor WHERE IdLibro = @IdLibro", cn, tx);
                        borrar.Parameters.Add(P("@IdLibro", l.IdLibro));
                        borrar.ExecuteNonQuery();

                        GuardarAutores(cn, tx, l);
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Eliminar(int id)
        {
            Ejecutar("DELETE FROM Libro WHERE IdLibro = @Id", P("@Id", id));
        }

        private static void AgregarParametros(SqlCommand cmd, Libro l)
        {
            cmd.Parameters.Add(P("@Titulo", l.Titulo.Trim()));
            cmd.Parameters.Add(P("@ISBN", Opcional(l.ISBN)));
            cmd.Parameters.Add(P("@Anio", l.Anio));
            cmd.Parameters.Add(P("@IdCategoria", l.IdCategoria));
            cmd.Parameters.Add(P("@IdEditorial", l.IdEditorial));
        }

        private static void GuardarAutores(SqlConnection cn, SqlTransaction tx, Libro l)
        {
            foreach (int idAutor in l.IdsAutores)
            {
                var cmd = new SqlCommand("INSERT INTO LibroAutor (IdLibro, IdAutor) VALUES (@IdLibro, @IdAutor)", cn, tx);
                cmd.Parameters.Add(P("@IdLibro", l.IdLibro));
                cmd.Parameters.Add(P("@IdAutor", idAutor));
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class EjemplarDatos
    {
        private const string SelectBase = @"
            SELECT e.IdEjemplar, e.IdLibro, l.Titulo AS TituloLibro, e.CodigoBarras, e.Ubicacion, e.Estado
            FROM Ejemplar e INNER JOIN Libro l ON l.IdLibro = e.IdLibro ";

        public List<Ejemplar> Listar()
        {
            return Mapear(Consultar(SelectBase + "ORDER BY l.Titulo, e.CodigoBarras"));
        }

        public List<Ejemplar> ListarDisponibles()
        {
            return Mapear(Consultar(SelectBase + "WHERE e.Estado = 'Disponible' ORDER BY l.Titulo, e.CodigoBarras"));
        }

        public void Insertar(Ejemplar e)
        {
            Ejecutar("INSERT INTO Ejemplar (IdLibro, CodigoBarras, Ubicacion, Estado) VALUES (@IdLibro, @Codigo, @Ubicacion, @Estado)",
                P("@IdLibro", e.IdLibro), P("@Codigo", e.CodigoBarras.Trim()),
                P("@Ubicacion", Opcional(e.Ubicacion)), P("@Estado", e.Estado));
        }

        public void Actualizar(Ejemplar e)
        {
            Ejecutar(@"UPDATE Ejemplar SET IdLibro = @IdLibro, CodigoBarras = @Codigo, Ubicacion = @Ubicacion, Estado = @Estado
                       WHERE IdEjemplar = @Id",
                P("@IdLibro", e.IdLibro), P("@Codigo", e.CodigoBarras.Trim()),
                P("@Ubicacion", Opcional(e.Ubicacion)), P("@Estado", e.Estado), P("@Id", e.IdEjemplar));
        }

        public void Eliminar(int id)
        {
            Ejecutar("DELETE FROM Ejemplar WHERE IdEjemplar = @Id", P("@Id", id));
        }

        private static List<Ejemplar> Mapear(DataTable tabla)
        {
            var lista = new List<Ejemplar>();
            foreach (DataRow f in tabla.Rows)
            {
                lista.Add(new Ejemplar
                {
                    IdEjemplar = Convert.ToInt32(f["IdEjemplar"]),
                    IdLibro = Convert.ToInt32(f["IdLibro"]),
                    TituloLibro = Texto(f, "TituloLibro"),
                    CodigoBarras = Texto(f, "CodigoBarras"),
                    Ubicacion = Texto(f, "Ubicacion"),
                    Estado = Texto(f, "Estado")
                });
            }
            return lista;
        }
    }
}
