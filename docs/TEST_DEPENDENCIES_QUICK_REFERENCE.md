# Referencia Rapida: Tests Multi-Framework (`qckdev`)

## Checklist

1. No tocar `TargetFrameworks` de `qckdev`.
2. Mantener `qckdevTest` con split de dependencias (moderno/legacy).
3. Mantener `qckdevTest.Net35` y `qckdevTest.Net40` en la solucion.
4. Verificar salida `qckdev.2.dll` en `net35`.
5. Verificar tests en un TFM moderno y build de `net40`.

## Frameworks de `qckdevTest`

`net10.0;net9.0;net8.0;net7.0;net6.0;net5.0;netcoreapp3.1;net461;net451;net45`

## Proyectos legacy

- `qckdevTest.Net35`
- `qckdevTest.Net40`

## Comandos de verificacion

```powershell
dotnet build qckdev\qckdev.csproj -f net35
dotnet build qckdevTest\qckdevTest.csproj
dotnet build qckdevTest.Net40\qckdevTest.Net40.csproj
dotnet test  qckdevTest\qckdevTest.csproj -f net8.0
```

## Nota

`qckdevTest.Net35` requiere el targeting pack de `.NET Framework 3.5` en el host.
