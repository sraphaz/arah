# Wiki Araponga

Frontend da Wiki do Araponga - Documentação completa da plataforma.

## 🎯 Sobre

Este é o frontend da Wiki do Araponga, construído com Next.js 15, TypeScript e Tailwind CSS. Utiliza a mesma identidade visual do portal principal, com design elevado e foco em legibilidade e organização.

## 🚀 Desenvolvimento

```bash
# Instalar dependências
npm install

# Rodar em desenvolvimento (porta 3001)
npm run dev

# Build para produção
npm run build

# Iniciar servidor de produção
npm start
```

## 📁 Estrutura

- `app/` - Páginas e layouts Next.js
- `app/page.tsx` - Página inicial (índice da documentação)
- `app/docs/[slug]/page.tsx` - Páginas dinâmicas para documentos individuais
- `app/docs/page.tsx` - Lista completa de todos os documentos
- `app/globals.css` - Estilos globais com identidade visual do Araponga
- `app/layout.tsx` - Layout raiz

## 🎨 Identidade Visual

O wiki utiliza a mesma identidade visual do portal principal:
- Paleta Forest (verdes)
- Glass cards com backdrop blur
- Watermark do logo Araponga
- Tipografia Sora (variável)
- Design limpo e elevado

## 📚 Documentação

Os documentos são carregados dinamicamente da pasta `docs/` na raiz do projeto. A estrutura de navegação é definida em `app/page.tsx` e `app/docs/page.tsx`.

## 🌐 Deploy

O wiki é deployado automaticamente via **GitHub Actions** para **GitHub Pages** no domínio `wiki.araponga.app`.

### CI/CD Automatizado

- ✅ Build e testes em cada PR
- ✅ Deploy automático em push para `main`
- ✅ CNAME configurado automaticamente para `wiki.araponga.app`

Ver **[CI_CD.md](./CI_CD.md)** para documentação completa do pipeline.

### Deploy Manual (Alternativo)

O wiki também pode ser deployado em:
- **Vercel** (recomendado para Next.js)
- **Netlify**
- **Qualquer servidor Node.js**

## 🔗 Links

- **Wiki**: https://wiki.araponga.app (quando deployado)
- **Portal**: https://araponga.app
- **Repositório**: https://github.com/sraphaz/araponga
