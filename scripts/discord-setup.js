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
    },
    {
        categoryName: '🌐 Canais Sociais',
        channels: [
            {
                name: 'apresentações',
                description: 'Apresente-se para a comunidade! Conte um pouco sobre você e seu interesse no projeto',
                type: ChannelType.GuildText,
                public: true
            },
            {
                name: 'off-topic',
                description: 'Discussões casuais, tópicos gerais, conversas livres sobre diversos assuntos',
                type: ChannelType.GuildText,
                public: true
            },
            {
                name: 'celebrações',
                description: 'Celebre conquistas, marcos do projeto, contribuições destacadas e momentos importantes',
                type: ChannelType.GuildText,
                public: true
            },
            {
                name: 'territórios',
                description: 'Compartilhe experiências territoriais, histórias de comunidades, conexões locais',
                type: ChannelType.GuildText,
                public: true
            },
            {
                name: 'recursos',
                description: 'Compartilhe recursos úteis: ferramentas, artigos, eventos relacionados, aprendizado',
                type: ChannelType.GuildText,
                public: true
            }
        ]
    }
];

// Mensagem de boas-vindas
const welcomeMessage = `🌟 **Bem-vindo ao Araponga**

Espaço para construção colaborativa de uma plataforma digital comunitária orientada ao território.

📚 **Para começar**
1. Leia: https://devportal.araponga.app/wiki/docs/ONBOARDING_PUBLICO
2. Escolha seu caminho (Desenvolvedor ou Analista Funcional)
3. Explore outras salas conforme interesse
4. Apresente-se e contribua

💡 **Principais salas**
- #geral - Discussões gerais
- #desenvolvedores - Espaço técnico
- #analistas-funcionais - Espaço funcional
- #propostas-funcionais - Discussão de propostas
- #feedback-comunidade - Feedback de uso
- #apresentações - Apresente-se para a comunidade
- #territórios - Experiências e histórias territoriais

🤝 **Valores**
- Território como referência
- Autonomia territorial
- Tecnologia a serviço da vida
- Decolonização digital

Perguntas? Pergunte aqui ou explore outras salas.`;

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
    console.log('Iniciando configuração do Discord do Araponga\n');

    // Obter token de variável de ambiente ou solicitar interativamente
    let token = process.env.DISCORD_BOT_TOKEN;
    if (token) {
        console.log('Token encontrado em variável de ambiente');
        console.log(`Primeiros caracteres: ${token.substring(0, 10)}...\n`);
    } else {
        console.log('Token não encontrado em variável de ambiente');
        console.log('Solicitando token interativamente\n');
        console.log('Como obter o token:');
        console.log('1. Acesse: https://discord.com/developers/applications');
        console.log('2. Selecione sua aplicação ou crie uma nova');
        console.log('3. Vá em "Bot" > "Reset Token" ou "Copy"');
        console.log('4. Cole o token abaixo\n');

        token = await askQuestion('Token do bot: ');
        token = token.trim();

        if (!token) {
            console.error('\nErro: Token não pode estar vazio');
            process.exit(1);
        }

        console.log(`Token recebido (${token.length} caracteres)\n`);
    }

    // Validar formato básico do token
    console.log('Validando token...');
    if (token.length < 50) {
        console.error('Erro: Token muito curto');
        console.error('Tokens do Discord geralmente têm 59+ caracteres');
        console.error('Verifique se copiou o token completo');
        process.exit(1);
    }

    // Verificar formato (tipo.id.secret)
    const tokenParts = token.split('.');
    if (tokenParts.length !== 3) {
        console.warn('Aviso: Token não tem formato padrão (tipo.id.secret)');
        console.warn('Continuação pode falhar na autenticação');
    } else {
        console.log(`Token válido (tipo: ${tokenParts[0]}, ID: ${tokenParts[1]})\n`);
    }

    // Obter Guild ID de variável de ambiente ou solicitar interativamente
    let guildId = process.env.DISCORD_GUILD_ID;
    if (!guildId) {
        console.log('\nGuild ID não encontrado em variável de ambiente');
        console.log('Solicitando ID do servidor interativamente\n');
        console.log('Como obter o Guild ID:');
        console.log('1. No Discord, ative "Modo Desenvolvedor":');
        console.log('   Configurações do Usuário > Avançado > Modo de Desenvolvedor');
        console.log('2. Clique com botão direito no nome do servidor (ícone)');
        console.log('3. Clique em "Copiar ID"');
        console.log('4. Cole o ID abaixo\n');

        guildId = await askQuestion('ID do servidor (Guild ID): ');
        guildId = guildId.trim();

        if (!guildId) {
            console.error('\nErro: Guild ID não pode estar vazio');
            process.exit(1);
        }
    }

    // Validar formato do Guild ID (deve ser numérico)
    console.log('\nValidando Guild ID...');
    if (!/^\d+$/.test(guildId)) {
        console.error('Erro: Guild ID deve conter apenas números');
        console.error(`ID fornecido: ${guildId}`);
        console.error('Verifique se copiou o ID correto');
        process.exit(1);
    }
    console.log(`Guild ID validado: ${guildId}\n`);

    const client = new Client({
        intents: [
            GatewayIntentBits.Guilds,
            GatewayIntentBits.GuildMessages
            // MessageContent não é necessário para criar canais, apenas para ler conteúdo de mensagens
            // GatewayIntentBits.MessageContent
        ]
    });

    // Event listeners
    client.on('ready', () => {
        console.log('Bot conectado e pronto');
        console.log(`Bot: ${client.user.tag} (${client.user.id})\n`);
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
        console.log('\nConectando ao Discord...');

        const loginPromise = client.login(token);
        const timeoutPromise = new Promise((_, reject) =>
            setTimeout(() => reject(new Error('Timeout ao conectar (mais de 10 segundos)')), 10000)
        );

        await Promise.race([loginPromise, timeoutPromise]);
        console.log('Login bem-sucedido\n');

        // Aguardar bot ficar pronto
        await new Promise(resolve => setTimeout(resolve, 2000));

        console.log('Buscando servidor...');

        const guild = await client.guilds.fetch(guildId);
        console.log(`Servidor encontrado: ${guild.name}`);
        console.log(`Membros: ${guild.memberCount} | Canais: ${guild.channels.cache.size}\n`);

        // Verificar permissões do bot
        console.log('Verificando permissões do bot...');
        try {
            const botMember = await guild.members.fetch(client.user.id);
            const botPermissions = botMember.permissions;

            if (!botPermissions) {
                console.warn('Não foi possível verificar permissões. Continuando...\n');
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
                    console.warn('Nenhuma permissão válida encontrada. Continuando...\n');
                } else {
                    const missingPermissions = requiredPermissions.filter(
                        perm => perm !== undefined && perm !== null && !botPermissions.has(perm)
                    );

                    if (missingPermissions.length > 0) {
                        console.error('Erro: Bot não tem todas as permissões necessárias');
                        console.error('Permissões faltando:');
                        missingPermissions.forEach(perm => {
                            if (perm !== undefined && perm !== null) {
                                const permName = Object.keys(PermissionFlagsBits).find(key => PermissionFlagsBits[key] === perm);
                                console.error(`- ${permName || 'Desconhecida'}`);
                            }
                        });
                        console.error('\nSolução:');
                        console.error('1. Configurações do Servidor > Integrações');
                        console.error('2. Encontre o bot na lista');
                        console.error('3. Ative todas as permissões necessárias');
                        console.error('4. Verifique hierarquia de roles do bot');
                        throw new Error('Permissões insuficientes');
                    }
                    console.log('Bot tem todas as permissões necessárias\n');
                }
            }
        } catch (permError) {
            if (permError.message === 'Permissões insuficientes') {
                throw permError;
            }
            console.warn(`Não foi possível verificar permissões: ${permError.message}`);
            console.warn('Continuando...\n');
        }

        // Criar categorias e canais
        console.log('Criando categorias e canais...');
        console.log(`Total de categorias: ${channelConfig.length}\n`);
        const createdChannels = {};

        for (let i = 0; i < channelConfig.length; i++) {
            const categoryConfig = channelConfig[i];
            console.log(`[${i + 1}/${channelConfig.length}] ${categoryConfig.categoryName}`);

            // Verificar se categoria já existe
            const existingCategory = guild.channels.cache.find(
                ch => ch.type === ChannelType.GuildCategory && ch.name === categoryConfig.categoryName
            );

            if (existingCategory) {
                console.log(`Categoria já existe. Usando existente...`);
                var category = existingCategory;
            } else {
                category = await guild.channels.create({
                    name: categoryConfig.categoryName,
                    type: ChannelType.GuildCategory,
                    reason: 'Setup automático do Araponga'
                });
                console.log(`Categoria criada (ID: ${category.id})`);
            }

            // Criar canais dentro da categoria
            for (let j = 0; j < categoryConfig.channels.length; j++) {
                const channelInfo = categoryConfig.channels[j];
                console.log(`  [${j + 1}/${categoryConfig.channels.length}] #${channelInfo.name}`);

                // Verificar se canal já existe
                const existingChannel = guild.channels.cache.find(
                    ch => ch.type === channelInfo.type && ch.name === channelInfo.name && ch.parentId === category.id
                );

                if (existingChannel) {
                    console.log(`  Canal já existe. Usando existente...`);
                    createdChannels[channelInfo.name] = existingChannel;
                    continue;
                }

                // Criar canal
                const channel = await guild.channels.create({
                    name: channelInfo.name,
                    type: channelInfo.type,
                    parent: category.id,
                    topic: channelInfo.description,
                    reason: 'Setup automático do Araponga'
                });
                console.log(`  Canal criado (ID: ${channel.id})`);

                // Configurar permissões
                await channel.permissionOverwrites.edit(guild.roles.everyone, {
                    ViewChannel: true,
                    SendMessages: true,
                    ReadMessageHistory: true
                });

                createdChannels[channelInfo.name] = channel;

                // Delay para evitar rate limits
                await new Promise(resolve => setTimeout(resolve, 500));
            }
            console.log(''); // Linha em branco entre categorias
        }

        // Enviar ou atualizar mensagem de boas-vindas
        if (createdChannels['sala-pública']) {
            console.log('\nVerificando mensagem de boas-vindas...');

            // Verificar se há mensagem do bot existente
            const recentMessages = await createdChannels['sala-pública'].messages.fetch({ limit: 10 });
            const botMessages = recentMessages.filter(msg => msg.author.id === client.user.id);
            const existingWelcomeMessage = botMessages.find(msg =>
                msg.content.includes('Bem-vindo ao Araponga') ||
                msg.content.includes('🌟')
            );

            if (existingWelcomeMessage) {
                console.log('Mensagem de boas-vindas encontrada. Atualizando...');
                try {
                    await existingWelcomeMessage.edit(welcomeMessage);
                    if (!existingWelcomeMessage.pinned) {
                        await existingWelcomeMessage.pin();
                    }
                    console.log(`Mensagem atualizada (ID: ${existingWelcomeMessage.id})\n`);
                } catch (error) {
                    console.warn(`Não foi possível atualizar mensagem: ${error.message}`);
                    console.warn('Atualize manualmente ou delete a mensagem e execute o script novamente\n');
                }
            } else {
                // Criar nova mensagem
                const message = await createdChannels['sala-pública'].send(welcomeMessage);
                await message.pin();
                console.log(`Mensagem de boas-vindas criada e fixada (ID: ${message.id})\n`);
            }
        } else {
            console.warn('\nCanal #sala-pública não encontrado. Não foi possível criar mensagem de boas-vindas.\n');
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

        console.log('\n═══════════════════════════════════════════');
        console.log('Setup concluído com sucesso');
        console.log('═══════════════════════════════════════════\n');

        const totalChannels = channelConfig.reduce((sum, cat) => sum + cat.channels.length, 0);
        console.log('Resumo:');
        console.log(`- Categorias: ${channelConfig.length}`);
        console.log(`- Canais: ${totalChannels}`);
        console.log(`- Mensagem de boas-vindas: ${createdChannels['sala-pública'] ? 'Criada' : 'Não criada'}\n`);

        console.log('Próximos passos:');
        console.log('1. Verifique se todos os canais foram criados corretamente');
        console.log('2. Ajuste permissões se necessário');
        console.log('3. Adicione bots opcionais se desejar (MEE6, Dyno, etc.)');
        console.log('4. Convide pessoas para o servidor\n');

        if (createdChannels['sala-pública']) {
            console.log('Criando links de convite...');
            try {
                const permanentInvite = await createdChannels['sala-pública'].createInvite({
                    maxAge: 0,
                    maxUses: 0,
                    temporary: false,
                    reason: 'Link permanente do Discord do Araponga'
                });
                console.log('Link permanente criado:');
                console.log(`${permanentInvite.url}\n`);
            } catch (inviteError) {
                console.error('Não foi possível criar link de convite automaticamente');
                console.error(`Erro: ${inviteError.message}`);
                console.error('\nSolução manual:');
                console.error('1. Configurações do Servidor > Convites');
                console.error('2. Clique em "Criar Convite"');
                console.error('3. Configure: Sem data de expiração, Sem limite de usos');
                console.error('4. Copie o link e atualize a documentação\n');
            }
        }

        console.log('\n═══════════════════════════════════════════');

    } catch (error) {
        console.error('\n═══════════════════════════════════════════');
        console.error('Erro durante o setup');
        console.error('═══════════════════════════════════════════');
        console.error(`Tipo: ${error.name || 'Error'}`);
        console.error(`Mensagem: ${error.message}`);

        if (error.code) {
            console.error(`Código Discord: ${error.code}`);
        }

        if (error.stack) {
            console.error('\nStack trace:');
            console.error(error.stack);
        }

        // Tratamento específico de erros
        console.error('\nDiagnóstico:');

        if (error.code === 50001 || error.code === 50013) {
            console.error('Erro: Permissões Insuficientes\n');
            console.error('Solução:');
            console.error('1. Configurações do Servidor > Integrações');
            console.error('2. Encontre o bot na lista');
            console.error('3. Ative as seguintes permissões:');
            console.error('   - Ver Canais');
            console.error('   - Gerenciar Canais');
            console.error('   - Enviar Mensagens');
            console.error('   - Gerenciar Mensagens');
            console.error('   - Ler Histórico de Mensagens');
            console.error('   - Fixar Mensagens');
            console.error('4. Verifique hierarquia de roles do bot');
        } else if (error.code === 10004) {
            console.error('Erro: Servidor não encontrado\n');
            console.error('Solução:');
            console.error(`1. Verifique se o Guild ID está correto (ID fornecido: ${guildId || 'não definido'})`);
            console.error('2. Verifique se o bot está adicionado ao servidor');
            console.error('3. Para obter o Guild ID correto:');
            console.error('   - Ative Modo Desenvolvedor no Discord');
            console.error('   - Clique com botão direito no nome do servidor');
            console.error('   - Clique em "Copiar ID"');
        } else if (error.message && (error.message.includes('401') || error.message.includes('Unauthorized')) || error.code === 40001 || error.code === 50001) {
            console.error('Erro: Token inválido ou expirado\n');
            console.error(`Token: ${token ? token.length + ' caracteres' : 'N/A'}`);
            console.error(`Formato: ${token ? token.split('.').length + ' partes' : 'N/A'}`);
            console.error(`Código: ${error.code || 'N/A'}\n`);
            console.error('Solução:');
            console.error('1. Verifique se copiou o token completo (sem espaços)');
            console.error('2. Tokens do Discord geralmente têm 59+ caracteres');
            console.error('3. Acesse: https://discord.com/developers/applications');
            console.error('4. Selecione sua aplicação > "Bot" > "Reset Token" ou "Copy"');
            console.error('5. Certifique-se de copiar TODO o token');
            console.error('6. Cole novamente sem espaços extras');
            console.error('\nNota: Token deve ter 3 partes separadas por ponto (tipo.id.secret)');
        } else if (error.message && error.message.includes('429')) {
            console.error('Erro: Rate limit excedido\n');
            console.error('Solução: Aguarde alguns minutos e tente novamente');
        } else if (error.message && (error.message.includes('500') || error.message.includes('502') || error.message.includes('503'))) {
            console.error('Erro: Problema temporário no servidor Discord\n');
            console.error('Solução: Aguarde alguns minutos e tente novamente');
            console.error('Status: https://status.discord.com/');
        } else if (error.message && (error.message.includes('disallowed intents') || error.message.includes('Used disallowed intents'))) {
            console.error('Erro: Intents não habilitados no Discord\n');
            console.error('Solução:');
            console.error('1. Acesse: https://discord.com/developers/applications');
            console.error('2. Selecione sua aplicação > "Bot"');
            console.error('3. Role até "Privileged Gateway Intents"');
            console.error('4. Ative os intents necessários');
            console.error('5. Salve as mudanças e execute o script novamente');
            console.error('\nNota: Este script não requer Message Content Intent');
        } else {
            console.error('\nDiagnóstico:');
            console.error('1. Verifique se o token está correto');
            console.error('2. Verifique se o Guild ID está correto');
            console.error('3. Verifique se o bot está adicionado ao servidor');
            console.error('4. Verifique se o bot tem as permissões necessárias');
            console.error('5. Verifique a conexão com a internet');
            console.error('6. Verifique se os intents estão habilitados\n');
            console.error('Mais ajuda: https://devportal.araponga.app/wiki/docs/DISCORD_SETUP');
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
                console.log('\nCliente desconectado.');
            }
        } catch (destroyError) {
            // Ignorar erros ao destruir
        }
    }
}

// Executar
setupDiscord().catch(console.error);
