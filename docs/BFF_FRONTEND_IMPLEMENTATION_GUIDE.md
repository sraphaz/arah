# Guia de Implementação Frontend - BFF API

**Data**: 2026-01-27  
**Versão da API**: v2  
**Base URL**: `/api/v2/journeys`  
**Contrato OpenAPI**: [BFF_API_CONTRACT.yaml](./BFF_API_CONTRACT.yaml)

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Autenticação](#autenticação)
3. [Exemplos de Uso](#exemplos-de-uso)
4. [Migração da API v1 para v2](#migração-da-api-v1-para-v2)
5. [Tratamento de Erros](#tratamento-de-erros)
6. [Boas Práticas](#boas-práticas)

---

## 🎯 Visão Geral

A API BFF (Backend for Frontend) foi criada para:

- ✅ **Reduzir chamadas de rede**: De 5-10 chamadas para 1 por jornada
- ✅ **Simplificar lógica no frontend**: Dados já formatados para UI
- ✅ **Melhorar UX**: Sugestões contextuais e dados agregados
- ✅ **Encapsular responsabilidades de UI**: Transformações no backend

### Estrutura de Endpoints

```
/api/v2/journeys/
├── onboarding/
│   ├── POST /complete
│   └── GET /suggested-territories
├── feed/
│   ├── GET /territory-feed
│   ├── POST /create-post
│   └── POST /interact
├── events/
│   ├── GET /territory-events
│   ├── POST /create-event
│   └── POST /participate
└── marketplace/
    ├── GET /search
    ├── POST /add-to-cart
    └── POST /checkout
```

---

## 🔐 Autenticação

### Token JWT

A API BFF usa o mesmo sistema de autenticação da API v1:

```http
Authorization: Bearer <token_jwt>
```

**Obter Token**:
```http
POST /api/v1/auth/social
Content-Type: application/json

{
  "provider": "GOOGLE",
  "token": "...",
  "cpf": "..." // opcional
}
```

**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-01-28T10:00:00Z"
}
```

### Headers Obrigatórios

```http
Authorization: Bearer <token>
Content-Type: application/json
X-Session-Id: <session_id> // Opcional, para usuários não autenticados
```

---

## 💻 Exemplos de Uso

### 1. Onboarding Completo

**Antes (API v1 - 6 chamadas)**:
```dart
// 1. Autenticar
final authResponse = await authService.socialLogin(...);

// 2. Listar territórios
final territories = await territoryService.list();

// 3. Territórios próximos
final nearby = await territoryService.getNearby(lat, lng);

// 4. Selecionar território
await territoryService.select(territoryId);

// 5. Entrar como VISITOR
await membershipService.enter(territoryId);

// 6. Buscar feed inicial
final feed = await feedService.getFeed(territoryId);
```

**Depois (API v2 BFF - 1 chamada)**:
```dart
final response = await http.post(
  Uri.parse('$baseUrl/api/v2/journeys/onboarding/complete'),
  headers: {
    'Authorization': 'Bearer $token',
    'Content-Type': 'application/json',
  },
  body: jsonEncode({
    'selectedTerritoryId': territoryId,
    'location': {
      'latitude': lat,
      'longitude': lng,
    },
  }),
);

final data = jsonDecode(response.body);
// data.user - dados do usuário
// data.territory - território selecionado
// data.initialFeed - feed inicial formatado
// data.suggestedActions - ações sugeridas
```

### 2. Criar Post com Mídia

**Antes (API v1 - 3 chamadas)**:
```dart
// 1. Upload de cada mídia
final mediaIds = [];
for (var file in mediaFiles) {
  final media = await mediaService.upload(file);
  mediaIds.add(media.id);
}

// 2. Criar post
final post = await feedService.createPost(
  title: title,
  content: content,
  mediaIds: mediaIds,
);

// 3. Buscar post criado para exibir
final postDetails = await feedService.getPost(post.id);
```

**Depois (API v2 BFF - 1 chamada)**:
```dart
final request = http.MultipartRequest(
  'POST',
  Uri.parse('$baseUrl/api/v2/journeys/feed/create-post?territoryId=$territoryId'),
);

request.headers['Authorization'] = 'Bearer $token';

// Adicionar campos
request.fields['title'] = title;
request.fields['content'] = content;
request.fields['type'] = 'POST';
request.fields['visibility'] = 'PUBLIC';

// Adicionar arquivos
for (var file in mediaFiles) {
  request.files.add(
    await http.MultipartFile.fromPath('mediaFiles', file.path),
  );
}

final response = await request.send();
final responseBody = await response.stream.bytesToString();
final data = jsonDecode(responseBody);

// data.post - post formatado para UI
// data.mediaUrls - URLs das mídias
// data.suggestions - sugestões contextuais
```

### 3. Feed do Território

**Antes (API v1 - 5+ chamadas)**:
```dart
// 1. Buscar posts
final posts = await feedService.getFeed(territoryId);

// 2. Para cada post, buscar contadores
final counts = {};
for (var post in posts) {
  counts[post.id] = await feedService.getCounts(post.id);
}

// 3. Para cada post, buscar mídias
final mediaUrls = {};
for (var post in posts) {
  mediaUrls[post.id] = await mediaService.listByOwner(
    ownerType: 'Post',
    ownerId: post.id,
  );
}

// 4. Para posts de evento, buscar detalhes
final events = {};
for (var post in posts.where((p) => p.type == 'EVENT')) {
  events[post.id] = await eventsService.getEvent(post.referenceId);
}

// 5. Agregar tudo no frontend
final feedItems = posts.map((post) {
  return FeedItem(
    post: post,
    counts: counts[post.id],
    media: mediaUrls[post.id],
    event: events[post.id],
  );
}).toList();
```

**Depois (API v2 BFF - 1 chamada)**:
```dart
final response = await http.get(
  Uri.parse('$baseUrl/api/v2/journeys/feed/territory-feed')
    .replace(queryParameters: {
      'territoryId': territoryId,
      'pageNumber': '1',
      'pageSize': '20',
    }),
  headers: {
    'Authorization': 'Bearer $token',
  },
);

final data = jsonDecode(response.body);

// data.items - array de FeedItemJourney já formatado
// data.pagination - informações de paginação
// data.filters - filtros disponíveis

final feedItems = data['items'].map((item) {
  return FeedItem(
    post: item['post'],
    counts: item['counts'],
    media: item['media'],
    event: item['event'],
    author: item['author'],
    userInteractions: item['userInteractions'],
    metadata: item['metadata'],
  );
}).toList();
```

### 4. Participar de Evento

**Antes (API v1 - 4 chamadas)**:
```dart
// 1. Buscar detalhes do evento
final event = await eventsService.getEvent(eventId);

// 2. Buscar participantes
final participants = await eventsService.getParticipants(eventId);

// 3. Marcar interesse
await eventsService.setInterest(eventId);

// 4. Buscar evento atualizado
final updatedEvent = await eventsService.getEvent(eventId);
```

**Depois (API v2 BFF - 1 chamada)**:
```dart
final response = await http.post(
  Uri.parse('$baseUrl/api/v2/journeys/events/participate'),
  headers: {
    'Authorization': 'Bearer $token',
    'Content-Type': 'application/json',
  },
  body: jsonEncode({
    'eventId': eventId,
    'action': 'INTEREST', // ou 'CONFIRM', 'CANCEL'
  }),
);

final data = jsonDecode(response.body);

// data.event - evento atualizado formatado
// data.userParticipation - estado de participação do usuário
// data.participants - lista de participantes
// data.suggestions - eventos similares/próximos
```

### 5. Marketplace - Buscar e Comprar

**Antes (API v1 - 6 chamadas)**:
```dart
// 1. Buscar lojas
final stores = await storeService.list(territoryId);

// 2. Buscar itens
final items = await itemService.search(query);

// 3. Buscar detalhes do item
final itemDetails = await itemService.getItem(itemId);

// 4. Adicionar ao carrinho
await cartService.addItem(itemId, quantity);

// 5. Buscar carrinho
final cart = await cartService.getCart();

// 6. Finalizar compra
final order = await cartService.checkout();
```

**Depois (API v2 BFF - 3 chamadas)**:
```dart
// 1. Buscar itens (formatado)
final searchResponse = await http.get(
  Uri.parse('$baseUrl/api/v2/journeys/marketplace/search')
    .replace(queryParameters: {
      'territoryId': territoryId,
      'query': query,
      'pageNumber': '1',
    }),
);

final searchData = jsonDecode(searchResponse.body);
// searchData.items - itens formatados para UI
// searchData.stores - lojas
// searchData.filters - filtros disponíveis
// searchData.suggestions - sugestões

// 2. Adicionar ao carrinho
final addToCartResponse = await http.post(
  Uri.parse('$baseUrl/api/v2/journeys/marketplace/add-to-cart'),
  headers: {
    'Authorization': 'Bearer $token',
    'Content-Type': 'application/json',
  },
  body: jsonEncode({
    'territoryId': territoryId,
    'itemId': itemId,
    'quantity': 1,
  }),
);

final cartData = jsonDecode(addToCartResponse.body);
// cartData.cart - carrinho atualizado
// cartData.total - total formatado
// cartData.suggestions - produtos relacionados

// 3. Finalizar compra
final checkoutResponse = await http.post(
  Uri.parse('$baseUrl/api/v2/journeys/marketplace/checkout'),
  headers: {
    'Authorization': 'Bearer $token',
    'Content-Type': 'application/json',
  },
  body: jsonEncode({
    'territoryId': territoryId,
    'paymentMethod': 'PIX',
    'shippingAddress': {
      'street': 'Rua Exemplo',
      'number': '123',
      // ...
    },
  }),
);

final orderData = jsonDecode(checkoutResponse.body);
// orderData.order - ordem criada
// orderData.payment - informações de pagamento
// orderData.confirmation - confirmação
```

---

## 🔄 Migração da API v1 para v2

### Estratégia de Migração

1. **Coexistência**: API v1 e v2 coexistem
2. **Migração Gradual**: Migrar jornada por jornada
3. **Fallback**: Se BFF falhar, usar API v1

### Exemplo de Wrapper

```dart
class FeedService {
  final bool useBff;
  
  Future<TerritoryFeed> getTerritoryFeed({
    required String territoryId,
    int page = 1,
  }) async {
    if (useBff) {
      try {
        return await _getTerritoryFeedBff(territoryId, page);
      } catch (e) {
        // Fallback para API v1
        return await _getTerritoryFeedV1(territoryId, page);
      }
    } else {
      return await _getTerritoryFeedV1(territoryId, page);
    }
  }
  
  Future<TerritoryFeed> _getTerritoryFeedBff(
    String territoryId,
    int page,
  ) async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/v2/journeys/feed/territory-feed')
        .replace(queryParameters: {
          'territoryId': territoryId,
          'pageNumber': page.toString(),
        }),
      headers: {'Authorization': 'Bearer $token'},
    );
    
    if (response.statusCode != 200) {
      throw Exception('Failed to load feed');
    }
    
    final data = jsonDecode(response.body);
    return TerritoryFeed.fromBffJson(data);
  }
  
  Future<TerritoryFeed> _getTerritoryFeedV1(
    String territoryId,
    int page,
  ) async {
    // Implementação usando API v1
    // ...
  }
}
```

---

## ⚠️ Tratamento de Erros

### Códigos de Status HTTP

| Código | Significado | Ação |
|--------|------------|------|
| `200` | Sucesso | Processar resposta normalmente |
| `201` | Criado | Recurso criado com sucesso |
| `400` | Requisição Inválida | Validar dados enviados |
| `401` | Não Autenticado | Reautenticar usuário |
| `402` | Pagamento Necessário | Mostrar tela de pagamento |
| `404` | Não Encontrado | Recurso não existe |
| `429` | Muitas Requisições | Aguardar e tentar novamente |
| `500` | Erro do Servidor | Tentar novamente ou usar fallback |

### Estrutura de Erro

```json
{
  "error": "Invalid request parameters",
  "code": "INVALID_REQUEST",
  "details": {
    "field": "title",
    "message": "Title is required"
  },
  "timestamp": "2026-01-27T10:00:00Z"
}
```

### Exemplo de Tratamento

```dart
try {
  final response = await http.post(...);
  
  if (response.statusCode == 200) {
    final data = jsonDecode(response.body);
    return Success(data);
  } else if (response.statusCode == 400) {
    final error = jsonDecode(response.body);
    return Failure(error['error']);
  } else if (response.statusCode == 401) {
    // Reautenticar
    await authService.refreshToken();
    // Tentar novamente
    return await http.post(...);
  } else {
    return Failure('Unexpected error: ${response.statusCode}');
  }
} catch (e) {
  return Failure('Network error: $e');
}
```

---

## ✅ Boas Práticas

### 1. Cache de Respostas

```dart
class FeedCache {
  final Map<String, CachedFeed> _cache = {};
  
  Future<TerritoryFeed> getFeed(String territoryId) async {
    final key = 'feed_$territoryId';
    
    // Verificar cache
    if (_cache.containsKey(key)) {
      final cached = _cache[key]!;
      if (cached.isValid) {
        return cached.data;
      }
    }
    
    // Buscar do servidor
    final feed = await feedService.getTerritoryFeed(territoryId);
    
    // Atualizar cache
    _cache[key] = CachedFeed(
      data: feed,
      timestamp: DateTime.now(),
      ttl: Duration(minutes: 5),
    );
    
    return feed;
  }
}
```

### 2. Paginação Infinita

```dart
class InfiniteFeedScroll extends StatefulWidget {
  @override
  _InfiniteFeedScrollState createState() => _InfiniteFeedScrollState();
}

class _InfiniteFeedScrollState extends State<InfiniteFeedScroll> {
  final List<FeedItem> _items = [];
  int _currentPage = 1;
  bool _hasMore = true;
  bool _isLoading = false;
  
  Future<void> _loadMore() async {
    if (_isLoading || !_hasMore) return;
    
    setState(() => _isLoading = true);
    
    try {
      final response = await feedService.getTerritoryFeed(
        territoryId: territoryId,
        page: _currentPage,
      );
      
      setState(() {
        _items.addAll(response.items);
        _currentPage++;
        _hasMore = response.pagination.hasNextPage;
        _isLoading = false;
      });
    } catch (e) {
      setState(() => _isLoading = false);
      // Tratar erro
    }
  }
  
  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      itemCount: _items.length + (_hasMore ? 1 : 0),
      itemBuilder: (context, index) {
        if (index == _items.length) {
          _loadMore();
          return LoadingIndicator();
        }
        return FeedItemWidget(item: _items[index]);
      },
    );
  }
}
```

### 3. Retry com Exponential Backoff

```dart
Future<T> retryWithBackoff<T>(
  Future<T> Function() operation, {
  int maxRetries = 3,
  Duration initialDelay = const Duration(seconds: 1),
}) async {
  int retries = 0;
  Duration delay = initialDelay;
  
  while (retries < maxRetries) {
    try {
      return await operation();
    } catch (e) {
      retries++;
      if (retries >= maxRetries) {
        rethrow;
      }
      await Future.delayed(delay);
      delay = Duration(milliseconds: delay.inMilliseconds * 2);
    }
  }
  
  throw Exception('Max retries exceeded');
}
```

### 4. Validação de Dados

```dart
class CreatePostRequest {
  final String title;
  final String content;
  final String type;
  final String visibility;
  
