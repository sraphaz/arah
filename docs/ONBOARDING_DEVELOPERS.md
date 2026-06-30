# Desenvolvedores

---

## Bem-vindo ao Arah

Você chegou aqui porque **entende que tecnologia pode servir à vida** e não o contrário.

Este documento é para você que:
- Tem **raciocínio lógico** e quer aplicar sua inteligência de forma útil
- **Entende requisitos funcionais** e quer ver ideias se transformarem em código
- Vem de **outras áreas** (construção, matemática, humanidades) e quer descobrir possibilidades
- Quer **aprender a construir aplicações** participando de um projeto real
- Acredita que **nossa inteligência deve ser reconhecida como valor** a serviço do território

**Não precisa ser programador experiente**. Se você consegue pensar logicamente e tem curiosidade, está no lugar certo.

---

## Por que o Arah existe?

### O Problema

Vivemos um tempo em que plataformas digitais:
- Capturam nossa atenção e nos desconectam do lugar onde vivemos
- Transformam comunidades em mercados
- Extraem dados para lucro, sem reconhecer nossa inteligência como valor

### A Proposta

O Arah é uma **plataforma digital comunitária orientada ao território** que:
- **Reconhece o território** como referência fundamental
- **Valoriza a presença física** e a vida local
- **Respeita a autonomia** das comunidades
- **Não extrai dados** para publicidade
- **Serve ao cuidado coletivo** e à continuidade da vida no território

---

## Princípios Fundamentais

### 1. Território como Referência

O **território** no Arah é um lugar físico real:
- Tem nome, localização e fronteira geográfica
- Existe **antes do aplicativo** e continua existindo sem usuários
- **Não contém lógica social** (isso fica em camadas separadas)

> "O território existe antes do app e continua existindo mesmo sem usuários."

### 2. Vida Social em Camadas

Relações humanas (moradores, visitantes, visibilidade, moderação) **não pertencem ao território**.

Elas pertencem a **camadas sociais que referenciam o território**.

Isso torna o sistema:
- Mais claro
- Mais justo
- Mais adaptável ao tempo

### 3. Tecnologia a Serviço da Vida

O Arah **não é**:
- Um marketplace agressivo
- Uma rede de engajamento infinito
- Um produto de vigilância

É uma **infraestrutura digital comunitária** pensada para:
- Autonomia local
- Cuidado coletivo
- Continuidade da vida no território
- Fortalecimento do vínculo entre pessoas e lugar

---

## Como Funciona o Desenvolvimento

### O que você precisa

1. **Raciocínio lógico** - Você já tem isso
2. **Curiosidade** - Querer entender como as coisas funcionam
3. **Ferramenta**: **Cursor** - Um editor de código com inteligência artificial integrada

### O que é o Cursor?

**Cursor** é um editor de código que **entende o contexto** do projeto e ajuda você a:
- Escrever código seguindo os padrões do projeto
- Entender como as coisas funcionam
- Criar novas funcionalidades mantendo a consistência
- Aprender enquanto constrói

> **Agentes**: Leia [AGENTS.md](../AGENTS.md) — repo operável por agentes (manifests em `.agents/`, skills em `.skills/`).

---

## Estrutura do Projeto

O projeto está organizado em **camadas claras** (Clean Architecture):

```
backend/
├── Arah.Domain      # Conceitos centrais (território, posts, etc.)
├── Arah.Application # Lógica de negócio (services)
├── Arah.Infrastructure # Persistência (banco de dados, armazenamento)
├── Arah.Api         # Interface HTTP (endpoints)
└── Arah.Tests       # Testes automatizados
```

**Não precisa entender tudo de uma vez**. O Cursor vai te ajudar quando você precisar.

---

## Valores e Princípios

### Tecnologia Decolonizadora

O Arah busca:
- **Não replicar padrões coloniais** de extração e dominação
- **Valorizar saberes locais** e conhecimento do território
- **Reconhecer inteligência** como valor, não como recurso a extrair
- **Servir à autonomia** das comunidades, não controlá-las

### Digital ao Serviço do Social

A tecnologia aqui:
- **Não é fim**, é ferramenta
- **Serve ao território**, não o substitui
- **Fortalecer vínculos**, não substituí-los
- **Facilitar organização**, não organizar por nós

### Respeito à Vida

