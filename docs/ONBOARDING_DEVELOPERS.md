# Desenvolvedores

---

## 🎯 Bem-vindo ao Araponga

Você chegou aqui porque **entende que tecnologia pode servir à vida** e não o contrário.

Este documento é para você que:
- Tem **raciocínio lógico** e quer aplicar sua inteligência de forma útil
- **Entende requisitos funcionais** e quer ver ideias se transformarem em código
- Vem de **outras áreas** (construção, matemática, humanidades) e quer descobrir possibilidades
- Quer **aprender a construir aplicações** participando de um projeto real
- Acredita que **nossa inteligência deve ser reconhecida como valor** a serviço do território

**Não precisa ser programador experiente**. Se você consegue pensar logicamente e tem curiosidade, está no lugar certo.

---

## 🌍 Por que o Araponga existe?

### O Problema

Vivemos um tempo em que plataformas digitais:
- Capturam nossa atenção e nos desconectam do lugar onde vivemos
- Transformam comunidades em mercados
- Extraem dados para lucro, sem reconhecer nossa inteligência como valor

### A Proposta

O Araponga é uma **plataforma digital comunitária orientada ao território** que:
- **Reconhece o território** como referência fundamental
- **Valoriza a presença física** e a vida local
- **Respeita a autonomia** das comunidades
- **Não extrai dados** para publicidade
- **Serve ao cuidado coletivo** e à continuidade da vida no território

### Para quem é feito?

Para pessoas e comunidades que precisam de um canal digital **ancorado ao território real**:
- Moradores e visitantes que querem se conectar com o lugar
- Comunidades que precisam organizar-se digitalmente sem perder o vínculo territorial
- Quem acredita que **tecnologia deve servir à vida**, não capturá-la

---

## 💡 Princípios Fundamentais

### 1. Território como Referência

O **território** no Araponga é um lugar físico real:
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

O Araponga **não é**:
- Um marketplace agressivo
- Uma rede de engajamento infinito
- Um produto de vigilância

É uma **infraestrutura digital comunitária** pensada para:
- Autonomia local
- Cuidado coletivo
- Continuidade da vida no território
- Fortalecimento do vínculo entre pessoas e lugar

---

## 🛠️ Como Funciona o Desenvolvimento

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

> **Dica**: O Cursor **lê automaticamente** o arquivo `.cursorrules` na raiz do projeto, que contém todas as regras e padrões. Ele vai te guiar!

### Você não precisa saber tudo

O Cursor te ajuda a:
- Escrever código mesmo sem conhecer toda a sintaxe
- Entender arquitetura enquanto trabalha
- Seguir padrões automaticamente
- Aprender na prática

**É como ter um colega experiente ao lado**, sempre pronto para ajudar.

---

## 🚀 Primeiros Passos

### 1. Verificar Requisitos Mínimos

Antes de começar, vamos garantir que você tem tudo necessário:

#### Requisitos do Sistema

**Obrigatórios:**
- **Sistema Operacional**: Windows 10/11, macOS 10.15+, ou Linux (Ubuntu 20.04+)
- **Memória RAM**: Mínimo 4GB (recomendado 8GB ou mais)
- **Espaço em Disco**: Mínimo 2GB livres
- **Conexão Internet**: Para baixar dependências e atualizações

**Programas Necessários:**

1. **Git** (controle de versão)
   - Windows: Baixe em https://git-scm.com/download/win
   - macOS: Instale via `brew install git` ou https://git-scm.com/download/mac
   - Linux: `sudo apt install git` (Ubuntu/Debian) ou equivalente

2. **.NET SDK 8.0** (para executar o projeto)
   - Baixe em: https://dotnet.microsoft.com/download/dotnet/8.0
   - Escolha "SDK" (não apenas Runtime)
   - Instale seguindo as instruções do instalador
   - Verifique após instalar: `dotnet --version` (deve mostrar 8.0.x)

3. **Cursor** (editor de código recomendado)
   - Baixe em: https://cursor.sh/
   - É gratuito e tem versões para Windows, macOS e Linux
   - Instale normalmente (setup automático)

**Opcionais (mas recomendados):**

4. **Docker Desktop** (para banco de dados Postgres)
   - Baixe em: https://www.docker.com/products/docker-desktop
   - Útil para rodar banco de dados localmente
   - Não é obrigatório - o projeto funciona com banco em memória para desenvolvimento

