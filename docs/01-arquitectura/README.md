# 01 — Arquitectura

Esta sección documenta la arquitectura completa de la solución desde cuatro vistas complementarias: alto nivel, capas, componentes y despliegue.

---

## 1.1 Vista de Alto Nivel (Cliente-Servidor)

El sistema sigue el patrón **Cliente-Servidor** clásico, con la API REST como **único punto de acceso a la base de datos** (sección 3.5.8 del PDF: "intermediario obligatorio").

<img src="../diagrams/01-arquitectura-alto-nivel.png" alt="Arquitectura de alto nivel" width="900" />

<details>
<summary>Ver fuente Mermaid</summary>

```mermaid
flowchart TB
    subgraph Clientes["Clientes"]
        DESKTOP["Aplicación Desktop<br/>(WinForms .NET 9)<br/>Panel Administrativo"]
        WEB["Aplicación Web<br/>(ASP.NET Core MVC / Blazor)<br/>Portal Clientes / Pagos"]
        MOBILE["App Móvil<br/>(opcional - fase 2)"]
    end

    subgraph Edge["Borde / Seguridad"]
        APIM["Azure API Management<br/>Rate Limiting · Versionado · OpenAPI"]
        FRONTDOOR["Azure Front Door<br/>WAF · TLS · CDN"]
    end

    subgraph Backend["Backend - Azure App Service"]
        API["ASP.NET Core 9 Web API<br/>RESTful + JWT + RBAC"]
    end

    subgraph Servicios["Servicios Transversales"]
        AAD["Azure AD B2C<br/>Identidad de Clientes"]
        KV["Azure Key Vault<br/>Secretos y Conn Strings"]
        REDIS["Azure Cache for Redis<br/>Catálogos y Sesión"]
        BLOB["Azure Blob Storage<br/>Audios HU17 · PDFs"]
        FUNC["Azure Functions<br/>Jobs: Mora · Corte · Backups"]
    end

    subgraph Datos["Capa de Datos"]
        SQL[("Azure SQL Database<br/>FullInternetServices · 8 tablas")]
        BACKUP[("Azure Backup<br/>Geo-redundante")]
    end

    subgraph Observabilidad["Observabilidad"]
        AI["Application Insights"]
        LOG["Azure Monitor / Log Analytics"]
    end

    DESKTOP -->|HTTPS · JWT| APIM
    WEB -->|HTTPS · JWT| FRONTDOOR
    MOBILE -->|HTTPS · JWT| APIM
    FRONTDOOR --> APIM
    APIM --> API

    API --> AAD
    API --> KV
    API --> REDIS
    API --> BLOB
    API --> SQL

    FUNC --> SQL
    FUNC --> BLOB
    SQL --> BACKUP
    API --> AI
    API --> LOG
    FUNC --> AI
```

</details>

### Decisiones clave

- **API REST como único punto de acceso a datos** → cumple RNF02 (RBAC) y RNF07 (cifrado).
- **API Management** delante del App Service para versionado, throttling y políticas.
- **Front Door + WAF** únicamente para tráfico web público (portal de clientes).
- Los WinForms hablan directamente con APIM (red privada o internet con TLS).

---

## 1.2 Arquitectura por Capas (Clean Architecture)

<img src="../diagrams/02-arquitectura-capas.png" alt="Arquitectura por capas" width="900" />

<details>
<summary>Ver fuente Mermaid</summary>

```mermaid
flowchart TB
    subgraph CapaPresentacion["1. Presentación"]
        WF["WinForms (Admin)"]
        WEBC["Web Cliente"]
        CTRL["Controllers API<br/>(ASP.NET Core)"]
    end

    subgraph CapaAplicacion["2. Aplicación"]
        APP_SVC["Casos de Uso<br/>(MediatR Handlers)"]
        DTO["DTOs · Commands · Queries"]
        VAL["Validadores<br/>(FluentValidation)"]
    end

    subgraph CapaDominio["3. Dominio (núcleo)"]
        ENT["Entidades<br/>Cliente, Contrato, Pago,<br/>Reclamo, Mora..."]
        ENUMS["Enums / Constantes"]
        DOM_SVC["Servicios de Dominio<br/>CalculadoraMora<br/>AsignadorTecnico"]
        IREPO["Interfaces Repo · IUnitOfWork"]
    end

    subgraph CapaInfra["4. Infraestructura"]
        EFCORE["EF Core 9<br/>DbContext · Migrations"]
        REPO["Repositorios"]
        AUTH["JWT · BCrypt"]
        STORAGE["Azure Blob Adapter"]
        CACHE["Redis Adapter"]
    end

    CapaPresentacion --> CapaAplicacion
    CapaAplicacion --> CapaDominio
    CapaInfra --> CapaDominio
    CapaPresentacion -.composition.-> CapaInfra
```