Desenvolvemos com:
- **Cuidado** - Cada linha de código tem impacto
- **Consciência** - Sabemos que tecnologia muda relações
- **Responsabilidade** - Fazemos escolhas que servem à vida
- **Humildade** - Não pretendemos ter todas as respostas

---

## Como Contribuir

### 1. Escolha algo para fazer

**Sugestões para iniciantes**:
- Melhorar documentação (traduzir, explicar melhor)
- Adicionar testes para funcionalidades existentes
- Corrigir bugs pequenos
- Melhorar mensagens de erro para usuários

**Pergunte ao Cursor**: "Quais são as tarefas mais simples para começar?"

### 2. Use o Cursor para fazer

1. **Descreva o que você quer fazer**
2. **Cursor gera o código** seguindo os padrões
3. **Você revisa e ajusta** (aprendendo no processo)
4. **Cursor valida** (build, testes, documentação)

### 3. Crie um Pull Request

**Antes de criar o PR, o Cursor te ajuda a**:
- ✅ Validar que build passa
- ✅ Validar que testes passam
- ✅ Verificar conflitos de merge
- ✅ Atualizar documentação

---

## Guia Técnico Detalhado

Para **detalhes técnicos sobre configuração do ambiente**, **passo a passo de setup**, **comandos específicos** e **exemplos práticos de desenvolvimento**, consulte o **guia técnico completo** no DevPortal:

👉 **[Onboarding para Desenvolvedores - DevPortal](https://devportal.Arah.app/#onboarding-developers)**

O guia técnico inclui:
- Verificação de requisitos do sistema (Windows/macOS/Linux)
- Instalação passo a passo de ferramentas (Git, .NET SDK 8.0, Cursor, Docker)
- Comandos detalhados para clonar, restaurar, compilar e testar
- Configuração do ambiente de desenvolvimento
- Guia de primeira contribuição com exemplo prático
- Entendimento detalhado da Clean Architecture
- Comandos para executar a API localmente

---

## Recursos de Aprendizado

### Documentação Essencial

1. **Comece por aqui**:
   - [`README.md`](../README.md) - Visão geral do projeto
   - [`docs/01_PRODUCT_VISION.md`](./01_PRODUCT_VISION.md) - Por que existe e para quem
   - [`docs/05_GLOSSARY.md`](./05_GLOSSARY.md) - Termos e conceitos

2. **Para entender código**:
   - [`.cursorrules`](../.cursorrules) - Regras e padrões (Cursor lê automaticamente)
   - [`docs/12_DOMAIN_MODEL.md`](./12_DOMAIN_MODEL.md) - Modelo de dados
   - [`docs/60_API_LÓGICA_NEGÓCIO.md`](./60_API_LÓGICA_NEGÓCIO.md) - Como as funcionalidades funcionam

3. **Para contribuir**:
   - [`docs/41_CONTRIBUTING.md`](./41_CONTRIBUTING.md) - Guia de contribuição
   - [`docs/CURSOR_DOCUMENTATION_RULES.md`](./CURSOR_DOCUMENTATION_RULES.md) - Regras de documentação

---

## Glossário Rápido

**Territory** (Território): Lugar físico real - a base de tudo no Arah  
**Visitor** (Visitante): Usuário presente no território com vínculo básico  
**Resident** (Morador): Usuário com vínculo aprovado e acesso a conteúdo restrito  
**Feed**: Linha do tempo de posts (pessoal ou do território)  
**Post**: Conteúdo publicado no feed  
**Item**: Produto ou serviço no marketplace (não "listing")  
**GeoAnchor**: Ponto georreferenciado (lat/lng) que ancora conteúdo no território  
**Membership**: Vínculo entre uma pessoa e um território  
**Clean Architecture**: Organização do código em camadas (Domain, Application, Infrastructure, API)

**Para mais termos**: Veja [`docs/05_GLOSSARY.md`](./05_GLOSSARY.md)

---

## Conclusão

Bem-vindo ao Arah.

Aqui, tecnologia serve à vida, não o contrário.

Aqui, sua inteligência é reconhecida como valor a serviço do território.

Aqui, você pode aprender a construir aplicações participando de algo que importa.

**Comece pequeno. Use o Cursor. Aprenda fazendo. Contribua com cuidado.**

---

**Última Atualização**: 2025-01-20  
**Versão**: 1.0

**Perguntas?** Abra uma Issue ou consulte o [guia técnico completo no DevPortal](https://devportal.Arah.app/#onboarding-developers)!
