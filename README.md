# Sistema de Gestión de Biblioteca

Aplicación de escritorio en **C# (Windows Forms, .NET Framework 4.8)** conectada a **SQL Server**.
Trabajo Práctico – 3er Parcial (Tema 3) – Ingeniería Informática.

Permite administrar el catálogo (categorías, editoriales, autores, libros y ejemplares), los socios
y los usuarios del sistema, y registrar los **préstamos** y **devoluciones**.

## Requisitos

- Visual Studio 2019, 2022 o 2026 con la carga de trabajo **Desarrollo de escritorio de .NET**.
- Un motor SQL Server, por ejemplo:
  - **SQL Server LocalDB** (recomendado): en el *Visual Studio Installer* → Modificar →
    Componentes individuales → marcar **SQL Server Express LocalDB**.
  - o **SQL Server Express** (gratuito) + **SQL Server Management Studio (SSMS)**.

## Instalación

1. **Crear la base de datos**: abrir `BaseDatos/BibliotecaDB.sql` y ejecutarlo completo
   (en SSMS, o en Visual Studio: *Ver → Explorador de objetos de SQL Server* → clic derecho en el
   servidor `(localdb)\MSSQLLocalDB` → *Nueva consulta* → pegar el script → Ejecutar).
   Desde consola también se puede ejecutar:
   ```
   sqlcmd -S "(localdb)\MSSQLLocalDB" -i BaseDatos\BibliotecaDB.sql -f 65001
   ```
2. **Cadena de conexión**: si no usa LocalDB, cambie `Data Source` en
   `SistemaBiblioteca/App.config` (por ejemplo `.\SQLEXPRESS`).
3. Abrir `SistemaBiblioteca.sln` en Visual Studio y presionar **F5**.

### Problema frecuente: "Cannot open database 'Biblioteca' requested by the login"

Ocurre si la instancia de LocalDB se recrea: los archivos `Biblioteca.mdf` y `Biblioteca_log.ldf`
siguen en la carpeta del usuario (`C:\Users\<usuario>\`) pero el servidor ya no los tiene registrados.
Se soluciona volviendo a adjuntarlos (ejecutar en una consulta sobre `master`):

```sql
CREATE DATABASE Biblioteca
ON (FILENAME = N'C:\Users\<usuario>\Biblioteca.mdf'),
   (FILENAME = N'C:\Users\<usuario>\Biblioteca_log.ldf')
FOR ATTACH;
```

Si se prefiere empezar de cero, borrar esos dos archivos y volver a ejecutar `BibliotecaDB.sql`.

## Usuarios de prueba

| Usuario         | Contraseña | Rol           |
|-----------------|------------|---------------|
| `admin`         | `admin123` | Administrador |
| `bibliotecario` | `biblio123`| Bibliotecario |

Las contraseñas se guardan cifradas con **SHA-256**.

## Estructura del proyecto (arquitectura por capas)

```
CrearEntrega.ps1            Genera Entrega/SistemaBiblioteca_Entrega.zip para subir a Canvas
Documentacion/
  Documentacion_Sistema_Biblioteca.docx   Documento del proyecto (UML, ER, diccionario, manual)
BaseDatos/
  BibliotecaDB.sql          Script de creación (11 tablas, 3FN) + datos de ejemplo
SistemaBiblioteca/
  Modelos/                  Clases del dominio (Rol, Usuario, Libro, Prestamo, ...)
  Datos/                    Acceso a datos con ADO.NET (consultas parametrizadas, transacciones)
    Conexion.cs             Conexión y utilidades SQL
    Seguridad.cs            Hash SHA-256 y sesión del usuario
    CatalogoDatos.cs        Categorías, editoriales y autores
    LibroDatos.cs           Libros (con autores) y ejemplares
    PersonasDatos.cs        Roles, usuarios y socios
    PrestamoDatos.cs        Préstamos, devoluciones y atrasos
  Formularios/              Interfaz Windows Forms
    FrmLogin                Inicio de sesión
    FrmPrincipal            Menú principal (MDI)
    FrmCategorias, FrmEditoriales, FrmAutores, FrmLibros,
    FrmEjemplares, FrmSocios, FrmUsuarios      Mantenimientos (ABM)
    FrmPrestamo, FrmDevolucion                 Movimientos
```

## Funcionalidades

- Inicio de sesión con roles; el menú **Usuarios** solo lo ve el Administrador.
- ABM con búsqueda en todas las entidades. Validación de campos obligatorios y mensajes claros
  ante datos duplicados o registros relacionados que no se pueden eliminar.
- **Préstamo**: se elige un socio activo y uno o varios ejemplares disponibles; se registra la
  cabecera y el detalle en una transacción y los ejemplares pasan a *Prestado*.
- **Devolución**: lista los ejemplares pendientes (en rojo los atrasados); al devolver se registra
  la fecha real, el ejemplar vuelve a *Disponible* y, si no quedan pendientes, el préstamo pasa a
  *Devuelto*.
- Los préstamos vencidos se marcan automáticamente como *Atrasado*.
