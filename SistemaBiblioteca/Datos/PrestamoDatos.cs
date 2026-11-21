using System;
using System.Data;
using System.Data.SqlClient;
using SistemaBiblioteca.Modelos;
using static SistemaBiblioteca.Datos.Conexion;

namespace SistemaBiblioteca.Datos
{
    public class PrestamoDatos
    {
        
        public int Registrar(Prestamo p)
        {
            if (p.Detalles.Count == 0)
                throw new InvalidOperationException("El préstamo debe tener al menos un ejemplar.");

            using (var cn = Obtener())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        var cab = new SqlCommand(@"
                            INSERT INTO Prestamo (IdSocio, IdUsuario, FechaPrestamo, FechaDevolucionPrevista, Estado, Observacion)
                            VALUES (@IdSocio, @IdUsuario, GETDATE(), @FechaPrevista, 'Activo', @Observacion);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);", cn, tx);
                        cab.Parameters.Add(P("@IdSocio", p.IdSocio));
                        cab.Parameters.Add(P("@IdUsuario", p.IdUsuario));
                        cab.Parameters.Add(P("@FechaPrevista", p.FechaDevolucionPrevista.Date));
                        cab.Parameters.Add(P("@Observacion", Opcional(p.Observacion)));
                        p.IdPrestamo = (int)cab.ExecuteScalar();

                        foreach (var d in p.Detalles)
                        {
                            var marcar = new SqlCommand(
                                "UPDATE Ejemplar SET Estado = 'Prestado' WHERE IdEjemplar = @IdEjemplar AND Estado = 'Disponible'", cn, tx);
                            marcar.Parameters.Add(P("@IdEjemplar", d.IdEjemplar));
                            if (marcar.ExecuteNonQuery() == 0)
                                throw new InvalidOperationException("Uno de los ejemplares seleccionados ya no está disponible.");

                            var det = new SqlCommand(
                                "INSERT INTO DetallePrestamo (IdPrestamo, IdEjemplar) VALUES (@IdPrestamo, @IdEjemplar)", cn, tx);
                            det.Parameters.Add(P("@IdPrestamo", p.IdPrestamo));
                            det.Parameters.Add(P("@IdEjemplar", d.IdEjemplar));
                            det.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return p.IdPrestamo;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

       
        public bool RegistrarDevolucion(int idDetalle)
        {
            using (var cn = Obtener())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        var leer = new SqlCommand(
                            "SELECT IdPrestamo, IdEjemplar FROM DetallePrestamo WHERE IdDetalle = @Id AND FechaDevolucionReal IS NULL", cn, tx);
                        leer.Parameters.Add(P("@Id", idDetalle));
                        int idPrestamo, idEjemplar;
                        using (var r = leer.ExecuteReader())
                        {
                            if (!r.Read())
                                throw new InvalidOperationException("Ese ejemplar ya fue devuelto.");
                            idPrestamo = r.GetInt32(0);
                            idEjemplar = r.GetInt32(1);
                        }

                        var det = new SqlCommand("UPDATE DetallePrestamo SET FechaDevolucionReal = GETDATE() WHERE IdDetalle = @Id", cn, tx);
                        det.Parameters.Add(P("@Id", idDetalle));
                        det.ExecuteNonQuery();

                        var ej = new SqlCommand("UPDATE Ejemplar SET Estado = 'Disponible' WHERE IdEjemplar = @Id", cn, tx);
                        ej.Parameters.Add(P("@Id", idEjemplar));
                        ej.ExecuteNonQuery();

                        var cerrar = new SqlCommand(@"
                            UPDATE Prestamo SET Estado = 'Devuelto'
                            WHERE IdPrestamo = @Id
                              AND NOT EXISTS (SELECT 1 FROM DetallePrestamo
                                              WHERE IdPrestamo = @Id AND FechaDevolucionReal IS NULL)", cn, tx);
                        cerrar.Parameters.Add(P("@Id", idPrestamo));
                        bool completo = cerrar.ExecuteNonQuery() > 0;

                        tx.Commit();
                        return completo;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void ActualizarAtrasados()
        {
            Ejecutar("UPDATE Prestamo SET Estado = 'Atrasado' WHERE Estado = 'Activo' AND FechaDevolucionPrevista < CAST(GETDATE() AS DATE)");
        }

        public DataTable ListarPrestamos()
        {
            return Consultar(@"
                SELECT p.IdPrestamo AS [N°], s.Apellidos + ', ' + s.Nombres AS Socio,
                       p.FechaPrestamo AS [Fecha préstamo], p.FechaDevolucionPrevista AS [Devolución prevista],
                       (SELECT COUNT(*) FROM DetallePrestamo d WHERE d.IdPrestamo = p.IdPrestamo) AS Ejemplares,
                       p.Estado, u.NombreUsuario AS [Atendido por], p.Observacion AS [Observación]
                FROM Prestamo p
                INNER JOIN Socio s   ON s.IdSocio = p.IdSocio
                INNER JOIN Usuario u ON u.IdUsuario = p.IdUsuario
                ORDER BY p.IdPrestamo DESC");
        }

        public DataTable ListarPendientes()
        {
            return Consultar(@"
                SELECT d.IdDetalle, p.IdPrestamo AS [N° préstamo], s.Apellidos + ', ' + s.Nombres AS Socio,
                       s.Documento, l.Titulo AS Libro, e.CodigoBarras AS [Código],
                       p.FechaPrestamo AS [Fecha préstamo], p.FechaDevolucionPrevista AS [Devolución prevista],
                       CASE WHEN p.FechaDevolucionPrevista < CAST(GETDATE() AS DATE)
                            THEN DATEDIFF(DAY, p.FechaDevolucionPrevista, CAST(GETDATE() AS DATE)) ELSE 0 END AS [Días de atraso],
                       p.Estado
                FROM DetallePrestamo d
                INNER JOIN Prestamo p ON p.IdPrestamo = d.IdPrestamo
                INNER JOIN Socio s    ON s.IdSocio = p.IdSocio
                INNER JOIN Ejemplar e ON e.IdEjemplar = d.IdEjemplar
                INNER JOIN Libro l    ON l.IdLibro = e.IdLibro
                WHERE d.FechaDevolucionReal IS NULL
                ORDER BY p.FechaDevolucionPrevista, p.IdPrestamo");
        }
    }
}