#### Verificar Instalações

Abra um terminal (PowerShell no Windows, Terminal no macOS/Linux) e execute:

```bash
# Verificar Git
git --version
# Deve mostrar algo como: git version 2.40.0 ou superior

# Verificar .NET
dotnet --version
# Deve mostrar: 8.0.x

# Verificar Cursor (se instalado)
# Abra o Cursor e vá em Help > About para ver a versão
```

**Se alguma verificação falhar**: Instale o programa correspondente e tente novamente.

### 2. Configurar o Ambiente

#### Passo 1: Clonar o Projeto

Abra um terminal e execute:

```bash
# Clone o repositório
git clone https://github.com/sraphaz/araponga.git

# Entre na pasta do projeto
cd araponga
```

**Verificação**: Você deve ver a pasta `araponga` com arquivos dentro (backend/, docs/, README.md, etc.)

#### Passo 2: Restaurar Dependências

```bash
# Restaura pacotes NuGet necessários
dotnet restore
```

**O que acontece**: O .NET baixa todas as bibliotecas necessárias (pode levar alguns minutos na primeira vez).

**Verificação**: Ao terminar, não deve haver erros. Você verá "Restore succeeded" ou similar.

#### Passo 3: Compilar o Projeto

```bash
# Compila o projeto para verificar se tudo está OK
dotnet build
```

**Verificação**: Deve terminar com "Build succeeded". Se houver erros, verifique se o .NET SDK 8.0 está instalado corretamente.

#### Passo 4: Abrir no Cursor

1. **Abra o Cursor** (duplo-clique no ícone)
2. **File > Open Folder** (ou `Ctrl+K Ctrl+O` no Windows/Linux, `Cmd+K Cmd+O` no macOS)
3. **Selecione a pasta `araponga`** que você acabou de clonar
4. **Pronto!** O Cursor automaticamente:
   - Lê o arquivo `.cursorrules` com todas as regras do projeto
   - Configura o ambiente de desenvolvimento
   - Prepara para você começar a trabalhar

**Verificação**: No Cursor, você deve ver a estrutura do projeto na barra lateral (Explorer).

#### Passo 5: (Opcional) Testar o Projeto

Para garantir que tudo está funcionando:

```bash
# Rodar os testes
dotnet test
```

**O que acontece**: Executa todos os testes automatizados (pode levar alguns minutos).

**Resultado esperado**: Todos os testes passam (ou a maioria, se houver alguns pendentes). Se houver falhas, não se preocupe - pode ser configuração específica.

**Executar a API localmente** (opcional, para ver funcionando):

```bash
# Rodar a API
dotnet run --project backend/Araponga.Api
```

**O que acontece**: A API inicia e você verá uma mensagem como "Now listening on: http://localhost:5000"

**Testar**: Abra no navegador `http://localhost:5000` - você verá o portal do desenvolvedor.

**Para parar**: Pressione `Ctrl+C` no terminal.

---

### 3. Configuração Consciente (Recomendado)

#### Espaço de Trabalho Adequado

**Considere:**
- **Dedique tempo** - desenvolvimento requer atenção
- **Ambiente confortável** - lugar onde você pode focar
- **Internet estável** - para baixar dependências e pesquisar
- **Backup** - git já faz isso, mas mantenha suas mudanças commitadas

#### Primeiro Uso do Cursor

**Cursor pode parecer complexo no início - é normal!**

- **Comece simples**: Use a interface básica primeiro
- **Explore gradualmente**: Vá descobrindo funcionalidades conforme precisa
- **Use a ajuda**: `Ctrl+Shift+P` (Windows/Linux) ou `Cmd+Shift+P` (macOS) abre comandos
- **Pergunte ao Cursor**: Ele entende o contexto do projeto e pode ajudar

### 2. Entender a Estrutura

O projeto está organizado em **camadas claras**:

```
backend/
├── Araponga.Domain      # Conceitos centrais (território, posts, etc.)
├── Araponga.Application # Lógica de negócio (services)
├── Araponga.Infrastructure # Persistência (banco de dados, armazenamento)
├── Araponga.Api         # Interface HTTP (endpoints)
└── Araponga.Tests       # Testes automatizados
```