</details>

### Reglas de dependencia

1. `Presentación` → `Aplicación` → `Dominio` (flujo principal).
2. `Infraestructura` depende de `Dominio` (implementa sus interfaces — Inversión de Dependencia).
3. **El Dominio no depende de nadie**: no conoce EF, ni HTTP, ni Azure. Esto se refleja en `FIS.Domain.csproj`, que **no tiene `<PackageReference>` a nada externo**.
4. Los **DTOs viajan entre Presentación y Aplicación**; las **Entidades nunca salen del Dominio**.

### Mapeo proyecto → capa

| Capa | Proyecto |
|---|---|
| Presentación (API + Desktop) | `FIS.Api`, `FIS.Desktop` |
| Aplicación | `FIS.Application` |
| Dominio | `FIS.Domain` |
| Infraestructura | `FIS.Infrastructure` |
| Contratos compartidos | `FIS.Contracts` |

---

## 1.3 Diagrama de Componentes

Detalle de los componentes runtime y sus interacciones, incluyendo los repositorios, el pipeline de MediatR y los adaptadores de infraestructura.

<img src="../diagrams/03-componentes.png" alt="Diagrama de componentes" width="1000" />

<details>
<summary>Ver fuente Mermaid</summary>

```mermaid
flowchart LR
    subgraph WinForms["FIS.Desktop"]
        UI["Formularios (Vistas)"]
        APICLIENT["IFisApiClient<br/>(Refit)"]
        TOKEN["TokenStore<br/>(memoria + DPAPI)"]
        SESION["SesionUsuario"]
    end

    subgraph WebApp["Cliente Web (futuro)"]
        RAZOR["Razor Pages / Blazor"]
        WEBSVC["HttpClient tipado"]
    end

    subgraph API["FIS.Api"]
        AUTHC["AuthController"]
        CLIC["ClientesController"]
        OTROS["...Controllers"]
        MID["Middleware<br/>JWT · RBAC · Logging<br/>Exception Handler"]
    end

    subgraph App["FIS.Application"]
        UC1["LoginCommand<br/>+ Handler"]
        UC2["ListarClientesQuery<br/>+ Handler"]
        VALID["LoginCommandValidator<br/>(FluentValidation)"]
    end

    subgraph Domain["FIS.Domain"]
        D_USR["Usuario / Rol"]
        D_CLI["Cliente"]
        DS_MORA["CalculadoraMora"]
    end

    subgraph Infra["FIS.Infrastructure"]
        DBCTX["FisDbContext<br/>(EF Core)"]
        REPOS["UsuarioRepository<br/>ClienteRepository"]
        JWTSVC["JwtTokenService"]
        HASH["BCryptPasswordHasher"]
    end

    SQL[("Azure SQL")]
    BLOB[("Azure Blob")]
    REDIS[("Azure Redis")]

    UI --> APICLIENT --> MID
    RAZOR --> WEBSVC --> MID
    MID --> AUTHC & CLIC & OTROS
    AUTHC --> UC1
    CLIC --> UC2
    UC1 -.validates.-> VALID
    UC1 & UC2 --> Domain
    UC1 & UC2 --> REPOS
    REPOS --> DBCTX --> SQL
    JWTSVC --> AUTHC
    HASH --> UC1
```

</details>

---

## 1.4 Diagrama de Despliegue (Azure)

Vista física de la solución desplegada en producción.

<img src="../diagrams/04-despliegue-azure.png" alt="Despliegue Azure" width="1000" />

<details>
<summary>Ver fuente Mermaid</summary>

