/* =====================================================================
   SISTEMA DE GESTIÓN DE BIBLIOTECA
   Script de creación de la base de datos "Biblioteca" (SQL Server)
   - 11 tablas normalizadas a 3FN con PK, FK, UNIQUE y CHECK
   - Datos de ejemplo (usuarios admin / bibliotecario)
   Ejecutar completo en SQL Server Management Studio, en el Explorador
   de objetos de SQL Server de Visual Studio o con sqlcmd.
   ===================================================================== */

IF DB_ID('Biblioteca') IS NULL
    CREATE DATABASE Biblioteca;
GO

USE Biblioteca;
GO

/* ---------------------------------------------------------------------
   Eliminación de tablas (orden inverso a las dependencias) para poder
   volver a ejecutar el script desde cero.
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.DetallePrestamo', 'U') IS NOT NULL DROP TABLE dbo.DetallePrestamo;
IF OBJECT_ID('dbo.Prestamo', 'U')        IS NOT NULL DROP TABLE dbo.Prestamo;
IF OBJECT_ID('dbo.Socio', 'U')           IS NOT NULL DROP TABLE dbo.Socio;
IF OBJECT_ID('dbo.Ejemplar', 'U')        IS NOT NULL DROP TABLE dbo.Ejemplar;
IF OBJECT_ID('dbo.LibroAutor', 'U')      IS NOT NULL DROP TABLE dbo.LibroAutor;
IF OBJECT_ID('dbo.Libro', 'U')           IS NOT NULL DROP TABLE dbo.Libro;
IF OBJECT_ID('dbo.Autor', 'U')           IS NOT NULL DROP TABLE dbo.Autor;
IF OBJECT_ID('dbo.Editorial', 'U')       IS NOT NULL DROP TABLE dbo.Editorial;
IF OBJECT_ID('dbo.Categoria', 'U')       IS NOT NULL DROP TABLE dbo.Categoria;
IF OBJECT_ID('dbo.Usuario', 'U')         IS NOT NULL DROP TABLE dbo.Usuario;
IF OBJECT_ID('dbo.Rol', 'U')             IS NOT NULL DROP TABLE dbo.Rol;
GO

/* ------------------------------ TABLAS ------------------------------ */

CREATE TABLE dbo.Rol (
    IdRol        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Rol PRIMARY KEY,
    Nombre       VARCHAR(50)  NOT NULL CONSTRAINT UQ_Rol_Nombre UNIQUE,
    Descripcion  VARCHAR(150) NULL
);

CREATE TABLE dbo.Usuario (
    IdUsuario      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuario PRIMARY KEY,
    NombreUsuario  VARCHAR(50)  NOT NULL CONSTRAINT UQ_Usuario_NombreUsuario UNIQUE,
    Clave          VARCHAR(100) NOT NULL,               -- hash SHA-256 (hexadecimal)
    NombreCompleto VARCHAR(100) NOT NULL,
    IdRol          INT          NOT NULL CONSTRAINT FK_Usuario_Rol REFERENCES dbo.Rol(IdRol),
    Activo         BIT          NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT (1),
    FechaCreacion  DATETIME     NOT NULL CONSTRAINT DF_Usuario_FechaCreacion DEFAULT (GETDATE())
);

CREATE TABLE dbo.Categoria (
    IdCategoria  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categoria PRIMARY KEY,
    Nombre       VARCHAR(60)  NOT NULL CONSTRAINT UQ_Categoria_Nombre UNIQUE,
    Descripcion  VARCHAR(150) NULL
);

CREATE TABLE dbo.Editorial (
    IdEditorial  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Editorial PRIMARY KEY,
    Nombre       VARCHAR(80)  NOT NULL CONSTRAINT UQ_Editorial_Nombre UNIQUE,
    Telefono     VARCHAR(30)  NULL,
    Email        VARCHAR(80)  NULL,
    Direccion    VARCHAR(150) NULL
);

CREATE TABLE dbo.Autor (
    IdAutor      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Autor PRIMARY KEY,
    Nombres      VARCHAR(60) NOT NULL,
    Apellidos    VARCHAR(60) NOT NULL,
    Nacionalidad VARCHAR(50) NULL
);

