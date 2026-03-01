# Framework Compatibility

Este documento resume la compatibilidad actual de `qckdev` tras la adaptacion a la topologia usada en otros proyectos del repositorio.

## Libreria principal (`qckdev/qckdev.csproj`)

- Se mantienen los `TargetFrameworks` existentes (sin cambios de alcance funcional):
  - `netstandard2.0`
  - `netstandard1.2`
  - `net461`
  - `net40`
  - `net35`
- Regla de nombre de ensamblado:
  - `net35` genera `qckdev.2.dll`
  - resto de frameworks generan `qckdev.dll`

## Tests

### Runner principal (`qckdevTest/qckdevTest.csproj`)

Frameworks configurados:

- `net10.0`
- `net9.0`
- `net8.0`
- `net7.0`
- `net6.0`
- `net5.0`
- `netcoreapp3.1`
- `net461`
- `net451`
- `net45`

### Runners legacy separados

- `qckdevTest.Net35` (`.NET Framework 3.5`)
- `qckdevTest.Net40` (`.NET Framework 4.0`)

## Notas de compatibilidad relevantes

1. En .NET 10 existe `System.Linq.Enumerable.LeftJoin`; para evitar ambiguedad con `qckdev.Linq.Enumerable.LeftJoin` se forzo llamada explicita en tests.
2. En `net45`/`net451` se excluyen tests EF (`QueryableTest` + `TestDbContext` + entidades) por incompatibilidad de stack.
3. En `net5.0` se excluye `Reflection/ExtensionTest.cs` por resolucion de tipos `IdentityReference` en este setup.
4. `qckdevTest.Net35` requiere el targeting pack de .NET 3.5 instalado en el host para compilar.

## Validacion recomendada

```powershell
dotnet build qckdev\qckdev.csproj -f net35
dotnet build qckdevTest\qckdevTest.csproj
dotnet build qckdevTest.Net40\qckdevTest.Net40.csproj
dotnet test  qckdevTest\qckdevTest.csproj -f net8.0
```
