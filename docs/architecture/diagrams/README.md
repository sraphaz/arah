# Diagramas de arquitetura (LikeC4)

Exportados por `./scripts/diagrams/export-likec4.ps1` com tokens do design system.

| Pasta | Formato |
|-------|---------|
| `png/` | PNG (LikeC4 CLI, tema dark) |
| `svg/` | SVG (Graphviz dot + tokens Arah) |
| `dot/` | Graphviz intermediário |

Modelo fonte: [../likec4/arah.likec4](../likec4/arah.likec4) · Legado: [../../design/arah.likec4](../../design/arah.likec4)

Regenerar:

```powershell
./scripts/diagrams/export-likec4.ps1
./scripts/agents/arah-agents.ps1 skill -Skill likec4-export
```
