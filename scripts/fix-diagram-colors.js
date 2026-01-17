/**
 * Script para corrigir cores dos diagramas SVG gerados
 * Substitui cores hardcoded pelas cores do tema Araponga
 */

const fs = require('fs');
const path = require('path');

const DIAGRAMS_DIR = path.join(__dirname, '../backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams');

// Cores do tema Araponga
const COLORS = {
  background: '#141a21',      // Fundo cinza escuro
  text: '#ffffff',            // Texto branco puro para melhor contraste
  textMuted: '#e8edf2',       // Texto cinza claro
  accent: '#4dd4a8',          // Verde água
  link: '#7dd3ff',            // Azul claro
  border: '#25303a',          // Borda cinza
  actorBg: '#1a2129',         // Fundo dos atores (um pouco mais claro)
  actorBorder: '#7dd3ff',     // Borda dos atores (azul)
  actorLine: '#7dd3ff',       // Linha dos atores (azul)
  actorText: '#ffffff',       // Texto dos atores (branco puro para máximo contraste)
  messageLine: '#7dd3ff',      // Linhas de mensagem (azul)
  arrowhead: '#7dd3ff',       // Pontas de seta (azul)
  labelBoxBorder: '#4dd4a8',   // Borda de labels (verde água)
  loopLine: '#4dd4a8',        // Linhas de loop (verde água)
  noteBorder: '#4dd4a8',      // Borda de notas (verde água)
  noteBg: '#2a3a45',          // Fundo de notas
  activationBg: '#2a3a45',    // Fundo de ativação
  activationBorder: '#7dd3ff'  // Borda de ativação (azul)
};

// Função para converter RGB para hex
function rgbToHex(rgb) {
  const match = rgb.match(/rgb\((\d+),\s*(\d+),\s*(\d+)\)/);
  if (!match) return null;
  const r = parseInt(match[1]).toString(16).padStart(2, '0');
  const g = parseInt(match[2]).toString(16).padStart(2, '0');
  const b = parseInt(match[3]).toString(16).padStart(2, '0');
  return `#${r}${g}${b}`;
}

