# Genera el ZIP para entregar en Canvas: Entrega\SistemaBiblioteca_Entrega.zip
# Incluye la solución, el script de la base, la documentación y el README
# (sin las carpetas de compilación bin, obj ni .vs).
# Uso: clic derecho -> "Ejecutar con PowerShell", o desde una terminal: .\CrearEntrega.ps1

$raiz = $PSScriptRoot
$salida = Join-Path $raiz 'Entrega'
$zip = Join-Path $salida 'SistemaBiblioteca_Entrega.zip'
$temp = Join-Path ([IO.Path]::GetTempPath()) ('entrega_' + [guid]::NewGuid().ToString('N'))
$carpeta = Join-Path $temp 'SistemaBiblioteca'

New-Item -ItemType Directory -Force $salida, $carpeta | Out-Null

$incluir = @('SistemaBiblioteca.sln', 'README.md', 'BaseDatos', 'Documentacion', 'SistemaBiblioteca')
foreach ($item in $incluir) {
    $origen = Join-Path $raiz $item
    if (Test-Path $origen) { Copy-Item $origen -Destination $carpeta -Recurse -Force }
}

# Quitar archivos de compilación, de Visual Studio y temporales de Word (~$archivo.docx)
Get-ChildItem $carpeta -Recurse -Force -Directory |
    Where-Object { $_.Name -in @('bin', 'obj', '.vs') } |
    Remove-Item -Recurse -Force
Get-ChildItem $carpeta -Recurse -Force -File |
    Where-Object { $_.Name -like '~$*' -or $_.Extension -in @('.user', '.suo') } |
    Remove-Item -Force

if (Test-Path $zip) { Remove-Item $zip -Force }
Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem
# Se agregan las entradas una por una con "/" para que el ZIP se abra bien en cualquier sistema.
$archivo = [IO.Compression.ZipFile]::Open($zip, 'Create')
try {
    foreach ($f in Get-ChildItem $carpeta -Recurse -File) {
        $nombre = 'SistemaBiblioteca/' + $f.FullName.Substring($carpeta.Length + 1).Replace('\', '/')
        [void][IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archivo, $f.FullName, $nombre, 'Optimal')
    }
}
finally { $archivo.Dispose() }
Remove-Item $temp -Recurse -Force

$tam = [math]::Round((Get-Item $zip).Length / 1KB)
Write-Host "ZIP creado: $zip ($tam KB)"
