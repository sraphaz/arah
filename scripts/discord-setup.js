const { Client, GatewayIntentBits, PermissionFlagsBits, ChannelType } = require('discord.js');
const readline = require('readline');

// Configuração dos canais
const channelConfig = [
    {
        categoryName: '🟢 Entrada e Boas-Vindas',
        channels: [
            {
                name: 'sala-pública',
                description: 'Entrada principal, apresentações, perguntas iniciais',
                type: ChannelType.GuildText,
                public: true
            }
        ]
    },
    {
        categoryName: '💬 Comunicação Geral',
        channels: [
            {
                name: 'geral',
                description: 'Discussões gerais sobre o projeto, anúncios',
                type: ChannelType.GuildText,
                public: true
            }
        ]
    },
    {
        categoryName: '👨‍💻 Desenvolvimento',
        channels: [
            {
                name: 'desenvolvedores',
                description: 'Espaço para desenvolvedores discutirem implementação, código, arquitetura',
                type: ChannelType.GuildText,
                public: false
            },
            {
                name: 'desenvolvimento-geral',
                description: 'Discussões técnicas gerais, arquitetura, planejamento técnico',
                type: ChannelType.GuildText,
                public: false
            }
        ]
    },
    {
        categoryName: '👁️ Análise Funcional',
        channels: [
            {
                name: 'analistas-funcionais',
                description: 'Discussões sobre necessidades territoriais, observação, análise funcional',
                type: ChannelType.GuildText,
                public: true
            },
            {
                name: 'propostas-funcionais',
                description: 'Apresentar, debater e refinar propostas de funcionalidades',
                type: ChannelType.GuildText,
                public: true
            }
        ]
    },
    {
        categoryName: '🌍 Comunidade',
        channels: [
            {
                name: 'feedback-comunidade',
                description: 'Compartilhe sua experiência de uso, reporte problemas, sugira melhorias',
                type: ChannelType.GuildText,
                public: true
            }
        ]
    }
];

// Mensagem de boas-vindas
const welcomeMessage = `🌟 **Bem-vindo ao Araponga!**

Este é um espaço para construção colaborativa de uma plataforma digital comunitária orientada ao território.

📚 **Para começar:**
1. Leia: \`docs/ONBOARDING_PUBLICO.md\`
2. Escolha seu caminho (Desenvolvedor ou Analista Funcional)
3. Explore outras salas conforme interesse
4. Apresente-se e comece a contribuir!

💡 **Principais salas:**
- #geral - Discussões gerais
- #desenvolvedores - Espaço técnico
- #analistas-funcionais - Espaço funcional
- #propostas-funcionais - Discussão de propostas
- #feedback-comunidade - Feedback de uso

🤝 **Valores:**
- Território como referência
- Autonomia territorial
- Tecnologia a serviço da vida
- Decolonização digital

Perguntas? Sinta-se à vontade para perguntar aqui ou explorar outras salas!`;

// Função para pedir input do usuário
function askQuestion(query) {
    const rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout
    });

    return new Promise(resolve => {
        rl.question(query, answer => {
            rl.close();
            resolve(answer);
        });
    });
}

