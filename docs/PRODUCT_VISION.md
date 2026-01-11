# Visão do Produto (ARAPONGA)

## Propósito
Araponga é uma plataforma **território-first** e **comunidade-first** para organização comunitária local.
O território físico é a unidade central e a presença no território é requisito para vínculo.

## Princípios e regras inegociáveis
- **Território é a unidade central** e representa um lugar físico real.
- **Presença física é critério de vínculo**: no MVP não é possível associar território remotamente.
- **Consulta exige cadastro**: no MVP, feed e mapa exigem usuário autenticado.
- **Conteúdo é georreferenciado via GeoAnchor**.
- **Feed pessoal (perfil) + feed do território** coexistem.
- **Postagens referenciam ao menos 1 GeoAnchor** e podem conter múltiplos GeoAnchors (mídia é pós-MVP).
- **Visibilidade e papéis**: visitor e resident no MVP; friends/círculo interno é pós-MVP.
- **Mapa + feed integrados**: pins retornam dados mínimos para projeção no mapa; sincronia UI é pós-MVP.
- **Moderação**: reports de posts e de usuários; bloqueio; automações simples no MVP; sanções territoriais vs globais.
- **Admin**: visão administrativa para observar territórios, erros e relatórios (pós-MVP).

## Fluxo mínimo (MVP)
1. Usuário se cadastra.
2. A localização permite encontrar o território próximo (ex.: “Sertão do Camburi”).
3. Entra como **visitor**.
4. É incentivado a postar no feed.
5. A postagem aparece no mapa (via pins) e no feed; a sincronia visual entre pin e timeline fica para pós-MVP.

## Funcionalidades principais (classificação)
- [MVP] **Território + vínculo** com presença física local.
- [MVP] **Feeds** (pessoal e do território) com posts georreferenciados.
- [MVP] **Mapa integrado ao feed** via pins com dados mínimos.
- [MVP] **Visibilidade visitor/resident**.
- [MVP] **Reports e bloqueio** com moderação básica.
- [POST-MVP] **Friends (círculo interno)** e stories exclusivos.
- [POST-MVP] **Admin/observabilidade** com visão de territórios e saúde do sistema.
- [POST-MVP] **GeoAnchor avançado / memórias / galeria**.
- [POST-MVP] **Produtos/serviços territoriais, integrações e indicadores comunitários**.

## Público e impacto esperado
- Moradores, visitantes e organizações locais que precisam de um canal digital **ancorado ao território**.
- Fortalecimento de redes locais, comunicação de utilidade pública e presença comunitária real.

## Boas práticas transversais (MVP)
- **Consentimento explícito de localização** e explicação do motivo da coleta.
- **Falhas de geolocalização** comunicadas com orientação clara para o usuário.
- **Auditoria mínima** de ações de moderação e sanções.
- **Proteção contra abuso de reports** (deduplicação por janela de tempo).
- **Observabilidade mínima** (logs e métricas para erros de localização, reports e moderação).
