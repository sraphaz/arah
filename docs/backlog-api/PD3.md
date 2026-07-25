# PD-3: Territorial Organizations (CNPJ + reivindicação)

**Duração**: ~4–6 semanas  
**Prioridade**: 🟡 P1  
**Trilha**: Dados Públicos Territoriais  
**Depende de**: [PD-0](./PD0.md) · Store/Marketplace existentes · filas de verificação (WorkItem)  
**Status**: ⏳ Pendente

---

## Contexto

Existe `Store` territorial sem CNPJ e **sem** entidade Organization. Coletivos informais e pessoas físicas são centrais ao produto.

## Problema

Não há enriquecimento por registro público nem fluxo de reivindicação/verificação separado de “dado oficial”.

## Resultado esperado

Organizações formais podem informar CNPJ, confirmar/corrigir dados públicos e entrar em verificação de vínculo — **sem** selo automático e **sem** excluir informais.

## Escopo

### História — Organização formal

Como representante, quero informar CNPJ e confirmar dados públicos, para perfil territorial mais confiável.  
Critérios: consulta não concede selo; confirmação de vínculo; correção permitida; dado público ≠ comunitário; informais suportados.

### Modelagem (a confrontar no design — não copiar cegamente)

Avaliar extensão de `Store` **ou** novo agregado `TerritorialOrganization`:

- `TerritoryId`, `DisplayName`, `OrganizationType` (PF | InformalCollective | FormalOrg)
- `TaxId` (opcional), `PublicRegistrySnapshotId`
- `CommunityDescription`, `VerificationStatus`, `ClaimedByUserId`
- `GeoAnchorId` / horários / serviços

Fluxo: tipo → se formal, CNPJ → snapshot → confirmação → WorkItem de verificação.

### Técnico

- `IOrganizationRegistryProvider` → `BrasilApiOrganizationRegistryProvider`
- Separação registro público vs perfil comunitário
- Privacidade/LGPD e auditoria de consultas CNPJ

## Fora do escopo

- KYC completo / prova de representação legal automática
- Aprovação automática por situação cadastral Receita
- Exigir CNPJ para vender ou existir no mapa
- Participantes Pix

## Dependências

- PD-0 · Marketplace Store · WorkItem · identidade/privacidade · (opcional) FASE17 economia

## Critérios de aceite

- [ ] PF e coletivo informal cadastram sem TaxId
- [ ] Formal: lookup CNPJ + confirmação explícita do usuário
- [ ] VerificationStatus independente de “dados Receita ok”
- [ ] Snapshot separado do texto comunitário editável
- [ ] Testes de fallback, dado incompleto, CNPJ inválido, rate limit

## Riscos

- Exposição de dados cadastrais · scraping indevido · falsa autoridade do “verificado CNPJ”

## Definição de pronto

DoD + revisão domínio `mercado-economia` + `identidade-privacidade` + `territorio-membership`.

## Referências

- [REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS](./REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS.md)
- [PD4](./PD4.md)
