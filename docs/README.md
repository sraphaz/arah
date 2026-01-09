# 🐦 Araponga

**Araponga** é uma plataforma digital comunitária orientada ao território.  
Um espaço onde tecnologia serve à vida local, à convivência e à autonomia das comunidades.

Não é uma rede social genérica.  
É uma **extensão digital do território vivo**.

---

## 🌱 Propósito

Vivemos um tempo em que plataformas digitais:
- capturam atenção,
- desorganizam comunidades,
- e desconectam pessoas do lugar onde vivem.

O Araponga nasce como um contraponto consciente a esse modelo.

> **Território como referência.  
> Comunidade como prioridade.  
> Tecnologia como ferramenta — não como fim.**

A proposta é simples e profunda:  
**recolocar o território no centro da experiência digital.**

---

## 🧭 O que é o Araponga?

O Araponga é um aplicativo/plataforma que permite:

- 📍 **Descobrir e reconhecer territórios reais**
- 👥 **Organizar comunidades locais**
- 🗞️ **Compartilhar informações relevantes ao lugar**
- 🗺️ **Visualizar eventos, avisos e iniciativas no mapa**
- 🧑‍🌾 **Diferenciar moradores e visitantes com respeito**
- 🤝 **Fortalecer redes locais de cuidado, troca e presença**

Tudo isso **sem algoritmos de manipulação**,  
sem feed global infinito,  
sem extração de dados para publicidade.

---

## 🧠 Princípios fundamentais

### 1. Território é geográfico (e neutro)

No Araponga, `territory` representa **apenas um lugar físico real**:

- nome
- localização
- fronteira geográfica

Ele **não contém lógica social**.

> O território existe antes do app  
> e continua existindo mesmo sem usuários.

Essa decisão arquitetural evita:
- centralização indevida
- conflitos de poder
- confusão entre espaço físico e relações sociais

---

### 2. Vida social acontece em camadas separadas

Relações humanas como:
- moradores
- visitantes
- visibilidade de conteúdo
- regras de convivência
- moderação

**não pertencem ao território**.

Elas pertencem a **camadas sociais que referenciam o território**.

Isso torna o sistema:
- mais claro
- mais justo
- mais adaptável ao tempo

---

### 3. Tecnologia a serviço do território

O Araponga **não é**:
- um marketplace agressivo
- uma rede de engajamento infinito
- um produto de vigilância

Ele é uma **infraestrutura digital comunitária**, pensada para:

- autonomia local
- cuidado coletivo
- continuidade da vida no território
- fortalecimento do vínculo entre pessoas e lugar

---

## 🧩 Arquitetura (visão geral)

O backend segue princípios de **Clean Architecture**, com separação clara de responsabilidades:

backend/
├── Araponga.Api # API HTTP (controllers, endpoints)
├── Araponga.Application # Casos de uso / regras de aplicação
├── Araponga.Domain # Modelo de domínio (territory, regras centrais)
├── Araponga.Infrastructure # Persistência, integrações, adapters
├── Araponga.Shared # Tipos e utilitários compartilhados
└── Araponga.Tests # Testes automatizados


### Conceitos centrais do domínio

- **Territory**  
  Lugar físico real, neutro e persistente.

- **Membership**  
  Relação entre uma pessoa e um território (morador, visitante, etc.).

- **Feed / Map**  
  Informação contextual, sempre relacionada a um território específico.

---

## 🚀 Estado atual do projeto

- ✅ Backend inicial estruturado
- ✅ Descoberta e seleção de territórios
- ✅ Diferenciação entre morador e visitante
- ✅ Feed e mapa orientados ao território
- ✅ Testes automatizados
- ✅ CI configurado com builds reprodutíveis (`packages.lock.json`)
- 🚧 Frontend e experiências móveis em planejamento

O projeto está em **evolução ativa**, com foco em solidez antes de escala.

---

## 🛠️ Como rodar localmente

### Pré-requisitos
- .NET SDK 8.x
- Git

### Passos
```bash
git clone https://github.com/sraphaz/araponga.git
cd araponga
dotnet restore
dotnet build
dotnet test
dotnet run --project backend/Araponga.Api
```

A API ficará disponível conforme configurado no projeto (launchSettings.json).

### Portal de autosserviço

A página inicial da API (`/`) serve um portal estático com explicação do produto,
domínios, fluxos e quickstart. Em desenvolvimento, acesse também:

- `/swagger` (documentação da API)
- `/health` (status simples)

Para publicação como site estático, o portal também está disponível em `docs/` e
pode ser hospedado via GitHub Pages (basta apontar a origem para a pasta `docs`).
A versão do GitHub Pages inclui links diretos para documentação, user stories e changelog.

## 🤝 Contribuindo

Consulte o guia em [`CONTRIBUTING.md`](CONTRIBUTING.md).

O Araponga é aberto à colaboração, especialmente de pessoas interessadas em:

tecnologia com impacto social

comunidades locais

território, cultura e soberania

arquitetura de software consciente

regeneração e autonomia

Formas de contribuir:

código

testes

documentação

ideias

feedback conceitual

Antes de abrir PRs grandes, abra uma issue para alinharmos a direção.

## 🌎 Visão de futuro

Algumas direções possíveis (não promessas fechadas):

economias e moedas locais

trocas de serviços comunitários

governança distribuída

integração com iniciativas regenerativas

tecnologia como guardiã do território, não como exploradora

O Araponga não quer crescer rápido.
Quer criar raízes profundas.

✨ Uma nota pessoal

Este projeto nasce de uma escuta atenta:

da vida

do território

das comunidades

e dos limites do modelo digital atual

Se você chegou até aqui e sentiu que isso faz sentido,
você já faz parte da conversa.

## 📜 Licença

Este projeto é distribuído sob uma **licença aberta orientada à comunidade e ao território**.

- Versão oficial (EN): `LICENSE`
- Versão em português (PT-BR): `LICENSE.pt-BR`

🐦 Araponga canta para avisar, proteger e comunicar.
Que esta plataforma faça o mesmo.
