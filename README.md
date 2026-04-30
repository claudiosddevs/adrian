# Full Internet Services — Sistema de Monitoreo de Soporte Técnico y Ventas

> Solución cliente-servidor para gestión integral de soporte técnico y venta de servicios de internet.  
> Arquitectura limpia .NET 9 + WinForms + SQL Server + Azure.

---

## Contenido del Repositorio

```
.
├── docs/                    ← Documentación completa (diagramas, decisiones, guías)
├── src/
│   ├── FIS.Api/             ← ASP.NET Core 9 Web API (REST + JWT + RBAC)
│   ├── FIS.Application/     ← Casos de uso, CQRS con MediatR, validadores
│   ├── FIS.Contracts/       ← DTOs compartidos entre API y clientes
│   ├── FIS.Domain/          ← Entidades, enums, servicios de dominio (núcleo)
│   ├── FIS.Desktop/         ← Cliente WinForms — Panel administrativo completo
│   └── FIS.Infrastructure/  ← EF Core, repositorios, JWT, BCrypt, Azure adapters
├── tests/
│   ├── FIS.Domain.Tests/         ← Tests unitarios del dominio
│   ├── FIS.Application.Tests/    ← Tests de casos de uso
│   └── FIS.Api.IntegrationTests/ ← Tests de integración HTTP
├── db/
│   └── db.sql               ← DDL original con tablas, índices, SPs y triggers
├── scripts/                 ← Utilidades (CI, despliegue local, render-diagrams.ps1)
├── global.json              ← Pin de SDK .NET 9.0.306
└── FullInternetServices.sln
```

---

## Cobertura de Requerimientos Funcionales

| RF | Funcionalidad | API | Desktop | Estado |
|---|---|---|---|---|
| RF01 | Autenticación (login + biometría) | `AuthController` | `frmLogin` | ✓ JWT — biometría roadmap |
| RF02 | Gestión de clientes CRUD | `ClientesController` | `frmClientes` + `frmClienteDetalle` | ✓ Completo |
| RF03 | Gestión de servicios | `PlanesController` | `frmPlanes` + `frmPlanDetalle` | ✓ Completo |
| RF04 | Gestión de planes de internet | `PlanesController` | `frmPlanes` | ✓ Completo |
| RF05 | Registro de contratos | `ContratosController` | `frmContratos` + `frmContratoNuevo` | ✓ Completo |
| RF06 | Registro de pagos | `PagosController` | `frmPagos` + `frmRegistrarPago` | ✓ Completo |
| RF07 | Anulación de pagos | `POST /pagos/anular` | `frmPagos` (admin) | ✓ Completo |
| RF08 | Control de mora (día 12) | `GET /reportes/mora` | `frmReportes` | ✓ Completo |
| RF09 | Registro de reclamos telefónicos | `ReclamosController` | `frmReclamos` + `frmRegistrarReclamo` | ✓ Completo |
| RF10 | Clasificación de reclamos | `PATCH /reclamos/{id}/estado` | `frmRegistrarReclamo` | ✓ Leve/Medio/Complejo |
| RF11 | Asignación de técnicos | `PATCH /reclamos/{id}/tecnico` | `frmReclamos` | ✓ Con límite 5 activos |
| RF12 | Estado de soporte | `PATCH /reclamos/{id}/estado` | `frmCambiarEstadoReclamo` | ✓ 4 estados |
| RF13 | Grabación de llamadas | Modelo (`RutaAudio`) | — | Modelo listo, storage roadmap |
| RF14 | Evaluación de técnicos | `GET /reportes/tecnicos` | `frmReportes` tab Técnicos | ✓ % resolución + calificación |
| RF15 | Generación de reportes | `ReportesController` | `frmReportes` (3 tabs) | ✓ Mora / Ventas / Técnicos |
| RF16 | Gestión de usuarios y roles | `UsuariosController` | `frmUsuarios` + `frmUsuarioNuevo` | ✓ Completo |
| RF17 | Bitácora de operaciones | `GET /reportes/bitacora` | — | ✓ Entidad + migración + endpoint |
| RF18 | Plataforma web de pagos | — | — | Roadmap (Fase 2) |

---

## Cómo Arrancar (Quickstart)

### 1. Prerrequisitos

| Herramienta | Versión |
|---|---|
| .NET SDK | 9.0.x (`global.json` lo ancla) |
| SQL Server | 2019+ (Developer / Express / Azure SQL) |
| Visual Studio 2022 / Rider | 2022.10+ / 2024.3+ |
| dotnet-ef CLI | `dotnet tool install --global dotnet-ef` |

### 2. Configurar la cadena de conexión

Edita `src/FIS.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "FisDatabase": "Server=localhost;Database=FullInternetServices;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Aplicar las migraciones

```powershell
dotnet ef database update --project src/FIS.Infrastructure --startup-project src/FIS.Api
```

> Hay **2 migraciones**: `InitialCreate` (8 tablas del `db.sql`) + `AddBitacora` (tabla RF17).

### 4. Ejecutar la API

```powershell
dotnet run --project src/FIS.Api
```

La API arranca en `https://localhost:7001`. Abre el Swagger UI en `https://localhost:7001/swagger`.

Al arrancar en modo `Development`, el seeder inserta automáticamente:

