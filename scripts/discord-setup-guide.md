# 🤖 Guia de Setup Automatizado do Discord

**Guia Passo a Passo para Configurar o Discord do Araponga usando Script Automatizado**

**Versão**: 1.0  
**Data**: 2025-01-20

---

## 📋 Pré-requisitos

1. **Servidor Discord criado** ✅ (você já fez isso!)
2. **Comunidade habilitada** ✅ (você já fez isso!)
3. **Node.js instalado** (versão 16+)
   - Verificar: `node --version`
   - Baixar: https://nodejs.org/
4. **Bot criado e adicionado ao servidor** (ver abaixo)

---

## 🤖 Passo 1: Criar um Bot no Discord

### 1.1 Criar Aplicação

1. Acesse: https://discord.com/developers/applications
2. Clique em **"New Application"**
3. Dê um nome: **"Araponga Setup Bot"** ou similar
4. Clique em **"Create"**

### 1.2 Criar Bot

1. No menu lateral, clique em **"Bot"**
2. Clique em **"Add Bot"**
3. Clique em **"Yes, do it!"**
4. **Desmarque** "Public Bot" (se não quiser que apareça publicamente)
5. **Marque** as seguintes opções:
   - ✅ "Message Content Intent" (importante para o bot funcionar)
   - ❌ "Public Bot" (deixe desmarcado se não quiser bot público)

### 1.3 Obter Token do Bot

1. Ainda na seção **"Bot"**, encontre **"Token"**
2. Clique em **"Reset Token"** ou **"Copy"** (se já existir)
3. **COPIE O TOKEN** - você vai precisar dele!
4. ⚠️ **IMPORTANTE**: Mantenha o token seguro! Não compartilhe publicamente.

---

## 🔐 Passo 2: Obter ID do Servidor (Guild ID)

### Método 1: Via Interface do Discord

1. No Discord, ative o **Modo Desenvolvedor**:
   - Vá em **Configurações do Usuário** (⚙️)
   - Vá em **Avançado**
   - Ative **"Modo de Desenvolvedor"**

2. Com o modo desenvolvedor ativado:
   - Clique com botão direito no **nome do seu servidor** (ícone do servidor)
   - Clique em **"Copiar ID"**
   - Este é o **Guild ID** que você precisa!

### Método 2: Via URL

1. Abra o Discord no navegador
2. Abra seu servidor
3. A URL será algo como: `https://discord.com/channels/123456789012345678/...`
4. O número `123456789012345678` é o **Guild ID**

---

## 🔑 Passo 3: Adicionar Bot ao Servidor

### 3.1 Configurar OAuth2

1. No Discord Developer Portal, vá em **"OAuth2" > "URL Generator"**
2. Em **"SCOPES"**, marque:
   - ✅ `bot`
   - ✅ `applications.commands` (opcional, para comandos slash)
3. Em **"BOT PERMISSIONS"**, marque:
   - ✅ `View Channels`
   - ✅ `Manage Channels`
   - ✅ `Send Messages`
   - ✅ `Manage Messages`
   - ✅ `Read Message History`
   - ✅ `Add Reactions`
   - ✅ `Use External Emojis`
   - ✅ `Attach Files`
   - ✅ `Embed Links`
   - ✅ `Pin Messages`
4. Uma URL será gerada automaticamente na parte inferior

### 3.2 Autorizar Bot

1. **Copie a URL gerada**
2. **Cole no navegador** e pressione Enter
3. Selecione seu servidor (Araponga)
4. Clique em **"Autorizar"**
5. Complete o captcha se aparecer
6. ✅ Bot adicionado ao servidor!

**Verificação**: O bot deve aparecer na lista de membros do servidor (offline, mas presente).

### 3.3 Verificar Permissões do Bot no Servidor

1. No Discord, vá em **Configurações do Servidor** > **Integrações**
2. Encontre seu bot na lista
3. Verifique se todas as permissões estão ativadas:
   - Gerenciar Canais
   - Enviar Mensagens
   - Fixar Mensagens
   - Ler Histórico de Mensagens

