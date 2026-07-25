# PD-4: Financial Reference Data (bancos)

**Duração**: ~1–2 semanas  
**Prioridade**: 🟡 P1  
**Trilha**: Dados Públicos Territoriais  
**Depende de**: [PD-0](./PD0.md) · [FASE7](./FASE7.md) (payouts ✅)  
**Status**: ⏳ Pendente

---

## Contexto

Payouts existem via `IPayoutGateway`, mas cadastros de instituição financeira tendem a texto livre / dados do PSP.

## Problema

Códigos de banco inconsistentes atrapalham UX e suporte; risco de confundir diretório público com validação de titularidade.

## Resultado esperado

`IFinancialInstitutionDirectory` para listar código/ISPB/nome como **referência** em formulários de payout — sem substituir o PSP.

## Escopo

- Adapter BrasilAPI (bancos)
- Cache de lista (baixa volatilidade)
- Integração opcional nos formulários de dados bancários do vendedor/território

## Fora do escopo

- Validar agência/conta/titularidade
- Transferir valores
- Substituir Stripe/MP/`IPayoutGateway`
- Participantes Pix (fora, salvo produto Pix próprio)

## Critérios de aceite

- [ ] Lista de bancos via API Arah com cache
- [ ] Seleção no formulário não implica conta verificada
- [ ] Provider down → formulário manual
- [ ] Copy/UI deixa claro: “referência pública, não confirmação bancária”

## Riscos

- Usuário achar que o banco foi “validado” · lista desatualizada

## Referências

- [REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS](./REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS.md)
- [PD3](./PD3.md) · FASE55 (monetização) — sem acoplar Pix directory
