#!/usr/bin/env node
/**
 * Agent Graph MCP server (read-only, zero dependências).
 *
 * Expõe o grafo operacional do Arah (docs/_meta/agent-graph.generated.json) via
 * Model Context Protocol sobre stdio (JSON-RPC 2.0, mensagens delimitadas por
 * newline). Não escreve nada, não executa agentes: só leitura/explicação.
 *
 * Recursos:
 *   - arah://agent-graph        → o JSON completo do grafo
 * Tools:
 *   - agents_for_path(path)     → rules/agentes/skills acionados por um path
 *   - explain_path(path)        → explicação legível de "por que" foi acionado
 *   - skills_for_agent(agent)   → skills que um agente pode invocar
 *
 * Uso (registro no cliente MCP, ex. .cursor/mcp.json):
 *   { "mcpServers": { "arah-agent-graph": {
 *       "command": "node",
 *       "args": ["scripts/agents/agent-graph-mcp.mjs"] } } }
 */
import { readFileSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import { dirname, join } from 'node:path';
import { createInterface } from 'node:readline';

const PROTOCOL_VERSION = '2024-11-05';
const __dirname = dirname(fileURLToPath(import.meta.url));
const ROOT = join(__dirname, '..', '..');
const GRAPH_PATH = join(ROOT, 'docs', '_meta', 'agent-graph.generated.json');
const GRAPH_URI = 'arah://agent-graph';

function loadGraph() {
  return JSON.parse(readFileSync(GRAPH_PATH, 'utf8'));
}

// glob -> RegExp (mesma semântica de choreograph-agents.ps1: **, **/, *)
function globToRegex(glob) {
  const g = String(glob).replace(/\\/g, '/');
  if (g === '**' || g === '**/*') return /^.*$/;
  // Escapa metacaracteres regex (inclui '\'); '*' é tratado depois como glob.
  let re = g.replace(/[.+^${}()|[\]\\]/g, '\\$&');
  // Sentinelas para '**/' e '**' antes de expandir '*', senão o '*' inserido por
  // '**' (.* ) seria reprocessado por '*'->[^/]* (bug de ordem).
  re = re.replace(/\*\*\//g, '\u0000');
  re = re.replace(/\*\*/g, '\u0001');
  re = re.replace(/\*/g, '[^/]*');
  re = re.replace(/\u0000/g, '(?:.*/)?');
  re = re.replace(/\u0001/g, '.*');
  return new RegExp('^' + re + '$');
}

function matchRulesForPath(graph, path) {
  const norm = String(path).replace(/\\/g, '/');
  const matched = [];
  for (const rule of graph.nodes.rules || []) {
    // Regras disparadas por evento (ex.: pr-always, when: pull_request) não são
    // acionadas por path fora de um PR; espelha o gating de choreograph-agents.ps1
    // para não reportar qa/pr-steward em qualquer caminho local.
    if (rule.when === 'pull_request') continue;
    const hit = (rule.paths || []).some((p) => globToRegex(p).test(norm));
    if (hit) matched.push(rule);
  }
  return matched;
}

function agentsForPath(graph, path) {
  const rules = matchRulesForPath(graph, path);
  const operational = new Set();
  const domain = new Set();
  const skills = new Set();
  for (const rule of rules) {
    for (const a of rule.agents || []) {
      if (a.kind === 'domain') domain.add(a.id);
      else operational.add(a.id);
      for (const sk of a.skills || []) skills.add(sk);
    }
  }
  return {
    path,
    matched_rules: rules.map((r) => r.id),
    operational_agents: [...operational].sort(),
    domain_consults: [...domain].sort(),
    skills: [...skills].sort(),
  };
}

function explainPath(graph, path) {
  const r = agentsForPath(graph, path);
  const lines = [];
  if (r.matched_rules.length === 0) {
    lines.push(`Nenhuma rule de coreografia casa com "${path}".`);
    lines.push('A rule "pr-always" (paths: **) ainda ativa qa + pr-steward em qualquer PR.');
  } else {
    lines.push(`"${path}" casa com: ${r.matched_rules.join(', ')}.`);
    if (r.operational_agents.length) lines.push(`Ativa agentes operacionais: ${r.operational_agents.join(', ')}.`);
    if (r.domain_consults.length) lines.push(`Consulta domínios (parecer): ${r.domain_consults.join(', ')}.`);
    if (r.skills.length) lines.push(`Skills declaradas: ${r.skills.join(', ')}.`);
  }
  lines.push('Pipeline de revisão: ci/qa/bot-review → pr-steward → human-review (merge humano).');
  return { ...r, explanation: lines.join('\n') };
}

function skillsForAgent(graph, agentId) {
  const skills = new Set();
  for (const e of graph.edges || []) {
    if (e.type === 'may_invoke_skill' && e.from === `agent:${agentId}`) {
      skills.add(e.to.replace(/^skill:/, ''));
    }
  }
  const agent = (graph.nodes.agents || []).find((a) => a.id === agentId);
  return {
    agent: agentId,
    exists: Boolean(agent),
    kind: agent ? agent.kind : null,
    skills: [...skills].sort(),
  };
}

const TOOLS = [
  {
    name: 'agents_for_path',
    description: 'Lista rules, agentes operacionais, consultas de domínio e skills acionados por um path.',
    inputSchema: {
      type: 'object',
      properties: { path: { type: 'string', description: 'Caminho de arquivo (ex.: backend/Arah.Core/Foo.cs)' } },
      required: ['path'],
    },
  },
  {
    name: 'explain_path',
    description: 'Explica em linguagem natural por que agentes/skills são acionados por um path.',
    inputSchema: {
      type: 'object',
      properties: { path: { type: 'string' } },
      required: ['path'],
    },
  },
  {
    name: 'skills_for_agent',
    description: 'Lista as skills que um agente pode invocar, segundo o grafo.',
    inputSchema: {
      type: 'object',
      properties: { agent: { type: 'string', description: 'Id do agente (ex.: backend)' } },
      required: ['agent'],
    },
  },
];

function toolResult(obj) {
  return { content: [{ type: 'text', text: JSON.stringify(obj, null, 2) }] };
}

function handle(msg) {
  const { id, method, params } = msg;
  switch (method) {
    case 'initialize':
      return {
        protocolVersion: PROTOCOL_VERSION,
        capabilities: { tools: {}, resources: {} },
        serverInfo: { name: 'arah-agent-graph', version: '1.0.0' },
      };
    case 'ping':
      return {};
    case 'resources/list':
      return {
        resources: [
          { uri: GRAPH_URI, name: 'Arah Agent Graph', description: 'Grafo operacional gerado', mimeType: 'application/json' },
        ],
      };
    case 'resources/read': {
      if (params?.uri !== GRAPH_URI) throw { code: -32602, message: `unknown resource: ${params?.uri}` };
      const text = readFileSync(GRAPH_PATH, 'utf8');
      return { contents: [{ uri: GRAPH_URI, mimeType: 'application/json', text }] };
    }
    case 'tools/list':
      return { tools: TOOLS };
    case 'tools/call': {
      const graph = loadGraph();
      const name = params?.name;
      const args = params?.arguments || {};
      if (name === 'agents_for_path') return toolResult(agentsForPath(graph, args.path));
      if (name === 'explain_path') return toolResult(explainPath(graph, args.path));
      if (name === 'skills_for_agent') return toolResult(skillsForAgent(graph, args.agent));
      throw { code: -32602, message: `unknown tool: ${name}` };
    }
    default:
      throw { code: -32601, message: `method not found: ${method}` };
  }
}

function send(obj) {
  process.stdout.write(JSON.stringify(obj) + '\n');
}

function serve() {
  const rl = createInterface({ input: process.stdin });
  rl.on('line', (line) => {
    const trimmed = line.trim();
    if (!trimmed) return;
    let msg;
    try {
      msg = JSON.parse(trimmed);
    } catch {
      return; // ignora linhas não-JSON
    }
    // Notificações (sem id) não recebem resposta.
    if (msg.id === undefined || msg.id === null) return;
    try {
      send({ jsonrpc: '2.0', id: msg.id, result: handle(msg) });
    } catch (err) {
      const error = err && err.code ? err : { code: -32603, message: String(err?.message || err) };
      send({ jsonrpc: '2.0', id: msg.id, error });
    }
  });
}

// Loop stdio só quando executado direto; importado (testes) apenas expõe funções.
const isMain = process.argv[1] && fileURLToPath(import.meta.url) === process.argv[1];
if (isMain) serve();

export { globToRegex, matchRulesForPath, agentsForPath, explainPath };
