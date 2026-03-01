# Actualizacion de dependencias de tests

## Proyecto

`qckdev`

## Topologia de tests

- `qckdevTest` (runner principal multi-framework)
- `qckdevTest.Net35` (runner legacy separado)
- `qckdevTest.Net40` (runner legacy separado)

## Estado actual (fuente de verdad)

### 1) `qckdevTest`

#### Target frameworks

`net10.0;net9.0;net8.0;net7.0;net6.0;net5.0;netcoreapp3.1;net461;net451;net45`

#### Paquetes de testing modernos (`netcoreapp3.1+`)

- `Microsoft.NET.Test.Sdk` `17.11.1`
- `MSTest.TestAdapter` `3.2.2`
- `MSTest.TestFramework` `3.2.2`
- `coverlet.msbuild` `6.0.0`
- `coverlet.collector` `6.0.0`

#### Paquetes de testing legacy (`net461/net451/net45`)

- `Microsoft.NET.Test.Sdk` `17.11.0`
- `MSTest.TestAdapter` `2.2.10`
- `MSTest.TestFramework` `2.2.10`
- `coverlet.msbuild` `3.1.2`
- `coverlet.collector` `1.2.0`
- `Reference Include="Microsoft.CSharp"`

#### Dependencias funcionales por framework

- `System.Text.RegularExpressions` + `System.Net.Http`: todos salvo `net45/net40/net35`.
- `System.Security.Principal.Windows`: `netcoreapp3.1`, `net5.0`.
- EF Core:
  - `3.1.32`: `netcoreapp3.1`, `net5.0`, `net461`
  - `6.0.36`: `net6.0`
  - `7.0.20`: `net7.0`
  - `8.0.11`: `net8.0`
  - `9.0.0`: `net9.0`
  - `10.0.0`: `net10.0`

#### Exclusiones condicionales de codigo de test

- `net45`/`net451`: se excluyen tests de EF (`QueryableTest`, `TestDbContext`, entidades).
- `net5.0`: se excluye `Reflection\ExtensionTest.cs`.

### 2) `qckdevTest.Net35`

- Proyecto MSTest clasico (`ToolsVersion` no SDK style)
- Tests incluidos (link desde `qckdevTest`):
  - `CommandArgsDictionaryTest.cs`
  - `KeyTest.cs`
- Dependencia MSTest clasico via `packages.config`:
  - `VS.QualityTools.UnitTestFramework` `15.0.27323.2`

### 3) `qckdevTest.Net40`

- Proyecto MSTest clasico (`ToolsVersion` no SDK style)
- Tests incluidos (link desde `qckdevTest`):
  - `CommandArgsDictionaryTest.cs`
  - `KeyTest.cs`
- Dependencia MSTest clasico via `packages.config`:
  - `VS.QualityTools.UnitTestFramework` `15.0.27323.2`

## Reglas de mantenimiento

1. No cambiar `TargetFrameworks` de la libreria (`qckdev`) sin solicitud explicita.
2. Mantener separadas las dependencias de test modernas y legacy.
3. Conservar `qckdevTest.Net35` y `qckdevTest.Net40` en la solucion.
4. Si se anade un nuevo framework en `qckdevTest`, validar si requiere bloque propio de dependencias.
5. En conflictos con BCL nueva (ej. `.NET 10`), preferir llamada explicita a APIs de `qckdev`.