```mermaid
flowchart TB
    subgraph Internet["Internet"]
        USER1["Administrador<br/>(Oficina)"]
        USER2["Cajero / Técnico"]
        USER3["Cliente Final"]
    end

    subgraph Azure["Microsoft Azure - Brazil South / East US 2"]
        subgraph Edge["Edge"]
            FD["Azure Front Door<br/>+ WAF Premium"]
            APIM["API Management<br/>Standard v2"]
        end

        subgraph AppLayer["App Layer"]
            ASP1["App Service Plan P1v3"]
            ASP1 --> WEBAPI["App Service<br/>fis-api-prod"]
            ASP1 --> WEBAPP["App Service<br/>fis-web-prod"]
            FUNC["Function App<br/>fis-jobs-prod<br/>(Timer Triggers)"]
        end

        subgraph DataLayer["Data Layer"]
            SQL["Azure SQL DB<br/>GP_Gen5_2<br/>FullInternetServices"]
            BLOB["Storage Account<br/>fisstorageprod"]
            REDIS["Azure Cache for Redis<br/>Basic C1"]
        end

        subgraph Security["Seguridad"]
            KV["Key Vault<br/>fis-kv-prod"]
            AAD["Azure AD B2C"]
            MID["Managed Identity"]
        end

        subgraph Obs["Observabilidad"]
            AI["Application Insights"]
            LA["Log Analytics"]
            ALERT["Monitor Alerts"]
        end

        subgraph DevOps["DevOps"]
            ACR["Container Registry"]
            DEVOPS["Azure DevOps Pipelines"]
        end
    end

    USER1 -->|HTTPS WinForms| APIM
    USER2 -->|HTTPS WinForms| APIM
    USER3 -->|HTTPS Web| FD --> APIM

    APIM --> WEBAPI
    APIM --> WEBAPP

    WEBAPI --> SQL
    WEBAPI --> BLOB
    WEBAPI --> REDIS
    WEBAPI --> KV
    WEBAPI --> AAD
    WEBAPI -.uses.-> MID

    FUNC --> SQL
    FUNC --> BLOB
    WEBAPI --> AI
    FUNC --> AI
    AI --> LA --> ALERT

    DEVOPS --> ACR
    DEVOPS -.deploy.-> WEBAPI
    DEVOPS -.deploy.-> WEBAPP
    DEVOPS -.deploy.-> FUNC
```

</details>

Detalle de servicios Azure y SKUs en [05-cloud-azure](../05-cloud-azure/README.md).

---

## 1.5 Flujo de una Operación Típica

Ejemplo: registrar un pago desde la app de escritorio.

<img src="../diagrams/05-flujo-pago-secuencia.png" alt="Flujo de registrar pago" width="900" />

<details>
<summary>Ver fuente Mermaid</summary>

```mermaid
sequenceDiagram
    actor Cajero
    participant WF as frmRegistrarPago
    participant Refit as IFisApiClient
    participant API as PagosController
    participant MED as MediatR
    participant H as RegistrarPagoHandler
    participant DOM as Pago / CalculadoraMora
    participant REPO as PagoRepository
    participant DB as SQL Server

    Cajero->>WF: Datos del pago
    WF->>Refit: POST /api/v1/pagos
    Refit->>API: HTTP + JWT Bearer
    API->>API: ValidateToken + RBAC<br/>(Administrador, Cajero)
    API->>MED: RegistrarPagoCommand
    MED->>MED: ValidationBehavior<br/>(FluentValidation)
    MED->>H: Handle()
    H->>REPO: ObtenerContratoActivo()
    REPO->>DB: SELECT
    DB-->>REPO: Contrato
    H->>DOM: Pago.Crear()
    H->>DOM: CalculadoraMora.AplicarMora()
    H->>REPO: AddAsync(pago)
    H->>REPO: UnitOfWork.SaveChanges()
    REPO->>DB: INSERT (o EXEC sp_pago_insert)
    DB-->>REPO: id_pago, numero_recibo
    H-->>API: PagoDto
    API-->>Refit: 201 + ApiResponse
    Refit-->>WF: ApiResponse<PagoDto>
    WF-->>Cajero: Confirmación + recibo
```

</details>

---

## Referencias del PDF

| Sección PDF | Diagrama |
|---|---|
| 3.5 — Arquitectura de la Solución | 1.1, 1.2 |
| 3.5.5 — Representación Conceptual | 1.1 |
| 3.5.6 — Descripción Detallada de Capas | 1.2 |
| 3.5.7 — Flujo de Funcionamiento | 1.5 |
| 3.12 — Modelo de Despliegue | 1.4 (adaptado a Azure) |
