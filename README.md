# Full Internet Services — Sistema de Monitoreo de Soporte Técnico y Ventas

> Solución cliente-servidor para gestión integral de soporte técnico y venta de servicios de internet.
> Arquitectura limpia .NET 9 + WinForms + SQL Server + Azure.

---

## Contenido del Repositorio

```
.
├── docs/                    ← Documentación completa (diagramas, decisiones, guías)
├── src/
│   ├── FIS.Api/             ← ASP.NET Core 8 Web API (REST + JWT + RBAC)
│   ├── FIS.Application/     ← Casos de uso, CQRS con MediatR, validadores
│   ├── FIS.Contracts/       ← DTOs compartidos entre API y clientes
│   ├── FIS.Domain/          ← Entidades, enums, servicios de dominio (núcleo)
│   ├── FIS.Desktop/         ← Cliente WinForms — Panel administrativo
│   └── FIS.Infrastructure/  ← EF Core, repositorios, JWT, BCrypt, Azure adapters
├── tests/
│   ├── FIS.Domain.Tests/         ← Tests unitarios del dominio
│   ├── FIS.Application.Tests/    ← Tests de casos de uso
│   └── FIS.Api.IntegrationTests/ ← Tests de integración HTTP
├── db/
│   └── db.sql               ← DDL original con tablas, índices, SPs y triggers
├── scripts/                 ← Utilidades (CI, despliegue local, etc.)
├── global.json              ← Pin de SDK .NET 9.0.306
└── FullInternetServices.sln
```

---

## Cómo arrancar (Quickstart)

### 1. Prerrequisitos

| Herramienta | Versión |
|---|---|
| .NET SDK | 9.0.x |
| SQL Server | 2019+ (Developer / Express / Azure SQL) |
| Visual Studio 2022 / Rider | 2022.10+ / 2024.3+ |
| dotnet-ef CLI | `dotnet tool install --global dotnet-ef` |

### 2. Configurar la cadena de conexión

Edita `src/FIS.Api/appsettings.json` con tu instancia local de SQL Server:

```json
"ConnectionStrings": {
  "FisDatabase": "Server=localhost;Database=FullInternetServices;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Aplicar la migración inicial

```powershell
dotnet ef database update --project src/FIS.Infrastructure --startup-project src/FIS.Api
```

> Alternativa: ejecutar el script `db/db.sql` directamente en SSMS para tener también los SPs y triggers documentados en el PDF.

### 4. Ejecutar la API

```powershell
dotnet run --project src/FIS.Api
```

La API arranca en `https://localhost:7001`. Abre el Swagger UI en `https://localhost:7001/swagger`.

Al arrancar en `Development`, el seeder crea automáticamente:
- Roles `Administrador`, `Cajero`, `Tecnico`, `Cliente`
- Usuario admin: **username** `admin` / **password** `Admin123*`

### 5. Ejecutar el Cliente Desktop

```powershell
dotnet run --project src/FIS.Desktop
```

Inicia sesión con `admin / Admin123*`.

### 6. Ejecutar los tests

```powershell
dotnet test
```

---

## Documentación

Toda la documentación arquitectónica está en `/docs`. Empieza por el [README principal de docs](./docs/README.md).

| Sección | Descripción |
|---|---|
| [01 — Arquitectura](./docs/01-arquitectura/README.md) | Diagramas de alto nivel, capas, componentes y despliegue |
| [02 — Backend](./docs/02-backend/README.md) | Estructura del API, endpoints, RBAC, EF Core |
| [03 — Base de Datos](./docs/03-base-datos/README.md) | Modelo conceptual y lógico, índices, SPs, triggers |
| [04 — Aplicación Desktop](./docs/04-desktop/README.md) | Arquitectura WinForms, módulos, consumo de la API |
| [05 — Cloud Azure](./docs/05-cloud-azure/README.md) | Servicios, CI/CD, ambientes Dev/QA/Prod |
| [06 — Requerimientos](./docs/06-requerimientos/README.md) | Funcionales, no funcionales, matriz de cumplimiento |
| [07 — Mejoras](./docs/07-mejoras/README.md) | Monolito vs microservicios, monitoreo, versionado, caching |

---

## Tecnologías y Decisiones Clave

- **.NET 9** (LTS — la versión 8 también es válida; el `global.json` ancla el SDK).
- **Clean Architecture**: 4 capas con dependencias unidireccionales hacia el dominio.
- **CQRS + MediatR**: separación de comandos y consultas con pipeline de validación.
- **EF Core 9** + SQL Server, mapeando exactamente al DDL del archivo `db.sql`.
- **JWT Bearer** para autenticación, **claims de rol** para RBAC en endpoints.
- **BCrypt** para hash de contraseñas (work factor 12).
- **WinForms con DI** vía `Microsoft.Extensions.DependencyInjection` y **Refit** para consumo tipado de la API.
- **Azure** como objetivo de despliegue (App Service, Azure SQL, Blob Storage, Key Vault, AAD B2C).

---

## Contexto del Proyecto

El sistema cubre las 22 historias de usuario y los 18 RF / 14 RNF descritos en *Proyecto FINAL.pdf* del equipo de Full Internet Services (UPSCZ — Ingeniería de Software, 2026), implementando el modelo físico de 8 tablas de `db/db.sql`.

**Equipo del proyecto académico**: Adrian Gareca, Paul Carrizales, Denis Nogales.

---

## Estado del Build

| Componente | Estado |
|---|---|
| FIS.Domain | ✓ Build OK + 9 tests pasando |
| FIS.Application | ✓ Build OK |
| FIS.Infrastructure | ✓ Build OK + Migración inicial generada |
| FIS.Api | ✓ Build OK + Swagger disponible |
| FIS.Desktop | ✓ Build OK + Login + Dashboard funcional |

Última verificación: solución completa compila con **0 warnings, 0 errors**.