**Não precisa entender tudo de uma vez**. O Cursor vai te ajudar quando você precisar.

### 5. Começar Pequeno

**Sugestões de primeiras contribuições**:

#### Correção de Documentação
- Leia um documento em `docs/`
- Encontre algo desatualizado ou confuso
- Corrija e atualize
- **Resultado**: Você aprende o projeto enquanto melhora a documentação

#### Melhorar Testes
- Encontre um teste em `backend/Araponga.Tests/`
- Leia e entenda o que ele testa
- Adicione um caso de teste novo
- **Resultado**: Você aprende como as funcionalidades funcionam

#### Pequena Melhoria de Código
- Escolha um arquivo simples
- Use o Cursor para entender o código
- Faça uma melhoria pequena (legibilidade, comentários, etc.)
- **Resultado**: Você aprende padrões de código

---

## 🎓 Aprendendo na Prática

### Como o Cursor te Ajuda

#### 1. Quando você quer entender algo:

**Pergunte ao Cursor**:
```
"Como funciona o sistema de criação de posts?"
"Onde está a lógica de validação de território?"
"Como adiciono uma nova funcionalidade seguindo os padrões do projeto?"
```

O Cursor **entende o contexto** e te explica usando o código do projeto.

#### 2. Quando você quer criar algo:

**Descreva o que você quer**:
```
"Quero criar um endpoint para listar eventos próximos de uma localização"
```

O Cursor **cria o código** seguindo os padrões do projeto automaticamente.

#### 3. Quando você não tem certeza:

**Peça validação**:
```
"Esse código segue os padrões do projeto?"
"Como faço isso seguindo Clean Architecture?"
```

O Cursor **valida e corrige** seguindo as regras em `.cursorrules`.

### Regras Automáticas

O Cursor **sempre**:
- ✅ Atualiza documentação quando você muda código
- ✅ Valida build e testes antes de PR
- ✅ Segue padrões de nomenclatura (territory, items, 29 fases)
- ✅ Mantém Clean Architecture
- ✅ Adiciona testes para novas funcionalidades

**Você não precisa lembrar de tudo**. O Cursor lembra para você.

---

## 📚 Recursos de Aprendizado

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

### Exercícios Práticos

#### Exercício 1: Entender um Fluxo Completo

1. Escolha uma funcionalidade simples (ex: "listar posts do território")
2. Use o Cursor para perguntar: "Como funciona a listagem de posts?"
3. Siga o código do endpoint até o banco de dados
4. **Objetivo**: Entender como ideia vira código

#### Exercício 2: Fazer uma Mudança Pequena

1. Escolha um arquivo de teste simples
2. Adicione um caso de teste novo (o Cursor ajuda)
3. Rode os testes e veja passar
4. **Objetivo**: Sentir a satisfação de ver código funcionando

#### Exercício 3: Ler e Melhorar Documentação

1. Leia um documento técnico
2. Identifique algo que pode ser mais claro
3. Reescreva de forma mais simples
4. **Objetivo**: Contribuir enquanto aprende

---

## 🌱 Valores e Princípios

### Tecnologia Decolonizadora

O Araponga busca:
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

## 🤝 Como Contribuir

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

**Use o template** em `.github/pull_request_template.md` que tem checklist completo.

### 4. Aprenda com Feedback

- Code reviews são oportunidades de aprender
- Perguntas são bem-vindas
- Não existe pergunta "boba" - todos estamos aprendendo

---

## 🧠 Para Quem Vem de Outras Áreas

### Matemático? Perfeito!

Você já tem:
- ✅ Raciocínio lógico estruturado
- ✅ Capacidade de abstração
- ✅ Entendimento de sistemas e relações

**Aplicação aqui**:
- Modelagem de dados (como estruturar informações)
- Lógica de negócio (como validar regras)
- Algoritmos (como resolver problemas eficientemente)

**Cursor te ajuda a**: Traduzir seu raciocínio matemático em código.

### Trabalha com Construção? Perfeito!

Você já entende:
- ✅ Como ideias viram realidade física
- ✅ Estrutura, organização, dependências
- ✅ Como sistemas complexos funcionam

**Aplicação aqui**:
- Arquitetura de código (estrutura do projeto)
- Fluxos de trabalho (como as coisas se conectam)
- Organização (como manter ordem em sistemas complexos)

