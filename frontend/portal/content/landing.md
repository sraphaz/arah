# ARAPONGA

**Território-Primeiro & Comunidade-Primeiro**  
Plataforma orientada ao território para organização comunitária local

---

## O Pássaro que nos inspira

O Araponga, também conhecido como **“pássaro-ferreiro”**, é famoso por seu canto metálico e ressonante nas florestas brasileiras.  
Sua presença vibrante simboliza a força e a resiliência da natureza.

A escolha do Araponga reflete nosso compromisso com a autenticidade e a comunicação clara.  
Assim como ele se destaca em seu território, buscamos valorizar as comunidades locais e suas vozes singulares.

---

## O Problema

### O ruído da escala global dificulta o contexto local

#### Relevância diluída
Plataformas generalistas priorizam alcance global, tornando difícil descobrir informações verdadeiramente relevantes para o entorno imediato onde vivemos e atuamos.

#### Falta de contexto territorial
Ferramentas existentes não reconhecem adequadamente as especificidades de cada território — suas regras, cultura, dinâmicas de confiança e formas de organização comunitária.

#### Dependência de gigantes
Comunidades locais ficam reféns de algoritmos opacos e políticas de plataformas centralizadas que não atendem às necessidades específicas do território.

---

## A proposta: território primeiro, comunidade primeiro

### Orientado ao território
Cada instância representa um território específico, com suas próprias regras de visibilidade, governança e organização comunitária.

### Feed local + mapa integrado
Publicações e eventos aparecem tanto em timeline quanto em visualização geográfica, facilitando a descoberta do que acontece por perto.

### Governança explícita
Regras claras sobre quem pode ver e participar, com distinção entre visitantes e residentes, respeitando a autonomia comunitária.

O Araponga é uma plataforma de **código aberto** projetada para fortalecer a organização comunitária a partir do território.  
Ela combina feed social, mapeamento colaborativo e regras de participação definidas pela própria comunidade.

O objetivo é promover relevância local, reduzir ruído informacional e diminuir a dependência de plataformas generalistas que não atendem às especificidades de cada lugar.

---

## Como funciona na prática

O fluxo de uso foi desenhado para ser direto e respeitar as regras de cada território desde o primeiro acesso.

### 01 — Login e autenticação
Usuário faz login via sistema de autenticação.  
A identidade é verificada, mas ainda não há vínculo com um território específico.

### 02 — Escolher um território
Usuário seleciona o território de interesse na plataforma.  
Cada território possui regras próprias de participação e visibilidade.

### 03 — Visitante ou Residente
O sistema atribui o papel inicial:
- **Visitante**: acesso limitado  
- **Residente**: acesso ampliado  

A residência pode ser solicitada conforme regras locais.

### 04 — Feed e mapa filtrados
O conteúdo exibido respeita o papel do usuário.  
Visitantes veem apenas conteúdo público.  
Residentes acessam conteúdos restritos.  

Tudo aparece tanto no **feed** quanto no **mapa**.

---

## Arquitetura

### Domínios principais da plataforma

O Araponga organiza-se em domínios funcionais que trabalham de forma integrada.  
Cada domínio possui responsabilidades claras e se relaciona com os demais para garantir uma experiência completa orientada ao território.

#### Território
Define a unidade geográfica e suas regras.  
Cada território possui limites, governança e políticas próprias de participação.

#### Vínculo
Gerencia papéis (**visitor / resident**) e permissões.  
Controla quem pode acessar o quê dentro de cada território.

#### Feed
Publicações e eventos organizados em timeline.  
Inclui filtros por tipo, data e relevância territorial.  
Eventos possuem data e hora associadas.

#### Mapa
Visualização geográfica de conteúdos.  
Permite explorar publicações e eventos espacialmente dentro do território.

**Planejado:** Media + GeoAnchor — sistema de mídia georreferenciada para documentar lugares, memórias e histórias do território.  
Ainda em desenvolvimento.

---

## Regras de visibilidade: por que isso importa

### Visitante & Residente
A distinção entre visitante e residente permite que comunidades controlem o acesso a informações sensíveis ou restritas.

- **Visitante**: acesso apenas a conteúdo marcado como público  
- **Residente**: acesso ampliado, incluindo conteúdos restritos do território  

Essa separação respeita a autonomia comunitária e permite gradações de confiança baseadas em engajamento real.

### Público & Restrito
Cada publicação, evento ou conteúdo pode ser classificado como público ou restrito.