**Importante**: O bot precisa estar **acima** de outros membros na hierarquia de roles. Se necessário:
- Vá em **Configurações do Servidor** > **Roles**
- Mova o bot para uma posição mais alta na hierarquia

---

## 📦 Passo 4: Instalar Dependências

1. Abra terminal na pasta raiz do projeto (`araponga`)
2. Execute:

```bash
npm install discord.js
```

**Verificação**: Deve instalar sem erros. Se houver problemas:
- Verifique se Node.js está instalado: `node --version`
- Verifique se npm está instalado: `npm --version`
- Se não tiver npm, instale Node.js que vem com npm

---

## 🚀 Passo 5: Executar o Script

### Windows PowerShell

```powershell
# Configure variáveis de ambiente
$env:DISCORD_BOT_TOKEN="seu-token-do-bot-aqui"
$env:DISCORD_GUILD_ID="id-do-seu-servidor-aqui"

# Execute o script
node scripts/discord-setup.js
```

### Windows CMD

```cmd
set DISCORD_BOT_TOKEN=seu-token-do-bot-aqui
set DISCORD_GUILD_ID=id-do-seu-servidor-aqui
node scripts/discord-setup.js
```

### Linux/macOS

```bash
export DISCORD_BOT_TOKEN="seu-token-do-bot-aqui"
export DISCORD_GUILD_ID="id-do-seu-servidor-aqui"
node scripts/discord-setup.js
```

### Exemplo Completo

```bash
# Substitua pelos seus valores reais!
export DISCORD_BOT_TOKEN="MTIzNDU2Nzg5MDEyMzQ1Njc4OTAbC.ExAmPlE.1234567890abcdefghijklmnopqrstuvwxyz"
export DISCORD_GUILD_ID="123456789012345678"
node scripts/discord-setup.js
```

**Nota**: O token e o ID devem estar entre aspas se tiverem caracteres especiais.

---

## ✅ Passo 6: Verificar Resultado

O script deve mostrar:

```
🔄 Conectando ao Discord...
✅ Conectado ao Discord!
📡 Conectado ao servidor: [Nome do Servidor]

📁 Criando categorias e canais...
✅ Categoria criada: 🟢 Entrada e Boas-Vindas
  ✅ Canal criado: #sala-pública
✅ Categoria criada: 💬 Comunicação Geral
  ✅ Canal criado: #geral
... (e assim por diante)

📝 Criando mensagem de boas-vindas...
✅ Mensagem de boas-vindas criada e fixada!

🎉 Setup concluído com sucesso!

💡 Link de convite (válido por 7 dias):
   https://discord.gg/xxxxx
```

**Verifique no Discord**:
- ✅ Todas as categorias foram criadas?
- ✅ Todos os canais foram criados?
- ✅ Mensagem de boas-vindas está fixada em `#sala-pública`?
- ✅ Permissões estão corretas (todos podem ler/escrever nos públicos)?

---

## 🐛 Solução de Problemas

### Erro: "DISCORD_BOT_TOKEN não encontrado"

**Solução**: Configure a variável de ambiente antes de executar o script.

```bash
# Verifique se está configurado
echo $DISCORD_BOT_TOKEN  # Linux/macOS
echo $env:DISCORD_BOT_TOKEN  # PowerShell
```

### Erro: "Missing Permissions" ou Código 50013

**Solução**: 
1. Verifique se o bot tem todas as permissões necessárias no servidor
2. Verifique se o bot tem cargo com permissões adequadas
3. O bot precisa estar **acima** dos canais na hierarquia de roles
4. Vá em **Configurações do Servidor** > **Roles** e mova o bot para cima

### Erro: "Guild Not Found" ou Código 10004

**Solução**: 
1. Verifique se o Guild ID está correto
2. Verifique se o bot está adicionado ao servidor
3. Tente copiar o ID novamente (pode ter copiado errado)

### Erro: "Invalid Token" ou "401 Unauthorized"

**Solução**: 
1. Verifique se copiou o token corretamente (sem espaços extras)
2. Se necessário, crie um novo token em "Bot" > "Reset Token"
3. Certifique-se de que o token está entre aspas se tiver caracteres especiais

### Erro: "Cannot find module 'discord.js'"

