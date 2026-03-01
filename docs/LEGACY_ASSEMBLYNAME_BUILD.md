# Build de ensamblado legacy (`net35`) con sufijo `.2`

## Objetivo

Mantener compatibilidad binaria legacy en `qckdev` usando:

- `net35` -> `qckdev.2.dll`
- resto de frameworks -> `qckdev.dll`

## Configuracion aplicada

En `qckdev/qckdev.csproj`:

- `PropertyGroup` condicional para `net35`:
  - `<AssemblyName>$(MSBuildProjectName).2</AssemblyName>`
- `PropertyGroup` para el resto:
  - `<AssemblyName>$(MSBuildProjectName)</AssemblyName>`

## Verificacion

```powershell
dotnet build qckdev\qckdev.csproj -c Debug -f net35
```

Salida esperada:

- `qckdev\bin\Debug\net35\qckdev.2.dll`

## Nota de entorno

Si el host no tiene el Developer Pack/Targeting Pack de `.NET Framework 3.5`, el build de proyectos `net35` fallara con `MSB3644`.