  CreatePostRequest({
    required this.title,
    required this.content,
    required this.type,
    required this.visibility,
  });
  
  Map<String, dynamic> toJson() {
    // Validar antes de serializar
    if (title.isEmpty || title.length > 200) {
      throw ValidationException('Title must be between 1 and 200 characters');
    }
    
    if (content.isEmpty || content.length > 5000) {
      throw ValidationException('Content must be between 1 and 5000 characters');
    }
    
    if (!['POST', 'ALERT', 'EVENT'].contains(type)) {
      throw ValidationException('Invalid post type');
    }
    
    if (!['PUBLIC', 'RESIDENTS_ONLY'].contains(visibility)) {
      throw ValidationException('Invalid visibility');
    }
    
    return {
      'title': title,
      'content': content,
      'type': type,
      'visibility': visibility,
    };
  }
}
```

---

## 📚 Recursos Adicionais

- **Contrato OpenAPI**: [BFF_API_CONTRACT.yaml](./BFF_API_CONTRACT.yaml)
- **Avaliação BFF**: [AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md](./AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md)
- **API v1 (Referência)**: [60_99_API_RESUMO_ENDPOINTS.md](./api/60_99_API_RESUMO_ENDPOINTS.md)

---

**Última Atualização**: 2026-01-27  
**Status**: 📋 Guia Completo - Pronto para Implementação