**Cursor te ajuda a**: Ver código como construção de sistemas lógicos.

### Vem de Humanidades? Perfeito!

Você entende:
- ✅ Como pessoas se relacionam
- ✅ Contexto, cultura, território
- ✅ Como comunicação funciona

**Aplicação aqui**:
- Documentação (como explicar claramente)
- Interface com usuários (como comunicar funcionalidades)
- Validação de requisitos (como garantir que serve às pessoas)

**Cursor te ajuda a**: Conectar valores humanos com código.

---

## 🎯 Fluxo de Trabalho Recomendado

### 1. Entender o Contexto

```bash
# Leia primeiro
- README.md
- docs/01_PRODUCT_VISION.md
- docs/05_GLOSSARY.md

# Pergunte ao Cursor
"Me explique o que é territory neste projeto"
"Como funciona a criação de posts?"
```

### 2. Escolher uma Tarefa

```bash
# Veja o que precisa ser feito
- docs/02_ROADMAP.md (roadmap)
- docs/STATUS_FASES.md (status das fases)
- GitHub Issues (tarefas abertas)

# Ou crie sua própria
"Quero melhorar X porque..."
```

### 3. Desenvolver

```bash
# Use o Cursor para
1. Entender o código existente
2. Criar nova funcionalidade seguindo padrões
3. Adicionar testes
4. Atualizar documentação
```

### 4. Validar

```bash
# Cursor valida automaticamente, mas você pode verificar
dotnet build --configuration Release
dotnet test --configuration Release
```

### 5. Contribuir

```bash
# Criar PR seguindo template
# Cursor garante que tudo está certo
# Aprender com feedback
```

---

## 💬 Comunicação e Colaboração

### Linguagem

- **Seja claro** - Escreva como se explicando para alguém novo
- **Seja gentil** - Estamos todos aprendendo
- **Seja honesto** - Não sabe algo? Perfeito! Pergunte

### Como Pedir Ajuda

**Ao Cursor**:
- "Não entendo como X funciona"
- "Como faço Y seguindo os padrões do projeto?"
- "Esse código está correto?"

**À Comunidade**:
- Abra uma Issue no GitHub
- Explique o que você está tentando fazer
- Compartilhe o que já tentou

### Como Oferecer Ajuda

- Revise Pull Requests
- Responda Issues
- Melhore documentação
- Compartilhe aprendizados

---

## 🌿 Valores na Prática

### Respeito ao Território

Quando você escreve código:
- Pense no impacto real no território
- Considere como afeta comunidades
- Valorize a presença física e vínculos locais

### Autonomia

Quando você cria funcionalidades:
- Priorize controle local sobre centralização
- Dê opções, não imponha escolhas
- Respeite decisões comunitárias

### Cuidado

Quando você faz mudanças:
- Teste bem antes de publicar
- Documente o que fez
- Pense em quem vai manter depois

### Transparência

Quando você contribui:
- Explique o que você fez e por quê
- Documente decisões
- Compartilhe conhecimento

---

## 📖 Glossário Rápido

**Territory** (Território): Lugar físico real - a base de tudo no Araponga  
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

## 🎓 Aprendizado Contínuo

### Recursos Recomendados

1. **Sobre Clean Architecture**:
   - O Cursor segue automaticamente, mas entender ajuda
   - Pesquise: "Clean Architecture C#"

2. **Sobre C# (.NET)**:
   - Linguagem usada no backend
   - O Cursor te ajuda, mas entender o básico ajuda mais
   - Pesquise: "C# basics" ou "C# async await"

3. **Sobre Git/GitHub**:
   - Ferramenta de controle de versão
   - Essencial para contribuir
   - Pesquise: "Git basics" ou "GitHub workflow"

### Prática é a Melhor Professora

- **Não espere saber tudo** - Comece e aprenda fazendo
- **Use o Cursor** - Ele está aqui para ajudar
- **Pergunte** - Comunidade e Cursor estão aqui para isso
- **Contribua pequeno** - Pequenas contribuições são bem-vindas

---

## 🌍 Impacto Social

### Por que isso importa?

Quando você contribui com o Araponga, você:

1. **Democratiza tecnologia**:
   - Mostra que desenvolvimento não é só para "especialistas"
   - Valoriza diferentes tipos de inteligência
   - Cria caminhos para pessoas de outras áreas

