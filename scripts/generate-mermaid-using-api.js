/**
 * Script para gerar SVGs dos diagramas Mermaid usando API online (mermaid.ink)
 * Alternativa quando mermaid-cli não está disponível
 */

const fs = require('fs');
const path = require('path');
const https = require('https');

const DIAGRAMS_DIR = path.join(__dirname, '../backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams');

// Função para codificar diagrama em base64 (para mermaid.ink)
function encodeMermaid(diagramCode) {
  return Buffer.from(diagramCode).toString('base64').replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
}

// Paleta de cores do site Araponga (devportal)
const SITE_COLORS = {
  background: '#141a21', // --bg-card (um pouco mais claro que --bg para contraste)
  text: '#e8edf2', // --text
  textMuted: '#b8c5d2', // --text-muted
  accent: '#4dd4a8', // --accent (verde água)
  link: '#7dd3ff', // --link (azul claro)
  border: '#25303a', // --border
  activation: '#2a3a45' // Cor de ativação mais escura para contraste
};

// Configuração do tema customizado Mermaid
function getCustomThemeInit() {
  return `%%{init: {
    'theme': 'base',
    'themeVariables': {
      'primaryColor': '${SITE_COLORS.background}',
      'primaryTextColor': '${SITE_COLORS.text}',
      'primaryBorderColor': '${SITE_COLORS.border}',
      // Cores principais: verde água e azul claro para linhas/setas
      'lineColor': '${SITE_COLORS.link}', // Azul para linhas principais
      'secondaryColor': '${SITE_COLORS.background}',
      'tertiaryColor': '${SITE_COLORS.background}',
      'background': '${SITE_COLORS.background}',
      'mainBkg': '${SITE_COLORS.background}',
      'secondBkg': '${SITE_COLORS.background}',
      'textColor': '${SITE_COLORS.text}',
      'border1': '${SITE_COLORS.border}',
      'border2': '${SITE_COLORS.link}', // Azul para bordas destacadas
      // Setas e conexões: usar azul e verde água
      'arrowheadColor': '${SITE_COLORS.link}', // Azul para pontas de seta
      'arrowLineColor': '${SITE_COLORS.link}', // Azul para linhas de seta
      'signalColor': '${SITE_COLORS.accent}', // Verde água para sinais
      'signalTextColor': '${SITE_COLORS.text}',
      // Notas e caixas
      'noteBkgColor': '${SITE_COLORS.activation}',
      'noteTextColor': '${SITE_COLORS.text}',
      'noteBorderColor': '${SITE_COLORS.accent}', // Verde água para bordas de nota
      // Actores e participantes
      'actorBkg': '${SITE_COLORS.background}',
      'actorBorder': '${SITE_COLORS.link}', // Azul para bordas de ator
      'actorTextColor': '${SITE_COLORS.text}',
      'actorLineColor': '${SITE_COLORS.link}', // Azul para linhas de ator
      // Labels e caixas de texto
      'labelBoxBkgColor': '${SITE_COLORS.background}',
      'labelBoxBorderColor': '${SITE_COLORS.accent}', // Verde água para labels
      'labelTextColor': '${SITE_COLORS.textMuted}',
      'loopTextColor': '${SITE_COLORS.text}',
      // Ativações e seções
      'activationBorderColor': '${SITE_COLORS.link}', // Azul para bordas de ativação
      'activationBkgColor': '${SITE_COLORS.activation}',
      'sequenceNumberColor': '${SITE_COLORS.text}',
      'sectionBkgColor': '${SITE_COLORS.background}',
      'sectionBkgColor2': '${SITE_COLORS.activation}',
      'altSectionBkgColor': '${SITE_COLORS.background}',
      'excludeBkgColor': '${SITE_COLORS.activation}',
      // Escala de cores para variação
      'cScale0': '${SITE_COLORS.accent}', // Verde água
      'cScale1': '${SITE_COLORS.link}', // Azul claro
      'cScale2': '${SITE_COLORS.textMuted}'
    }
  }}%%

`;
}

// Função para fazer download do SVG com tema customizado
function downloadSVG(diagramCode, outputPath) {
  return new Promise((resolve, reject) => {
    // Adicionar tema customizado no início do código (se ainda não tiver)
    let enhancedCode = diagramCode;
    if (!enhancedCode.includes('%%{init:')) {
      enhancedCode = getCustomThemeInit() + diagramCode;
    }
    
    const encoded = encodeMermaid(enhancedCode);
    // bgColor define o fundo do SVG (mais claro que o fundo do site para contraste)
    const url = `https://mermaid.ink/svg/${encoded}?bgColor=${SITE_COLORS.background.replace('#', '')}`;
    
    const file = fs.createWriteStream(outputPath);
    
    https.get(url, (response) => {
      if (response.statusCode !== 200) {
        reject(new Error(`Erro ao baixar SVG: ${response.statusCode}`));
        return;
      }
      
      response.pipe(file);
      file.on('finish', () => {
        file.close();
        resolve();
      });
    }).on('error', (err) => {
      fs.unlink(outputPath, () => {}); // Remove arquivo parcial
      reject(err);
    });
  });
}

// Ler todos os arquivos .mmd
const mmdFiles = fs.readdirSync(DIAGRAMS_DIR).filter(f => f.endsWith('.mmd'));

console.log(`📊 Gerando ${mmdFiles.length} SVGs usando API online (mermaid.ink)...`);

let generated = 0;
let failed = 0;

async function generateAll() {
  for (const mmdFile of mmdFiles) {
    const mmdPath = path.join(DIAGRAMS_DIR, mmdFile);
    const diagramCode = fs.readFileSync(mmdPath, 'utf8');
    const svgPath = mmdPath.replace('.mmd', '.svg');
    const diagramId = path.basename(mmdFile, '.mmd');
    
    process.stdout.write(`   ${diagramId}... `);
    
    try {
      await downloadSVG(diagramCode, svgPath);
      console.log('✅');
      generated++;
      
      // Aguardar um pouco para não sobrecarregar a API
      await new Promise(resolve => setTimeout(resolve, 500));
    } catch (err) {
      console.log(`❌ ${err.message}`);
      failed++;
    }
  }
  
  console.log(`\n✅ ${generated}/${mmdFiles.length} SVGs gerados com sucesso!`);
  if (failed > 0) {
    console.log(`⚠️  ${failed} falharam. Use mermaid-cli como alternativa.`);
  }
}

generateAll().catch(console.error);