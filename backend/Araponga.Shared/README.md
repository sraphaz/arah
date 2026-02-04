# Araponga.Shared

Biblioteca para **tipos e utilitários compartilhados** entre projetos do backend (Application e outros).

## Uso

- **Constantes** comuns a mais de um projeto (nomes de claims, chaves de config, etc.).
- **Extensões** (extension methods) que não pertencem a Domain nem a um módulo.
- **DTOs ou tipos** compartilhados entre Application e Api/Infrastructure quando não couberem em Domain.

Não coloque aqui:
- Entidades de domínio → **Araponga.Domain** ou no módulo.
- Interfaces de aplicação ou repositórios → **Araponga.Application** (Interfaces).
- Código específico de um módulo → no próprio módulo.

## Referências

Este projeto deve ter **poucas dependências** (idealmente só .NET BCL) para ser referenciado sem ciclos. **Araponga.Application** referencia Araponga.Shared.