2. **Serve ao território**:
   - Fortalece comunidades locais
   - Facilita organização comunitária
   - Respeita autonomia e vínculos territoriais

3. **Descoloniza digital**:
   - Cria tecnologia que não extrai, mas serve
   - Reconhece inteligência como valor
   - Coloca tecnologia a serviço da vida

### Histórias de Transformação

> "Vim da construção, sou matemático. Descobri que posso usar minha lógica para construir sistemas que servem ao território. Não preciso mudar completamente de área - posso expandir minhas possibilidades."

> "Entendo requisitos funcionais e sempre quis ver minhas ideias viradas em código. Com o Cursor e o Araponga, finalmente consigo participar de algo maior."

---

## 🚦 Primeiros Passos Práticos

### Checklist de Início

- [ ] Instalei o Cursor
- [ ] Clonei o repositório
- [ ] Abri o projeto no Cursor
- [ ] Li o README.md
- [ ] Li docs/01_PRODUCT_VISION.md
- [ ] Entendi o que é "territory" no projeto
- [ ] Perguntei ao Cursor sobre algo que não entendi
- [ ] Identifiquei uma pequena tarefa para começar

### Tarefa Sugerida para Primeira Contribuição

**Melhorar um comentário em código**:

1. Abra um arquivo em `backend/Araponga.Application/Services/`
2. Encontre um comentário que pode ser mais claro
3. Use o Cursor: "Como posso melhorar este comentário para ficar mais claro?"
4. Faça a mudança
5. Crie um PR

**Resultado**: Você contribui, aprende e ganha confiança.

---

## 💡 Dicas Finais

### Para Aprender Melhor

1. **Faça pequeno** - Pequenas contribuições são mais fáceis de entender
2. **Pergunte ao Cursor** - Ele está aqui para isso
3. **Leia código existente** - Veja como outras coisas funcionam
4. **Documente enquanto aprende** - Escrever ajuda a entender

### Para Contribuir Melhor

1. **Siga as regras** - Elas estão no `.cursorrules` (Cursor aplica automaticamente)
2. **Teste antes de PR** - Cursor valida, mas você pode verificar
3. **Documente o que fez** - Ajuda quem vai manter depois
4. **Peça feedback** - Code reviews são oportunidades de aprender

### Para Manter os Valores

1. **Pense no território** - Como isso serve às comunidades?
2. **Respeite autonomia** - Não centralize, não controle
3. **Seja transparente** - Documente decisões e motivos
4. **Valorize inteligência** - Reconheça contribuições e saberes diversos

---

## 🤝 Comunidade

### Como Participar

- **GitHub Issues**: Tarefas, bugs, sugestões
- **Pull Requests**: Contribuições de código
- **Discussions**: Ideias, dúvidas, conversas

### Código de Conduta

- Seja respeitoso
- Seja acolhedor
- Valorize diferentes perspectivas
- Aprenda juntos

**Leia**: [`CODE_OF_CONDUCT.md`](../CODE_OF_CONDUCT.md)

---

## 📚 Próximos Passos

1. **Configure seu ambiente** (Cursor + projeto)
2. **Leia a documentação essencial** (README + Product Vision)
3. **Explore o código** com o Cursor
4. **Escolha uma tarefa pequena** para começar
5. **Contribua** e aprenda no processo

### Lembre-se

- **Você não precisa saber tudo** - Aprenda fazendo
- **Cursor está aqui para ajudar** - Use e abuse
- **Pequenas contribuições são valiosas** - Não precisa fazer tudo de uma vez
- **Toda inteligência é bem-vinda** - Matemático, construtor, humanista, todos podem contribuir

---

## 🌱 Conclusão

Bem-vindo ao Araponga.

Aqui, tecnologia serve à vida, não o contrário.

Aqui, sua inteligência é reconhecida como valor a serviço do território.

Aqui, você pode aprender a construir aplicações participando de algo que importa.

Aqui, desenvolvimento é **decriação** - criação consciente que descoloniza o digital.

**Comece pequeno. Use o Cursor. Aprenda fazendo. Contribua com cuidado.**

---

**Última Atualização**: 2025-01-20  
**Versão**: 1.0

**Perguntas?** Abra uma Issue ou pergunte ao Cursor!