// Função para corrigir cores no SVG
function fixSVGColors(svgContent) {
  let fixed = svgContent;
  
  // 1. Corrigir fundo (background-color no style do SVG)
  fixed = fixed.replace(/background-color:\s*rgb\(20,\s*26,\s*33\)/g, `background-color: ${COLORS.background}`);
  
  // 2. Corrigir atores (fill e stroke)
  fixed = fixed.replace(/fill="#eaeaea"/g, `fill="${COLORS.actorBg}"`);
  fixed = fixed.replace(/stroke="#666"/g, `stroke="${COLORS.actorBorder}"`);
  
  // 3. Corrigir linhas de ator
  fixed = fixed.replace(/class="actor-line[^"]*"[^>]*stroke="#999"/g, (match) => {
    return match.replace(/stroke="#999"/, `stroke="${COLORS.actorLine}"`);
  });
  fixed = fixed.replace(/stroke-width="0\.5px"\s+stroke="#999"/g, `stroke-width="0.5px" stroke="${COLORS.actorLine}"`);
  
  // 4. Corrigir linhas de mensagem
  fixed = fixed.replace(/class="messageLine0"[^>]*stroke="#333"/g, (match) => {
    return match.replace(/stroke="#333"/, `stroke="${COLORS.messageLine}"`);
  });
  fixed = fixed.replace(/class="messageLine1"[^>]*stroke="#333"/g, (match) => {
    return match.replace(/stroke="#333"/, `stroke="${COLORS.messageLine}"`);
  });
  fixed = fixed.replace(/stroke-width="2"[^>]*stroke="none"/g, `stroke-width="2" stroke="${COLORS.messageLine}"`);
  
  // 5. Corrigir pontas de seta (arrowhead)
  fixed = fixed.replace(/#arrowhead path[^}]*fill:#333[^}]*stroke:#333/g, (match) => {
    return match.replace(/fill:#333/g, `fill:${COLORS.arrowhead}`).replace(/stroke:#333/g, `stroke:${COLORS.arrowhead}`);
  });
  fixed = fixed.replace(/\.marker[^}]*fill:#333333[^}]*stroke:#333333/g, (match) => {
    return match.replace(/fill:#333333/g, `fill:${COLORS.arrowhead}`).replace(/stroke:#333333/g, `stroke:${COLORS.arrowhead}`);
  });
  
  // 6. Corrigir textos (incluindo textos dos atores inline)
  fixed = fixed.replace(/fill:black/g, `fill:${COLORS.text}`);
  fixed = fixed.replace(/fill="#000"/g, `fill="${COLORS.text}"`);
  fixed = fixed.replace(/fill="#333"/g, `fill="${COLORS.text}"`);
  
  // 6.1. Corrigir textos dos atores inline (tspan dentro de .actor-box)
  fixed = fixed.replace(/<text[^>]*class="actor[^"]*"[^>]*><tspan[^>]*>/g, (match) => {
    // Se o tspan não tem fill já definido, não precisamos adicionar (será controlado pelo CSS)
    return match;
  });
  
  // 6.2. Garantir que textos dos atores usam cor clara (substituir qualquer cor escura inline)
  fixed = fixed.replace(/<tspan([^>]*)fill="#[0-9a-f]{3,6}"([^>]*)>(.*?)<\/tspan>/gi, (match, before, after, content) => {
    // Se estiver dentro de um ator, usar cor clara
    const beforeMatch = match.match(/class="actor[^"]*"/);
    if (beforeMatch) {
      return `<tspan${before}fill="${COLORS.actorText}"${after}>${content}</tspan>`;
    }
    return match;
  });
  
  // 7. Corrigir labelBox (bordas verdes)
  fixed = fixed.replace(/class="labelBox"[^>]*stroke:hsl\([^)]+\)/g, (match) => {
    return match.replace(/stroke:hsl\([^)]+\)/, `stroke:${COLORS.labelBoxBorder}`);
  });
  
  // 8. Corrigir loopLine (verde água)
  fixed = fixed.replace(/class="loopLine"[^>]*stroke:hsl\([^)]+\)/g, (match) => {
    return match.replace(/stroke:hsl\([^)]+\)/, `stroke:${COLORS.loopLine}`);
  });
  
  // 9. Corrigir note (bordas verdes)
  fixed = fixed.replace(/\.note[^}]*stroke:#aaaa33/g, `stroke:${COLORS.noteBorder}`);
  fixed = fixed.replace(/\.note[^}]*fill:#fff5ad/g, `fill:${COLORS.noteBg}`);
  
  // 10. Corrigir activation (fundo e borda)
  fixed = fixed.replace(/\.activation0[^}]*fill:#f4f4f4[^}]*stroke:#666/g, (match) => {
    return match.replace(/fill:#f4f4f4/, `fill:${COLORS.activationBg}`).replace(/stroke:#666/, `stroke:${COLORS.activationBorder}`);
  });
  fixed = fixed.replace(/\.activation1[^}]*fill:#f4f4f4[^}]*stroke:#666/g, (match) => {
    return match.replace(/fill:#f4f4f4/, `fill:${COLORS.activationBg}`).replace(/stroke:#666/, `stroke:${COLORS.activationBorder}`);
  });
  fixed = fixed.replace(/\.activation2[^}]*fill:#f4f4f4[^}]*stroke:#666/g, (match) => {
    return match.replace(/fill:#f4f4f4/, `fill:${COLORS.activationBg}`).replace(/stroke:#666/, `stroke:${COLORS.activationBorder}`);
  });
  
  // 11. Adicionar estilos CSS customizados no final do <style>
  const styleEnd = fixed.indexOf('</style>');
  if (styleEnd > -1) {
    const customStyles = `
/* Cores customizadas Araponga */
#mermaid-svg .messageLine0 { stroke: ${COLORS.messageLine} !important; }
#mermaid-svg .messageLine1 { stroke: ${COLORS.messageLine} !important; }
#mermaid-svg .actor-line { stroke: ${COLORS.actorLine} !important; }
#mermaid-svg .actor { fill: ${COLORS.actorBg} !important; stroke: ${COLORS.actorBorder} !important; }
#mermaid-svg .labelBox { stroke: ${COLORS.labelBoxBorder} !important; }
#mermaid-svg .loopLine { stroke: ${COLORS.loopLine} !important; }
#mermaid-svg .note { stroke: ${COLORS.noteBorder} !important; fill: ${COLORS.noteBg} !important; }
#mermaid-svg .activation0, #mermaid-svg .activation1, #mermaid-svg .activation2 { 
  fill: ${COLORS.activationBg} !important; 
  stroke: ${COLORS.activationBorder} !important; 
}
#mermaid-svg #arrowhead path, #mermaid-svg .marker { 
  fill: ${COLORS.arrowhead} !important; 
  stroke: ${COLORS.arrowhead} !important; 
}
#mermaid-svg .messageText, #mermaid-svg .labelText, #mermaid-svg .loopText { 
  fill: ${COLORS.text} !important; 
}
#mermaid-svg text.actor > tspan, 
#mermaid-svg .actor-box tspan, 
#mermaid-svg .actor tspan,
#mermaid-svg text[class*="actor"] tspan { 
  fill: ${COLORS.actorText} !important; 
  color: ${COLORS.actorText} !important;
}
`;
    fixed = fixed.slice(0, styleEnd) + customStyles + fixed.slice(styleEnd);
  }
  
  return fixed;
}

// Processar todos os SVGs
const svgFiles = fs.readdirSync(DIAGRAMS_DIR).filter(f => f.endsWith('.svg'));

console.log(`🎨 Corrigindo cores de ${svgFiles.length} diagramas SVG...\n`);

let fixed = 0;
for (const svgFile of svgFiles) {
  const svgPath = path.join(DIAGRAMS_DIR, svgFile);
  const originalContent = fs.readFileSync(svgPath, 'utf8');
  const fixedContent = fixSVGColors(originalContent);
  
  if (originalContent !== fixedContent) {
    fs.writeFileSync(svgPath, fixedContent, 'utf8');
    console.log(`   ✅ ${svgFile}`);
    fixed++;
  } else {
    console.log(`   ⏭️  ${svgFile} (sem mudanças)`);
  }
}

console.log(`\n✅ ${fixed}/${svgFiles.length} diagramas corrigidos!`);