CREATE TABLE dbo.Libro (
    IdLibro      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Libro PRIMARY KEY,
    Titulo       VARCHAR(150) NOT NULL,
    ISBN         VARCHAR(20)  NULL,
    Anio         INT          NULL,
    IdCategoria  INT          NOT NULL CONSTRAINT FK_Libro_Categoria REFERENCES dbo.Categoria(IdCategoria),
    IdEditorial  INT          NOT NULL CONSTRAINT FK_Libro_Editorial REFERENCES dbo.Editorial(IdEditorial)
);
-- ISBN único pero opcional: el índice filtrado permite varios NULL.
CREATE UNIQUE INDEX UX_Libro_ISBN ON dbo.Libro(ISBN) WHERE ISBN IS NOT NULL;

-- Tabla intermedia que resuelve la relación N:M entre Libro y Autor.
CREATE TABLE dbo.LibroAutor (
    IdLibro INT NOT NULL CONSTRAINT FK_LibroAutor_Libro REFERENCES dbo.Libro(IdLibro) ON DELETE CASCADE,
    IdAutor INT NOT NULL CONSTRAINT FK_LibroAutor_Autor REFERENCES dbo.Autor(IdAutor),
    CONSTRAINT PK_LibroAutor PRIMARY KEY (IdLibro, IdAutor)
);

CREATE TABLE dbo.Ejemplar (
    IdEjemplar   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Ejemplar PRIMARY KEY,
    IdLibro      INT         NOT NULL CONSTRAINT FK_Ejemplar_Libro REFERENCES dbo.Libro(IdLibro),
    CodigoBarras VARCHAR(30) NOT NULL CONSTRAINT UQ_Ejemplar_CodigoBarras UNIQUE,
    Ubicacion    VARCHAR(50) NULL,
    Estado       VARCHAR(15) NOT NULL CONSTRAINT DF_Ejemplar_Estado DEFAULT ('Disponible')
                 CONSTRAINT CK_Ejemplar_Estado CHECK (Estado IN ('Disponible', 'Prestado', 'Baja'))
);

CREATE TABLE dbo.Socio (
    IdSocio       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Socio PRIMARY KEY,
    Nombres       VARCHAR(60)  NOT NULL,
    Apellidos     VARCHAR(60)  NOT NULL,
    Documento     VARCHAR(20)  NOT NULL CONSTRAINT UQ_Socio_Documento UNIQUE,
    Telefono      VARCHAR(30)  NULL,
    Email         VARCHAR(80)  NULL,
    Direccion     VARCHAR(150) NULL,
    FechaRegistro DATETIME     NOT NULL CONSTRAINT DF_Socio_FechaRegistro DEFAULT (GETDATE()),
    Activo        BIT          NOT NULL CONSTRAINT DF_Socio_Activo DEFAULT (1)
);

-- Cabecera del movimiento
CREATE TABLE dbo.Prestamo (
    IdPrestamo              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Prestamo PRIMARY KEY,
    IdSocio                 INT          NOT NULL CONSTRAINT FK_Prestamo_Socio   REFERENCES dbo.Socio(IdSocio),
    IdUsuario               INT          NOT NULL CONSTRAINT FK_Prestamo_Usuario REFERENCES dbo.Usuario(IdUsuario),
    FechaPrestamo           DATETIME     NOT NULL CONSTRAINT DF_Prestamo_FechaPrestamo DEFAULT (GETDATE()),
    FechaDevolucionPrevista DATETIME     NOT NULL,
    Estado                  VARCHAR(15)  NOT NULL CONSTRAINT DF_Prestamo_Estado DEFAULT ('Activo')
                            CONSTRAINT CK_Prestamo_Estado CHECK (Estado IN ('Activo', 'Devuelto', 'Atrasado')),
    Observacion             VARCHAR(200) NULL,
    CONSTRAINT CK_Prestamo_Fechas CHECK (FechaDevolucionPrevista >= CAST(FechaPrestamo AS DATE))
);

-- Detalle del movimiento
CREATE TABLE dbo.DetallePrestamo (
    IdDetalle           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DetallePrestamo PRIMARY KEY,
    IdPrestamo          INT      NOT NULL CONSTRAINT FK_Detalle_Prestamo REFERENCES dbo.Prestamo(IdPrestamo),
    IdEjemplar          INT      NOT NULL CONSTRAINT FK_Detalle_Ejemplar REFERENCES dbo.Ejemplar(IdEjemplar),
    FechaDevolucionReal DATETIME NULL
);
GO