O sistema garante que apenas usuários com permissão adequada tenham acesso a informações sensíveis.  
Isso é essencial para:
- proteger discussões internas  
- planejar ações comunitárias sem exposição desnecessária  
- construir ambientes de confiança  

Essas regras não são arbitrárias.  
Elas promovem **segurança, relevância e respeito às dinâmicas locais**.

---

## Valor gerado para comunidades e parceiros

### Relevância local garantida
Informações filtradas por território eliminam ruído global e priorizam o que realmente importa para quem vive ali.

### Construção de confiança
Regras explícitas de visibilidade e governança transparente promovem ambientes seguros para organização comunitária.

### Organização facilitada
Feed + mapa integrados simplificam a coordenação de eventos, mobilizações e iniciativas locais sem depender de plataformas externas.

### Custo reduzido de curadoria
Filtros territoriais e regras locais diminuem a necessidade de moderação centralizada intensiva, distribuindo responsabilidade.

---

## Tecnologia

### Arquitetura técnica: domínio primeiro, evolução incremental

O Araponga é construído com **.NET** e segue princípios de **Domain-Driven Design (DDD)**.  
A arquitetura prioriza domínios de negócio bem definidos e evolução incremental, permitindo adicionar funcionalidades sem comprometer a base existente.

### Componentes principais
- **API RESTful**: expõe recursos via HTTP, documentada com Swagger / OpenAPI  
- **Portal de autosserviço**: interface web para interação com feed, mapa e configurações  
- **Persistência**: banco relacional PostgreSQL com suporte a dados geográficos  
- **Autenticação e autorização**: baseada em papéis e políticas territoriais  
- **Cache distribuído**: Redis com fallback automático para cache em memória  
- **Processamento assíncrono**: eventos processados em background com retry e dead letter queue

### Performance e escalabilidade
- **Concorrência otimista**: RowVersion em entidades críticas para prevenir conflitos  
- **Cache distribuído**: Redis para alta disponibilidade e performance  
- **Processamento assíncrono**: eventos não bloqueiam requisições HTTP  
- **Read replicas**: suporte a réplicas de leitura para escalar queries  
- **Deployment multi-instância**: preparado para load balancing e escalabilidade horizontal  

### Observabilidade e monitoramento
- **Logs centralizados**: Serilog com Seq para análise e busca de logs estruturados  
- **Métricas Prometheus**: métricas HTTP, de negócio e de sistema expostas via `/metrics`  
- **Distributed tracing**: OpenTelemetry para rastreamento de requisições e queries  
- **Dashboards e alertas**: documentação completa para monitoramento em produção  
- **Runbook e troubleshooting**: procedimentos operacionais e resolução de problemas documentados  

### Princípios de design
- Domínio primeiro  
- Evolução incremental  
- API-first  
- Código aberto  

---

## Roadmap: evolução em três fases

### Fase 1 — MVP sólido  
**Status:** Em desenvolvimento

- Território e vínculos funcionais  
- Feed de publicações e eventos  
- Mapa básico  
- Regras de visibilidade  
- API documentada + portal web  

### Fase 2 — Mídia georreferenciada  
**Status:** Planejado

- Sistema Media + GeoAnchor  
- Upload de fotos e vídeos associados a locais  
- Galeria territorial de memórias  
- Marcadores visuais no mapa  

### Fase 3 — Experiências avançadas  
**Status:** Visão de longo prazo

- Plataforma de produtos e serviços territoriais  
- Integração com dados abertos  
- Ferramentas de análise comunitária  
- Indicadores ambientais do território  

---

## O Futuro

### O futuro é ancestral, local, descentralizado e inevitável

A transformação já começou.  
Comunidades estão retomando o controle sobre suas formas de organização, comunicação e governança.

Plataformas centralizadas que ignoram contextos locais estão perdendo relevância onde mais importa: **no território**.

O Araponga não é apenas uma ferramenta.  
É parte de um movimento maior em direção à autonomia comunitária, transparência e relevância local.

---

## Faça parte dessa transformação

- Explore o código e a documentação no GitHub  
- Teste a plataforma em sua comunidade  
- Contribua com ideias, código ou feedback  

**Território primeiro. Comunidade primeiro. Vida primeiro.**

---

## Apoie o desenvolvimento

O Araponga é um projeto de código aberto e conta com o apoio da comunidade para continuar crescendo.

Sua contribuição ajuda a:
- manter a infraestrutura  
- desenvolver novas funcionalidades  
- alcançar mais comunidades  

**PIX:** rapha.sos@gmail.com