| Entidad | Cantidad |
|---|---|
| Roles | 4 (Administrador, Cajero, Tecnico, Cliente) |
| Usuarios | 6 (admin, cajero1, cajero2, tecnico1, tecnico2, tecnico3) |
| Clientes | 12 (naturales y jurídicos) |
| Planes de servicio | 8 (internet, fibra, hosting, dominio) |
| Contratos | 12 |
| Pagos | 20+ (incluye pago anulado) |
| Reclamos | 8 (con estados y calificaciones variadas) |

### 5. Credenciales de demostración

| Usuario | Contraseña | Rol |
|---|---|---|
| `admin` | `Admin123*` | Administrador |
| `cajero1` | `Cajero123*` | Cajero |
| `cajero2` | `Cajero123*` | Cajero |
| `tecnico1` | `Tecnico123*` | Técnico |
| `tecnico2` | `Tecnico123*` | Técnico |
| `tecnico3` | `Tecnico123*` | Técnico |

### 6. Ejecutar el Cliente Desktop

```powershell
dotnet run --project src/FIS.Desktop
```

Inicia sesión con cualquier credencial de la tabla anterior. El menú lateral muestra los módulos habilitados según el rol del usuario autenticado.

### 7. Ejecutar los tests

```powershell
dotnet test
```

---

## Módulos del Panel Administrativo (WinForms)

| Formulario | Rol de acceso | Funcionalidad |
|---|---|---|
| `frmLogin` | Todos | Autenticación JWT |
| `frmDashboard` | Todos | Menú lateral con todos los módulos |
| `frmClientes` + `frmClienteDetalle` | Admin, Cajero | CRUD completo de clientes (RF02) |
| `frmPlanes` + `frmPlanDetalle` | Admin | Gestión de planes y servicios (RF03, RF04) |
| `frmContratos` + `frmContratoNuevo` | Admin | Registro y gestión de contratos (RF05) |
| `frmPagos` + `frmRegistrarPago` | Admin, Cajero | Pagos y anulaciones (RF06, RF07) |
| `frmReclamos` + `frmRegistrarReclamo` + `frmCambiarEstadoReclamo` | Admin, Técnico | Soporte técnico (RF09-RF12) |
| `frmUsuarios` + `frmUsuarioNuevo` | Admin | Usuarios y roles (RF16) |
| `frmReportes` (3 tabs) | Admin | Mora, ventas y técnicos (RF14, RF15) |

---

## Documentación

Toda la documentación arquitectónica está en `/docs`. Empieza por el [README principal de docs](./docs/README.md).

| Sección | Descripción |
|---|---|
| [01 — Arquitectura](./docs/01-arquitectura/README.md) | Diagramas de alto nivel, capas, componentes y despliegue |
| [02 — Backend](./docs/02-backend/README.md) | Endpoints REST completos, RBAC, EF Core, CQRS con MediatR |
| [03 — Base de Datos](./docs/03-base-datos/README.md) | Modelo conceptual y lógico, índices, SPs, triggers, BITACORA |
| [04 — Aplicación Desktop](./docs/04-desktop/README.md) | Arquitectura WinForms, 12 formularios, consumo de la API |
| [05 — Cloud Azure](./docs/05-cloud-azure/README.md) | Servicios, CI/CD, ambientes Dev/QA/Prod |
| [06 — Requerimientos](./docs/06-requerimientos/README.md) | RF01-RF18 + RNF01-RNF14 + matriz de cumplimiento actualizada |
| [07 — Mejoras](./docs/07-mejoras/README.md) | Monolito vs microservicios, monitoreo, versionado, caching |

---

## Tecnologías y Decisiones Clave

- **.NET 9** (LTS — el `global.json` ancla el SDK 9.0.306).
- **Clean Architecture**: 4 capas con dependencias unidireccionales hacia el dominio.
- **CQRS + MediatR**: 25 handlers (Commands + Queries) con pipeline de validación FluentValidation.
- **EF Core 9** + SQL Server, mapeando al DDL de `db.sql` con Fluent API.
- **JWT Bearer** para autenticación, **claims de rol** para RBAC en todos los endpoints.
- **BCrypt** para hash de contraseñas (work factor 12).
- **WinForms con DI** vía `Microsoft.Extensions.DependencyInjection` y **Refit** para consumo tipado.
- **Azure** como objetivo de despliegue (App Service, Azure SQL, Blob Storage, Key Vault, AAD B2C).

---

## Contexto del Proyecto

El sistema implementa los 18 RF y 14 RNF descritos en *Proyecto FINAL.pdf* del equipo de Full Internet Services (UPSCZ — Ingeniería de Software, 2026), usando el modelo físico de 8 tablas de `db/db.sql` más la tabla `BITACORA` para RF17.

**Equipo del proyecto académico**: Adrian Gareca, Paul Carrizales, Denis Nogales.

---

## Estado del Build

| Componente | Estado |
|---|---|
| FIS.Domain | ✓ Build OK + 9 tests unitarios pasando |
| FIS.Application | ✓ Build OK — 25 handlers (15 Commands + 10 Queries) |
| FIS.Infrastructure | ✓ Build OK — 2 migraciones + 6 repositorios + ReporteService |
| FIS.Api | ✓ Build OK — 8 controllers + Swagger completo |
| FIS.Desktop | ✓ Build OK — 12 formularios + Dashboard con menú lateral completo |

Última verificación: solución completa compila con **0 warnings, 0 errors** · 77 archivos · +4 580 líneas.