**Solução**:
```bash
# Instale as dependências
npm install discord.js
```

### Bot não aparece no servidor

**Solução**: 
1. Refaça o processo de autorização OAuth2 (Passo 3)
2. Certifique-se de selecionar o servidor correto
3. Verifique se completou o captcha

### Canais não aparecem ou estão vazios

**Solução**:
1. Verifique se o bot tem permissão "Gerenciar Canais"
2. Verifique se o bot tem permissão "Ver Canais"
3. Tente executar o script novamente (pode ter havido erro parcial)

---

## 📝 Próximos Passos Após Setup

### 1. Verificar Configuração

- [ ] Todas as categorias criadas?
- [ ] Todos os canais criados?
- [ ] Mensagem de boas-vindas fixada?
- [ ] Permissões corretas?

### 2. Ajustar Manualmente (Se Necessário)

- [ ] Ajustar permissões de canais específicos
- [ ] Criar roles adicionais (opcional)
- [ ] Adicionar regras no canal `#regras` (opcional)

### 3. Adicionar Bots Opcionais

- [ ] MEE6 ou Dyno (auto-moderação)
- [ ] GitHub bot (notificações de Issues/PRs)
- [ ] Outros bots conforme necessidade

### 4. Atualizar Documentação

- [ ] Atualizar `docs/ONBOARDING_PUBLICO.md` com link do Discord
- [ ] Atualizar `docs/CARTILHA_COMPLETA.md` com informações do Discord
- [ ] Adicionar link no README.md (se necessário)

### 5. Convidar Membros

- [ ] Use o link de convite gerado pelo script
- [ ] Ou crie um link permanente em **Configurações do Servidor** > **Widget** ou **Convites**

---

## 🔒 Segurança

⚠️ **NUNCA**:
- Compartilhe o token do bot publicamente
- Commit o token no Git
- Use o token em código público
- Deixe o token em arquivos de configuração não ignorados

✅ **SEMPRE**:
- Use variáveis de ambiente
- Mantenha o token privado
- Se o token vazar, resete-o imediatamente em "Bot" > "Reset Token"
- Adicione `.env` ao `.gitignore` se usar arquivo `.env`

---

## 💡 Dicas

### Criar Link Permanente de Convite

1. No Discord, vá em **Configurações do Servidor** > **Widget**
2. Ative o widget
3. Copie o link do widget
4. Ou crie um convite manual e configure para nunca expirar

### Adicionar Roles Automaticamente

Se quiser criar roles automaticamente, descomente a seção no script (`scripts/discord-setup.js`):

```javascript
// Descomente esta seção:
console.log('\n👥 Criando roles opcionais...');
const roles = [
    { name: 'Desenvolvedor', color: '#5865F2', mentionable: true },
    { name: 'Analista Funcional', color: '#57F287', mentionable: true },
    { name: 'Comunidade', color: '#FEE75C', mentionable: false }
];
// ... resto do código
```

### Usar Arquivo .env (Opcional)

Crie um arquivo `.env` na raiz do projeto:

```env
DISCORD_BOT_TOKEN=seu-token-aqui
DISCORD_GUILD_ID=id-do-servidor-aqui
```

Instale `dotenv`:
```bash
npm install dotenv
```

Modifique o script para usar:
```javascript
require('dotenv').config();
```

**Lembre-se**: Adicione `.env` ao `.gitignore`!

---

## 📚 Referências

- **Discord Developer Portal**: https://discord.com/developers/applications
- **Discord.js Documentation**: https://discord.js.org/
- **Guia Completo de Setup**: `docs/DISCORD_SETUP.md`
- **Discord Support**: https://support.discord.com/

---

## 🌱 Conclusão

Com este script, você configurou automaticamente:
- ✅ Categorias organizadas
- ✅ Canais essenciais
- ✅ Permissões configuradas
- ✅ Mensagem de boas-vindas fixada
- ✅ Link de convite gerado

**Agora você pode começar a usar o Discord do Araponga!**

**Dúvidas?** Abra uma Issue no GitHub ou pergunte no Discord!

---

**Última Atualização**: 2025-01-20  
**Versão**: 1.0
