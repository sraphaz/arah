# Visão Geral - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 🌐 Visão Geral

O Araponga é uma plataforma **território-first** e **comunidade-first** para organização comunitária local. Todas as operações são orientadas ao território, com diferenciação clara entre **visitantes (VISITOR)** e **moradores (RESIDENT)**.

### Princípios Fundamentais

- **Território é geográfico e neutro**: Representa apenas um lugar físico real
- **Consulta exige cadastro**: Feed, mapa e operações sociais exigem usuário autenticado
- **Presença física é critério de vínculo**: No MVP, não é possível associar território remotamente
- **Visibilidade diferenciada**: Conteúdo pode ser Público (todos) ou Apenas Moradores (RESIDENTS_ONLY)

### 🔒 Segurança e Rate Limiting

A API implementa várias camadas de segurança:

- **Rate Limiting**: Proteção contra abuso e DDoS
  - Auth endpoints: 5 req/min
  - Feed endpoints: 100 req/min
  - Write endpoints: 30 req/min
  - Global: 60 req/min (configurável)
- **HTTPS Obrigatório**: Em produção, todas as conexões são criptografadas
- **Security Headers**: Headers de segurança em todas as respostas
- **Validação de Input**: Validação automática de todos os requests
- **CORS Configurado**: Políticas de CORS por ambiente

Quando o rate limit é excedido, a API retorna:
- **Status Code**: `429 Too Many Requests`
- **Header**: `Retry-After` com tempo em segundos
- **Body**: ProblemDetails com detalhes do erro

---

## 📚 Documentação Relacionada

- **[Paginação](./60_00_API_PAGINACAO.md)** - Padrão de paginação em todos os endpoints
- **[Autenticação](./60_01_API_AUTENTICACAO.md)** - Login social e tokens JWT
- **[Territórios](./60_02_API_TERRITORIOS.md)** - Descoberta e seleção de territórios
- **[Regras de Visibilidade](./60_17_API_VISIBILIDADE.md)** - Regras de acesso e visibilidade

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
