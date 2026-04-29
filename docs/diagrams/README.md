# Diagramas Visuales

Esta carpeta contiene los **24 diagramas** de la documentación arquitectónica en dos formatos:

| Extensión | Propósito |
|---|---|
| `.mmd` | Fuente Mermaid editable (canónico). |
| `.png` | Render visual generado a partir del `.mmd`. |

Los archivos están numerados secuencialmente (`01-`, `02-`, ...) y nombrados de forma descriptiva.

---

## Cómo regenerar los PNG

Cuando edites un `.mmd`, regenera el PNG correspondiente con el script:

```powershell
pwsh -File scripts/render-diagrams.ps1
```

El script usa `npx @mermaid-js/mermaid-cli` y sólo requiere **Node.js** instalado. La primera ejecución descarga la herramienta (~150 MB en `~/.npm/_npx/`).

### Render manual de un único diagrama

```powershell
npx @mermaid-js/mermaid-cli -i docs/diagrams/09-base-datos-er.mmd -o docs/diagrams/09-base-datos-er.png -w 1600 -b white
```

---

## Catálogo de Diagramas

| # | Archivo | Sección |
|---|---|---|
| 01 | `01-arquitectura-alto-nivel` | [01-arquitectura](../01-arquitectura/) |
| 02 | `02-arquitectura-capas` | [01-arquitectura](../01-arquitectura/) |
| 03 | `03-componentes` | [01-arquitectura](../01-arquitectura/) |
| 04 | `04-despliegue-azure` | [01-arquitectura](../01-arquitectura/) |
| 05 | `05-flujo-pago-secuencia` | [01-arquitectura](../01-arquitectura/) |
| 06 | `06-backend-estructura-proyectos` | [02-backend](../02-backend/) |
| 07 | `07-backend-flujo-autenticacion` | [02-backend](../02-backend/) |
| 08 | `08-backend-cqrs-pipeline` | [02-backend](../02-backend/) |
| 09 | `09-base-datos-er` | [03-base-datos](../03-base-datos/) |
| 10 | `10-desktop-arquitectura` | [04-desktop](../04-desktop/) |
| 11 | `11-desktop-mvp-class` | [04-desktop](../04-desktop/) |
| 12 | `12-desktop-flujo-login` | [04-desktop](../04-desktop/) |
| 13 | `13-cloud-topologia` | [05-cloud-azure](../05-cloud-azure/) |
| 14 | `14-cloud-ambientes` | [05-cloud-azure](../05-cloud-azure/) |
| 15 | `15-cloud-gitflow` | [05-cloud-azure](../05-cloud-azure/) |
| 16 | `16-cloud-keyvault` | [05-cloud-azure](../05-cloud-azure/) |
| 17 | `17-mapa-documentacion` | [docs/](../) |
| 18 | `18-requerimientos-mindmap` | [06-requerimientos](../06-requerimientos/) |
| 19 | `19-mccall-calidad` | [06-requerimientos](../06-requerimientos/) |
| 20 | `20-monolito-vs-microservicios` | [07-mejoras](../07-mejoras/) |
| 21 | `21-logging-stack` | [07-mejoras](../07-mejoras/) |
| 22 | `22-versionado-gantt` | [07-mejoras](../07-mejoras/) |
| 23 | `23-caching` | [07-mejoras](../07-mejoras/) |
| 24 | `24-biometrico-secuencia` | [07-mejoras](../07-mejoras/) |

---

## Por qué dos formatos

1. **`.mmd` (texto)**: editable en cualquier editor, diff-friendly en Git, fuente única de verdad.
2. **`.png` (imagen)**: visible sin plugins en cualquier visor de Markdown (incluso visores que no soportan Mermaid), exportable a Word/PDF/presentaciones.

Los READMEs de cada área **incluyen ambas formas**: bloque Mermaid (para editar/copiar) seguido de la imagen `<img>` (para visualización inmediata).