/* -------------------------- DATOS DE EJEMPLO ------------------------- */

INSERT INTO dbo.Rol (Nombre, Descripcion) VALUES
('Administrador', 'Acceso total, incluida la gestión de usuarios'),
('Bibliotecario', 'Gestiona el catálogo, los socios y los préstamos');

-- Las contraseñas se guardan como hash SHA-256 en hexadecimal (igual que en C#).
-- admin / admin123     bibliotecario / biblio123
INSERT INTO dbo.Usuario (NombreUsuario, Clave, NombreCompleto, IdRol) VALUES
('admin',         CONVERT(VARCHAR(100), HASHBYTES('SHA2_256', 'admin123'), 2),  'Administrador del Sistema', 1),
('bibliotecario', CONVERT(VARCHAR(100), HASHBYTES('SHA2_256', 'biblio123'), 2), 'Bibliotecario General',     2);

INSERT INTO dbo.Categoria (Nombre, Descripcion) VALUES
('Novela',       'Narrativa de ficción'),
('Ciencia',      'Divulgación científica'),
('Historia',     'Libros de historia'),
('Informática',  'Programación y tecnología'),
('Infantil',     'Literatura para niños');

INSERT INTO dbo.Editorial (Nombre, Telefono, Email, Direccion) VALUES
('Editorial Sudamericana', '011-4555-0001', 'contacto@sudamericana.com', 'Buenos Aires'),
('Alfaguara',              '011-4555-0002', 'info@alfaguara.com',        'Madrid'),
('Anaya Multimedia',       '011-4555-0003', 'ventas@anaya.es',           'Madrid'),
('Planeta',                '011-4555-0004', 'planeta@planeta.com',       'Barcelona');

INSERT INTO dbo.Autor (Nombres, Apellidos, Nacionalidad) VALUES
('Gabriel',  'García Márquez', 'Colombiana'),
('Jorge Luis','Borges',        'Argentina'),
('Isabel',   'Allende',        'Chilena'),
('Stephen',  'Hawking',        'Británica'),
('Robert C.','Martin',         'Estadounidense'),
('Antoine',  'de Saint-Exupéry','Francesa');

INSERT INTO dbo.Libro (Titulo, ISBN, Anio, IdCategoria, IdEditorial) VALUES
('Cien años de soledad',            '9780307474728', 1967, 1, 1),
('Ficciones',                       '9788499089515', 1944, 1, 2),
('La casa de los espíritus',        '9788401352836', 1982, 1, 4),
('Breve historia del tiempo',       '9788484328995', 1988, 2, 4),
('Código limpio',                   '9788441532106', 2008, 4, 3),
('El principito',                   '9788498381498', 1943, 5, 2);

INSERT INTO dbo.LibroAutor (IdLibro, IdAutor) VALUES
(1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6);

INSERT INTO dbo.Ejemplar (IdLibro, CodigoBarras, Ubicacion, Estado) VALUES
(1, 'EJ-0001', 'Estante A1', 'Disponible'),
(1, 'EJ-0002', 'Estante A1', 'Disponible'),
(2, 'EJ-0003', 'Estante A2', 'Disponible'),
(3, 'EJ-0004', 'Estante A3', 'Disponible'),
(4, 'EJ-0005', 'Estante B1', 'Disponible'),
(5, 'EJ-0006', 'Estante C1', 'Disponible'),
(5, 'EJ-0007', 'Estante C1', 'Disponible'),
(6, 'EJ-0008', 'Estante D1', 'Disponible');

INSERT INTO dbo.Socio (Nombres, Apellidos, Documento, Telefono, Email, Direccion) VALUES
('Juan',   'Pérez',    '30111222', '0981-111111', 'juan.perez@mail.com',  'Calle 1 N° 100'),
('María',  'González', '31222333', '0981-222222', 'maria.gonz@mail.com',  'Calle 2 N° 200'),
('Carlos', 'López',    '32333444', '0981-333333', 'carlos.lopez@mail.com','Calle 3 N° 300');
GO

PRINT 'Base de datos Biblioteca creada correctamente.';
GO
