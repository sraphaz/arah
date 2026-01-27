# Guia de Uso - Postman Collection BFF API

**Data**: 2026-01-27  
**Versão**: 2.0.0

Este guia explica como importar e usar a coleção do Postman para testar a API BFF.

---

## 📥 Importar no Postman

### 1. Importar Coleção

1. Abra o Postman
2. Clique em **Import** (canto superior esquerdo)
3. Selecione o arquivo `BFF_Postman_Collection.json`
4. Clique em **Import**

### 2. Importar Ambientes (Opcional mas Recomendado)

Importe os arquivos de ambiente para facilitar a troca entre ambientes:

- **Produção**: `BFF_Postman_Environment.json`
- **Staging**: `BFF_Postman_Environment_Staging.json`
- **Local**: `BFF_Postman_Environment_Local.json`

**Como importar**:
1. Clique no ícone de engrenagem (⚙️) no canto superior direito
2. Clique em **Import**
3. Selecione os arquivos de ambiente
4. Selecione o ambiente desejado no dropdown

---

## 🚀 Como Usar

### Passo 1: Obter Token de Autenticação

1. Selecione o ambiente desejado (Local, Staging ou Production)
2. Vá para a pasta **Auth (v1)**
3. Abra a requisição **Social Login**
4. Preencha o body com seus dados:
   ```json
   {
     "provider": "GOOGLE",
     "token": "seu_google_token_aqui",
     "cpf": "12345678900"  // opcional
   }
   ```
5. Clique em **Send**
6. Copie o `token` da resposta
7. Cole o token na variável `auth_token` do ambiente:
   - Clique no ícone de olho (👁️) no canto superior direito
   - Edite a variável `auth_token`
   - Cole o token
   - Salve

### Passo 2: Configurar Território

1. Obtenha um `territory_id` válido (pode ser via API v1 ou banco de dados)
2. Edite a variável `territory_id` no ambiente
3. Cole o ID do território

### Passo 3: Testar Endpoints

Agora você pode testar qualquer endpoint da coleção:

#### Exemplo: Obter Feed do Território

1. Vá para **Feed > Get Territory Feed**
2. Clique em **Send**
3. A resposta deve retornar o feed formatado

#### Exemplo: Criar Post

1. Vá para **Feed > Create Post**
2. Preencha os campos no body:
   - `title`: Título do post
   - `content`: Conteúdo do post
   - `type`: POST, ALERT ou EVENT
   - `visibility`: PUBLIC ou RESIDENTS_ONLY
   - `tags[0]`, `tags[1]`: Tags (opcional)
   - `mediaFiles`: Selecione arquivos de imagem (opcional)
3. Clique em **Send**
4. A resposta deve retornar o post criado

---

## 📋 Estrutura da Coleção

### Onboarding
- **Complete Onboarding** - Completa onboarding em uma chamada
- **Get Suggested Territories** - Obtém territórios sugeridos

### Feed
- **Get Territory Feed** - Obtém feed formatado para UI
- **Create Post** - Cria post com mídias
- **Interact with Post** - Interage com post (like, comment, share)

### Events
- **Get Territory Events** - Lista eventos formatados
- **Create Event** - Cria evento com mídias
- **Participate in Event** - Participa de evento

### Marketplace
- **Search Items** - Busca itens formatados
- **Add to Cart** - Adiciona item ao carrinho
- **Checkout** - Finaliza compra

### Auth (v1)
- **Social Login** - Autentica e obtém token JWT

---

## 🔧 Variáveis de Ambiente

A coleção usa as seguintes variáveis:

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `base_url` | URL base da API | `https://api.araponga.com` |
| `auth_token` | Token JWT de autenticação | `eyJhbGciOiJIUzI1NiIs...` |
| `territory_id` | ID do território ativo | `550e8400-e29b-41d4-a716-446655440000` |
| `post_id` | ID do post (para interações) | `123e4567-e89b-12d3-a456-426614174000` |
| `event_id` | ID do evento (para participação) | `789e0123-e89b-12d3-a456-426614174002` |
| `item_id` | ID do item do marketplace | `456e7890-e89b-12d3-a456-426614174001` |

---

## 💡 Dicas

### 1. Usar Variáveis Dinâmicas

Após criar um post, você pode copiar o `id` da resposta e colar na variável `post_id` para testar interações.

### 2. Testes Automáticos

Você pode adicionar scripts de teste no Postman para validar respostas automaticamente:

```javascript
// Exemplo de teste para Get Territory Feed
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has items array", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('items');
    pm.expect(jsonData.items).to.be.an('array');
});

pm.test("Response has pagination", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('pagination');
});
```

### 3. Salvar Respostas como Exemplos

1. Após receber uma resposta válida
2. Clique em **Save Response**
3. Selecione **Save as Example**
4. Isso ajuda a documentar a API

### 4. Usar Pre-request Scripts

Você pode adicionar scripts que executam antes das requisições:

```javascript
// Exemplo: Auto-refresh token
if (pm.environment.get("token_expires_at") < Date.now()) {
    // Fazer refresh do token
}
```

---

## 🐛 Troubleshooting

### Erro 401 (Unauthorized)
- Verifique se o `auth_token` está configurado corretamente
- Verifique se o token não expirou
- Obtenha um novo token via **Auth (v1) > Social Login**

### Erro 400 (Bad Request)
- Verifique se todos os campos obrigatórios estão preenchidos
- Verifique se os tipos de dados estão corretos (UUIDs, enums, etc.)
- Veja a mensagem de erro na resposta para mais detalhes

### Erro 404 (Not Found)
- Verifique se o `base_url` está correto
- Verifique se o endpoint existe na versão da API
- Verifique se os IDs (territory_id, post_id, etc.) são válidos

---

## 📚 Documentação Relacionada

- **Contrato OpenAPI**: [BFF_API_CONTRACT.yaml](./BFF_API_CONTRACT.yaml)
- **Guia de Implementação**: [BFF_FRONTEND_IMPLEMENTATION_GUIDE.md](./BFF_FRONTEND_IMPLEMENTATION_GUIDE.md)
- **Exemplo Flutter**: [BFF_FLUTTER_EXAMPLE.md](./BFF_FLUTTER_EXAMPLE.md)
- **Resumo de Contratos**: [BFF_CONTRACT_SUMMARY.md](./BFF_CONTRACT_SUMMARY.md)

---

**Última Atualização**: 2026-01-27  
**Status**: 📋 Guia Completo - Pronto para Uso