async function setupDiscord() {
    console.log('🚀 Iniciando setup do Discord do Araponga...\n');

    // Tentar obter token de variável de ambiente primeiro, senão pedir
    let token = process.env.DISCORD_BOT_TOKEN;
    if (token) {
        console.log('✅ Token encontrado em variável de ambiente.');
        console.log(`   Token (primeiros 10 chars): ${token.substring(0, 10)}...`);
        console.log(`   Comprimento: ${token.length} caracteres\n`);
    } else {
        console.log('📝 Token não encontrado em variável de ambiente.');
        console.log('   Vamos pedir o token interativamente...\n');
        console.log('💡 Como obter o token:');
        console.log('   1. Acesse: https://discord.com/developers/applications');
        console.log('   2. Selecione sua aplicação (ou crie uma nova)');
        console.log('   3. Vá em "Bot" > "Reset Token" ou "Copy"');
        console.log('   4. Cole o token abaixo\n');
        
        token = await askQuestion('🔑 Cole o token do bot: ');
        token = token.trim();
        
        if (!token) {
            console.error('\n❌ Token não pode estar vazio!');
            process.exit(1);
        }
        
        console.log(`\n✅ Token recebido. Comprimento: ${token.length} caracteres`);
    }

    // Validar formato básico do token
    console.log('\n🔍 Validando token...');
    if (token.length < 50) {
        console.error('❌ Erro: Token parece muito curto!');
        console.error('   Tokens do Discord geralmente têm 59+ caracteres.');
        console.error('   Verifique se copiou o token completo.');
        process.exit(1);
    }

    // Verificar se tem formato básico (tipo.id.secret)
    const tokenParts = token.split('.');
    if (tokenParts.length !== 3) {
        console.warn('⚠️  Aviso: Token não parece ter o formato padrão (tipo.id.secret)');
        console.warn('   Continuação... (pode falhar na autenticação)');
    } else {
        console.log(`✅ Token parece válido (tipo: ${tokenParts[0]}, ID: ${tokenParts[1]})`);
    }

    // Tentar obter Guild ID de variável de ambiente primeiro, senão pedir
    let guildId = process.env.DISCORD_GUILD_ID;
    if (!guildId) {
        console.log('\n📝 Guild ID não encontrado em variável de ambiente.');
        console.log('   Vamos pedir o ID do servidor interativamente...\n');
        console.log('💡 Como obter o Guild ID:');
        console.log('   1. No Discord, ative "Modo Desenvolvedor":');
        console.log('      Configurações do Usuário > Avançado > Modo de Desenvolvedor');
        console.log('   2. Clique com botão direito no NOME do servidor (ícone)');
        console.log('   3. Clique em "Copiar ID"');
        console.log('   4. Cole o ID abaixo\n');
        
        guildId = await askQuestion('🏠 Cole o ID do servidor (Guild ID): ');
        guildId = guildId.trim();
        
        if (!guildId) {
            console.error('\n❌ Guild ID não pode estar vazio!');
            process.exit(1);
        }
    }

    // Validar formato básico do Guild ID (deve ser numérico)
    console.log('\n🔍 Validando Guild ID...');
    if (!/^\d+$/.test(guildId)) {
        console.error('❌ Erro: Guild ID deve conter apenas números!');
        console.error(`   ID fornecido: ${guildId}`);
        console.error('   Verifique se copiou o ID correto.');
        process.exit(1);
    }
    console.log(`✅ Guild ID validado: ${guildId}`);

    const client = new Client({
        intents: [
            GatewayIntentBits.Guilds,
            GatewayIntentBits.GuildMessages
            // MessageContent não é necessário para criar canais, apenas para ler conteúdo de mensagens
            // GatewayIntentBits.MessageContent
        ]
    });

    // Adicionar event listeners para debug
    client.on('ready', () => {
        console.log('✅ Bot conectado e pronto!');
        console.log(`   Bot: ${client.user.tag} (${client.user.id})`);
    });

    client.on('error', (error) => {
        console.error('\n❌ Erro no cliente Discord:');
        console.error(`   Tipo: ${error.name}`);
        console.error(`   Mensagem: ${error.message}`);
        if (error.code) {
            console.error(`   Código: ${error.code}`);
        }
    });

    client.on('warn', (warning) => {
        console.warn(`\n⚠️  Aviso: ${warning}`);
    });

    try {
        console.log('\n🔄 Conectando ao Discord...');
        console.log(`   Token (primeiros 10 chars): ${token.substring(0, 10)}...`);
        console.log(`   Token (comprimento): ${token.length} caracteres`);
        console.log(`   Token (formato): ${token.split('.').length} partes separadas por ponto`);
        
        // Tentar login com timeout
        console.log('   Tentando fazer login...');
        
        const loginPromise = client.login(token);
        const timeoutPromise = new Promise((_, reject) => 
            setTimeout(() => reject(new Error('Timeout ao conectar (mais de 10 segundos)')), 10000)
        );
        
        await Promise.race([loginPromise, timeoutPromise]);
        console.log('✅ Login bem-sucedido!');
        
        // Aguardar um pouco para garantir que o bot está pronto
        console.log('   Aguardando bot ficar pronto...');
        await new Promise(resolve => setTimeout(resolve, 2000));

        console.log('\n🔍 Buscando servidor...');
        console.log(`   Guild ID: ${guildId}`);
        
        const guild = await client.guilds.fetch(guildId);
        console.log(`✅ Servidor encontrado: ${guild.name}`);
        console.log(`   ID: ${guild.id}`);
        console.log(`   Membros: ${guild.memberCount}`);
        console.log(`   Canais existentes: ${guild.channels.cache.size}`);

        // Verificar permissões do bot
        console.log('\n🔐 Verificando permissões do bot...');
        try {
            const botMember = await guild.members.fetch(client.user.id);
            const botPermissions = botMember.permissions;
            
            if (!botPermissions) {
                console.warn('⚠️  Não foi possível verificar permissões do bot.');
                console.warn('   Continuando mesmo assim...');
            } else {
                const requiredPermissions = [
                    PermissionFlagsBits.ViewChannels,
                    PermissionFlagsBits.ManageChannels,
                    PermissionFlagsBits.SendMessages,
                    PermissionFlagsBits.ManageMessages,
                    PermissionFlagsBits.ReadMessageHistory,
                    PermissionFlagsBits.AddReactions
                ].filter(perm => perm !== undefined && perm !== null);
                
                if (requiredPermissions.length === 0) {
                    console.warn('⚠️  Nenhuma permissão válida encontrada para verificar.');
                    console.warn('   Continuando mesmo assim...');
                } else {
                    const missingPermissions = requiredPermissions.filter(
                        perm => perm !== undefined && perm !== null && !botPermissions.has(perm)
                    );
                    
                    if (missingPermissions.length > 0) {
                        console.error('❌ Bot não tem todas as permissões necessárias!');
                        console.error('   Permissões faltando:');
                        missingPermissions.forEach(perm => {
                            if (perm !== undefined && perm !== null) {
                                const permName = Object.keys(PermissionFlagsBits).find(key => PermissionFlagsBits[key] === perm);
                                console.error(`   - ${permName || 'Desconhecida'}`);
                            }
                        });
                        console.error('\n💡 Solução:');
                        console.error('   1. Vá em Configurações do Servidor > Integrações');
                        console.error('   2. Encontre o bot na lista');
                        console.error('   3. Ative todas as permissões necessárias');
                        console.error('   4. Certifique-se de que o bot está acima de outros membros na hierarquia de roles');
                        throw new Error('Permissões insuficientes');
                    }
                    console.log('✅ Bot tem todas as permissões necessárias!');
                }
            }
        } catch (permError) {
            if (permError.message === 'Permissões insuficientes') {
                throw permError; // Re-lançar se for erro de permissões
            }
            console.warn('⚠️  Não foi possível verificar permissões do bot.');
            console.warn(`   Erro: ${permError.message}`);
            console.warn('   Continuando mesmo assim...');
        }

        // Criar categorias e canais
        console.log('\n📁 Criando categorias e canais...');
        console.log(`   Total de categorias: ${channelConfig.length}`);
        const createdChannels = {};

        for (let i = 0; i < channelConfig.length; i++) {
            const categoryConfig = channelConfig[i];
            console.log(`\n[${i + 1}/${channelConfig.length}] Processando: ${categoryConfig.categoryName}`);
            
            // Verificar se categoria já existe
            const existingCategory = guild.channels.cache.find(
                ch => ch.type === ChannelType.GuildCategory && ch.name === categoryConfig.categoryName
            );
            
            if (existingCategory) {
                console.log(`⚠️  Categoria já existe: ${categoryConfig.categoryName}`);
                console.log(`   Usando categoria existente...`);
                var category = existingCategory;
            } else {
                // Criar categoria
                console.log(`   Criando categoria...`);
                category = await guild.channels.create({
                    name: categoryConfig.categoryName,
                    type: ChannelType.GuildCategory,
                    reason: 'Setup automático do Araponga'
                });
                console.log(`✅ Categoria criada: ${categoryConfig.categoryName} (ID: ${category.id})`);
            }

            // Criar canais dentro da categoria
            for (let j = 0; j < categoryConfig.channels.length; j++) {
                const channelInfo = categoryConfig.channels[j];
                console.log(`   [${j + 1}/${categoryConfig.channels.length}] Processando canal: #${channelInfo.name}`);
                
                // Verificar se canal já existe
                const existingChannel = guild.channels.cache.find(
                    ch => ch.type === channelInfo.type && ch.name === channelInfo.name && ch.parentId === category.id
                );
                
                if (existingChannel) {
                    console.log(`   ⚠️  Canal já existe: #${channelInfo.name}`);
                    console.log(`   Usando canal existente...`);
                    createdChannels[channelInfo.name] = existingChannel;
                    continue;
                }

                // Criar canal
                console.log(`   Criando canal...`);
                const channel = await guild.channels.create({
                    name: channelInfo.name,
                    type: channelInfo.type,
                    parent: category.id,
                    topic: channelInfo.description,
                    reason: 'Setup automático do Araponga'
                });
                console.log(`   ✅ Canal criado: #${channelInfo.name} (ID: ${channel.id})`);

                // Configurar permissões
                console.log(`   Configurando permissões...`);
                if (channelInfo.public) {
                    // Canal público: todos podem ler e escrever
                    await channel.permissionOverwrites.edit(guild.roles.everyone, {
                        ViewChannel: true,
                        SendMessages: true,
                        ReadMessageHistory: true
                    });
                    console.log(`   ✅ Permissões configuradas: Público (todos podem ler/escrever)`);
                } else {
                    // Canal não público: todos podem ler, mas escrever é opcional (deixar todos escreverem por enquanto)
                    await channel.permissionOverwrites.edit(guild.roles.everyone, {
                        ViewChannel: true,
                        SendMessages: true,
                        ReadMessageHistory: true
                    });
                    console.log(`   ✅ Permissões configuradas: Leitura pública, escrita permitida`);
                }

                createdChannels[channelInfo.name] = channel;
                
                // Pequeno delay para evitar rate limits
                await new Promise(resolve => setTimeout(resolve, 500));
            }
        }

        // Enviar mensagem de boas-vindas e fixar
        if (createdChannels['sala-pública']) {
            console.log('\n📝 Criando mensagem de boas-vindas...');
            
            // Verificar se já existe mensagem fixada
            const pinnedMessages = await createdChannels['sala-pública'].messages.fetchPinned();
            if (pinnedMessages.size > 0) {
                console.log('⚠️  Já existe mensagem fixada no canal. Pulando criação de nova mensagem...');
                console.log(`   (Se quiser atualizar, delete a mensagem fixada existente e execute o script novamente)`);
            } else {
                const message = await createdChannels['sala-pública'].send(welcomeMessage);
                await message.pin();
                console.log(`✅ Mensagem de boas-vindas criada e fixada! (ID: ${message.id})`);
            }
        } else {
            console.warn('\n⚠️  Canal #sala-pública não encontrado. Não foi possível criar mensagem de boas-vindas.');
        }

        // Criar roles opcionais (comentado por padrão - descomente se quiser)
        /*
        console.log('\n👥 Criando roles opcionais...');
        const roles = [
            { name: 'Desenvolvedor', color: '#5865F2', mentionable: true },
            { name: 'Analista Funcional', color: '#57F287', mentionable: true },
            { name: 'Comunidade', color: '#FEE75C', mentionable: false }
        ];

        for (const roleInfo of roles) {
            const role = await guild.roles.create({
                name: roleInfo.name,
                color: roleInfo.color,
                mentionable: roleInfo.mentionable,
                reason: 'Setup automático do Araponga'
            });
            console.log(`  ✅ Role criada: @${roleInfo.name}`);
        }
        */

        console.log('\n🎉 Setup concluído com sucesso!');
        console.log('═══════════════════════════════════════════');
        console.log('\n📊 Resumo:');
        console.log(`   - Categorias criadas: ${channelConfig.length}`);
        const totalChannels = channelConfig.reduce((sum, cat) => sum + cat.channels.length, 0);
        console.log(`   - Canais criados/verificados: ${totalChannels}`);
        console.log(`   - Mensagem de boas-vindas: ${createdChannels['sala-pública'] ? '✅ Criada' : '❌ Não criada'}`);
        console.log('\n📋 Próximos passos:');
        console.log('1. ✅ Verifique se todos os canais foram criados corretamente');
        console.log('2. ⚙️  Ajuste permissões se necessário (manual)');
        console.log('3. 🤖 Adicione bots opcionais se desejar (MEE6, Dyno, etc.)');
        console.log('4. 👥 Convide pessoas para o servidor!');
        
        if (createdChannels['sala-pública']) {
            console.log('\n💡 Criando links de convite...');
            try {
                // Link permanente (nunca expira)
                const permanentInvite = await createdChannels['sala-pública'].createInvite({
                    maxAge: 0, // 0 = nunca expira
                    maxUses: 0, // 0 = ilimitado
                    temporary: false,
                    reason: 'Link permanente do Discord do Araponga'
                });
                console.log('✅ Link permanente criado:');
                console.log(`   ${permanentInvite.url}`);
                
                // Também criar link temporário (7 dias) como backup
                try {
                    const tempInvite = await createdChannels['sala-pública'].createInvite({
                        maxAge: 604800, // 7 dias
                        maxUses: 0,
                        temporary: false,
                        reason: 'Link temporário de backup'
                    });
                    console.log('\n📋 Link temporário (backup, válido por 7 dias):');
                    console.log(`   ${tempInvite.url}`);
                } catch (tempError) {
                    console.warn('   ⚠️  Não foi possível criar link temporário de backup (não crítico)');
                }
                
                console.log('\n💡 Use o link permanente na documentação!');
            } catch (inviteError) {
                console.error('   ❌ Não foi possível criar link de convite automaticamente');
                console.error(`   Erro: ${inviteError.message}`);
                console.error('\n💡 Solução manual:');
                console.error('   1. Vá em Configurações do Servidor > Convites');
                console.error('   2. Clique em "Criar Convite"');
                console.error('   3. Configure: Sem data de expiração, Sem limite de usos');
                console.error('   4. Copie o link e atualize a documentação');
            }
        }
        
        console.log('\n═══════════════════════════════════════════');

    } catch (error) {
        console.error('\n❌ Erro durante o setup!');
        console.error('═══════════════════════════════════════════');
        console.error(`Tipo: ${error.name || 'Error'}`);
        console.error(`Mensagem: ${error.message}`);
        
        if (error.code) {
            console.error(`Código Discord: ${error.code}`);
        }
        
        if (error.stack) {
            console.error('\n📚 Stack trace:');
            console.error(error.stack);
        }
        
        // Tratamento específico de erros
        console.error('\n🔍 Diagnóstico:');
        
        if (error.code === 50001 || error.code === 50013) {
            console.error('❌ Permissões Insuficientes!');
            console.error('\n💡 Solução:');
            console.error('   1. Vá em Configurações do Servidor > Integrações');
            console.error('   2. Encontre seu bot na lista');
            console.error('   3. Ative as seguintes permissões:');
            console.error('      - Ver Canais');
            console.error('      - Gerenciar Canais');
            console.error('      - Enviar Mensagens');
            console.error('      - Gerenciar Mensagens');
            console.error('      - Ler Histórico de Mensagens');
            console.error('      - Fixar Mensagens');
            console.error('   4. Certifique-se de que o bot está acima de outros membros na hierarquia de roles');
        } else if (error.code === 10004) {
            console.error('❌ Guild/Servidor Não Encontrado!');
            console.error('\n💡 Solução:');
            console.error('   1. Verifique se o Guild ID está correto');
            console.error(`   ID fornecido: ${guildId || 'não definido'}`);
            console.error('   2. Verifique se o bot está adicionado ao servidor');
            console.error('   3. Como obter o Guild ID correto:');
            console.error('      - Ative Modo Desenvolvedor no Discord');
            console.error('      - Clique com botão direito no nome do servidor');
            console.error('      - Clique em "Copiar ID"');
        } else if (error.message && (error.message.includes('401') || error.message.includes('Unauthorized')) || error.code === 40001 || error.code === 50001) {
            console.error('❌ Token Inválido ou Expirado!');
            console.error(`\n📊 Informações do Token:`);
            console.error(`   Comprimento: ${token ? token.length : 'N/A'} caracteres`);
            console.error(`   Primeiros chars: ${token ? token.substring(0, 15) + '...' : 'N/A'}`);
            console.error(`   Formato: ${token ? token.split('.').length : 'N/A'} partes`);
            console.error(`   Erro específico: ${error.message}`);
            console.error(`   Código: ${error.code || 'N/A'}`);
            console.error('\n💡 Solução:');
            console.error('   1. Verifique se copiou o token completo (sem espaços extras no início/fim)');
            console.error('   2. Tokens do Discord geralmente têm 59+ caracteres');
            console.error('   3. Verifique se há caracteres invisíveis (copie novamente)');
            console.error('   4. Vá em https://discord.com/developers/applications');
            console.error('   5. Selecione sua aplicação');
            console.error('   6. Vá em "Bot" > "Reset Token" (ou "Copy" se já existe)');
            console.error('   7. Certifique-se de copiar TODO o token (geralmente algo como: MTA...123.456...789)');
            console.error('   8. Cole novamente (sem espaços extras)');
            console.error('\n🔍 Debug:');
            console.error('   - Token deve ter 3 partes separadas por ponto (.)');
            console.error('   - Exemplo formato: tipo.id.secret');
            console.error('   - Verifique se não copiou token de outra aplicação');
            console.error('\n🔒 Segurança:');
            console.error('   - Se o token foi comprometido, resete-o imediatamente');
            console.error('   - Nunca compartilhe o token publicamente');
        } else if (error.message && error.message.includes('429')) {
            console.error('❌ Rate Limit Excedido!');
            console.error('\n💡 Solução:');
            console.error('   - Aguarde alguns minutos e tente novamente');
            console.error('   - O Discord limita ações por minuto');
        } else if (error.message && error.message.includes('500') || error.message.includes('502') || error.message.includes('503')) {
            console.error('❌ Erro do Servidor Discord!');
            console.error('\n💡 Solução:');
            console.error('   - O Discord pode estar com problemas temporários');
            console.error('   - Aguarde alguns minutos e tente novamente');
            console.error('   - Verifique o status: https://status.discord.com/');
        } else if (error.message && error.message.includes('disallowed intents') || error.message.includes('Used disallowed intents')) {
            console.error('❌ Intents Não Habilitados no Discord!');
            console.error('\n💡 Solução:');
            console.error('   O bot precisa ter os intents habilitados no Discord Developer Portal.');
            console.error('\n📝 Passo a passo:');
            console.error('   1. Acesse: https://discord.com/developers/applications');
            console.error('   2. Selecione sua aplicação');
            console.error('   3. Vá em "Bot" (menu lateral)');
            console.error('   4. Role até a seção "Privileged Gateway Intents"');
            console.error('   5. Ative os seguintes intents:');
            console.error('      ✅ "MESSAGE CONTENT INTENT" (se você precisar ler conteúdo de mensagens)');
            console.error('      ⚠️  Para este script, você NÃO precisa ativar Message Content Intent');
            console.error('   6. Salve as mudanças');
            console.error('   7. Execute o script novamente');
            console.error('\n💡 Nota:');
            console.error('   - O script foi atualizado para NÃO usar MessageContent Intent');
            console.error('   - Isso não é necessário para criar canais');
            console.error('   - Se ainda der erro, verifique se ativou os intents corretos');
        } else {
            console.error('\n💡 Diagnóstico Geral:');
            console.error('   1. Verifique se o token está correto');
            console.error('   2. Verifique se o Guild ID está correto');
            console.error('   3. Verifique se o bot está adicionado ao servidor');
            console.error('   4. Verifique se o bot tem as permissões necessárias');
            console.error('   5. Verifique a conexão com a internet');
            console.error('   6. Verifique se os intents estão habilitados no Discord Developer Portal');
            console.error('\n📖 Para mais ajuda, veja: scripts/discord-setup-guide.md');
        }
        
        console.error('═══════════════════════════════════════════');
        
        // Tentar destruir o cliente mesmo em caso de erro
        try {
            if (client && client.user) {
                await client.destroy();
            }
        } catch (destroyError) {
            // Ignorar erros ao destruir
        }
        
        process.exit(1);
    } finally {
        // Garantir que o cliente seja destruído
        try {
            if (client && client.user) {
                await client.destroy();
                console.log('\n🔌 Cliente desconectado.');
            }
        } catch (destroyError) {
            // Ignorar erros ao destruir
        }
    }
}

// Executar
setupDiscord().catch(console.error);
