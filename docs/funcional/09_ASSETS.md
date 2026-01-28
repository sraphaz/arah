# Assets - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](funcional/00_PLATAFORMA_ARAPONGA.md)

---

## 🎯 Visão Geral

**TerritoryAssets** representam recursos valiosos do território que pertencem ao próprio território (naturais, culturais, comunitários, infraestruturais, simbólicos). **NÃO são vendáveis** e não devem ser tratados como produtos ou serviços.

### Objetivo

Permitir que usuários:
- **Cadastrem recursos** territoriais valiosos
- **Visualizem assets** no mapa
- **Validem assets** (curadores)
- **Referenciem assets** em posts/eventos

---

## 💼 Função de Negócio

### Para o Usuário

- Cadastrar recursos territoriais (trilhas, nascentes, pontos culturais)
- Visualizar assets validados no mapa
- Referenciar assets em posts/eventos

### Para a Comunidade

- **Registro**: Catalogar recursos valiosos do território
- **Preservação**: Documentar patrimônio territorial
- **Descoberta**: Facilitar descoberta de recursos

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### TerritoryAsset
- **Propósito**: Recurso territorial valioso
- **Atributos**: Nome, descrição, tipo, geolocalização obrigatória
- **Status**: PENDING, VALIDATED
- **Características**: Não vendável, não transferível

---

## ⚙️ Regras de Negócio

1. **Permissão**: Apenas moradores verificados ou curadores podem criar
2. **Geolocalização**: Obrigatória (pelo menos um GeoAnchor)
3. **Validação**: Apenas curadores podem validar
4. **Visibilidade**: Apenas assets validados são retornados
5. **Não vendável**: Assets não podem ser vendidos via marketplace

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](funcional/00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Marketplace](funcional/06_MARKETPLACE.md)** - Diferenciação: Assets não são vendáveis
- **[Mapa Territorial](funcional/05_MAPA_TERRITORIAL.md)** - Assets aparecem no mapa
- **[API - Assets](api/60_08_API_ASSETS.md)** - Documentação técnica

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
