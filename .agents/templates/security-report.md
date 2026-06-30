## Security Agent — Relatório

<!-- arah-security-gate -->

### Dependências
- [ ] `dotnet list package --vulnerable` sem críticas novas
- [ ] lock files commitados quando deps mudaram

### Secrets
- [ ] Nenhum token/senha/API key no diff
- [ ] Sem `.env` ou credenciais em paths rastreados

### LGPD / dados sensíveis
- [ ] Sem log de PII desnecessário
- [ ] Dados sensíveis conforme [SECURITY.md](SECURITY.md)

### Bloqueio recomendado
- CVE crítico novo → **não mergear** até correção
- Secret no diff → **reverter imediatamente**

---
_Automático via `agents-gates.yml`._
