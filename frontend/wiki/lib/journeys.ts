/**
 * Sistema de Jornadas Guiadas para Wiki
 * Define caminhos recomendados por perfil de usuário
 */

export interface JourneyStep {
  doc: string;
  label: string;
  description?: string;
}

export interface Journey {
  title: string;
  description: string;
  steps: JourneyStep[];
}

export const journeys: Record<string, Journey> = {
  developer: {
    title: 'Desenvolvedor',
    description: 'Caminho recomendado para desenvolvedores que querem integrar ou contribuir com o Araponga',
    steps: [
      {
        doc: 'ONBOARDING_DEVELOPERS',
        label: '1. Começar',
        description: 'Configuração inicial e primeiros passos',
      },
      {
        doc: '12_DOMAIN_MODEL',
        label: '2. Entender Domínio',
        description: 'Modelo de domínio completo e relacionamentos',
      },
      {
        doc: '11_ARCHITECTURE_SERVICES',
        label: '3. Explorar Serviços',
        description: 'Arquitetura de services e casos de uso',
      },
      {
        doc: '10_ARCHITECTURE_DECISIONS',
        label: '4. Decisões Arquiteturais',
        description: 'ADRs e padrões estabelecidos',
      },
      {
        doc: '21_CODE_REVIEW',
        label: '5. Padrões de Código',
        description: 'Convenções e melhores práticas',
      },
      {
        doc: '22_COHESION_AND_TESTS',
        label: '6. Testes e Qualidade',
        description: 'Estrutura de testes e cobertura',
      },
      {
        doc: '41_CONTRIBUTING',
        label: '7. Contribuir',
        description: 'Como contribuir para o projeto',
      },
    ],
  },
  analyst: {
    title: 'Analista Funcional',
    description: 'Caminho recomendado para analistas que querem entender a lógica de negócio e funcionalidades',
    steps: [
      {
        doc: 'ONBOARDING_ANALISTAS_FUNCIONAIS',
        label: '1. Começar',
        description: 'Onboarding completo para analistas',
      },
      {
        doc: '01_PRODUCT_VISION',
        label: '2. Entender Visão',
        description: 'Visão estratégica e princípios do produto',
      },
      {
        doc: '60_API_LÓGICA_NEGÓCIO',
        label: '3. Lógica de Negócio',
        description: 'Documentação completa da API e regras de negócio',
      },
      {
        doc: '04_USER_STORIES',
        label: '4. Ver User Stories',
        description: 'Histórias de usuário e casos de uso',
      },
      {
        doc: '27_USER_JOURNEYS_MAP',
        label: '5. Jornadas de Usuário',
        description: 'Mapa completo de jornadas e fluxos',
      },
    ],
  },
  manager: {
    title: 'Gestor',
    description: 'Caminho recomendado para gestores que querem entender a estratégia e roadmap',
    steps: [
      {
        doc: 'ONBOARDING_PUBLICO',
        label: '1. Visão Geral',
        description: 'Visão geral do projeto e propósito',
      },
      {
        doc: '01_PRODUCT_VISION',
        label: '2. Visão do Produto',
        description: 'Princípios, valores e estratégia',
      },
      {
        doc: '02_ROADMAP',
        label: '3. Roadmap',
        description: 'Planejamento e próximas fases',
      },
      {
        doc: '03_BACKLOG',
        label: '4. Backlog',
        description: 'Funcionalidades planejadas e priorizadas',
      },
      {
        doc: '39_ESTRATEGIA_CONVERGENCIA_MERCADO',
        label: '5. Estratégia de Mercado',
        description: 'Convergência e alinhamento com mercado',
      },
    ],
  },
};

/**
 * Obtém jornada por ID
 */
export function getJourney(id: string): Journey | undefined {
  return journeys[id];
}

/**
 * Obtém todas as jornadas
 */
export function getAllJourneys(): Journey[] {
  return Object.values(journeys);
}

/**
 * Obtém IDs de todas as jornadas
 */
export function getJourneyIds(): string[] {
  return Object.keys(journeys);
}
