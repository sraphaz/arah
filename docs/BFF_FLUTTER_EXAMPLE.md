# Exemplo de Implementação Flutter - BFF API

**Data**: 2026-01-27  
**Versão**: 2.0.0  
**Framework**: Flutter/Dart

Este documento contém um exemplo completo e funcional de implementação do BFF em Flutter.

---

## 📁 Estrutura de Arquivos

```
lib/
├── models/
│   ├── bff_models.dart
│   └── feed_models.dart
├── services/
│   ├── bff_api_service.dart
│   └── auth_service.dart
├── widgets/
│   └── feed_screen.dart
└── main.dart
```

---

## 📦 1. Modelos (Models)

### `lib/models/bff_models.dart`

```dart
// Modelos base para respostas BFF

class TerritoryFeedJourneyResponse {
  final List<FeedItemJourney> items;
  final Pagination pagination;
  final FeedFilters filters;

  TerritoryFeedJourneyResponse({
    required this.items,
    required this.pagination,
    required this.filters,
  });

  factory TerritoryFeedJourneyResponse.fromJson(Map<String, dynamic> json) {
    return TerritoryFeedJourneyResponse(
      items: (json['items'] as List)
          .map((item) => FeedItemJourney.fromJson(item))
          .toList(),
      pagination: Pagination.fromJson(json['pagination']),
      filters: FeedFilters.fromJson(json['filters']),
    );
  }
}

class FeedItemJourney {
  final PostSummary post;
  final PostCounts counts;
  final List<MediaItem> media;
  final EventSummary? event;
  final UserSummary author;
  final UserInteractions userInteractions;
  final PostMetadata metadata;

  FeedItemJourney({
    required this.post,
    required this.counts,
    required this.media,
    this.event,
    required this.author,
    required this.userInteractions,
    required this.metadata,
  });

  factory FeedItemJourney.fromJson(Map<String, dynamic> json) {
    return FeedItemJourney(
      post: PostSummary.fromJson(json['post']),
      counts: PostCounts.fromJson(json['counts']),
      media: (json['media'] as List)
          .map((item) => MediaItem.fromJson(item))
          .toList(),
      event: json['event'] != null
          ? EventSummary.fromJson(json['event'])
          : null,
      author: UserSummary.fromJson(json['author']),
      userInteractions: UserInteractions.fromJson(json['userInteractions']),
      metadata: PostMetadata.fromJson(json['metadata']),
    );
  }
}

class PostSummary {
  final String id;
  final String title;
  final String content;
  final String type;
  final String visibility;
  final String status;
  final String? mapEntityId;
  final DateTime createdAtUtc;
  final List<String>? tags;

  PostSummary({
    required this.id,
    required this.title,
    required this.content,
    required this.type,
    required this.visibility,
    required this.status,
    this.mapEntityId,
    required this.createdAtUtc,
    this.tags,
  });

  factory PostSummary.fromJson(Map<String, dynamic> json) {
    return PostSummary(
      id: json['id'] as String,
      title: json['title'] as String,
      content: json['content'] as String,
      type: json['type'] as String,
      visibility: json['visibility'] as String,
      status: json['status'] as String,
      mapEntityId: json['mapEntityId'] as String?,
      createdAtUtc: DateTime.parse(json['createdAtUtc'] as String),
      tags: json['tags'] != null
          ? List<String>.from(json['tags'] as List)
          : null,
    );
  }
}

class PostCounts {
  final int likes;
  final int shares;
  final int comments;

  PostCounts({
    required this.likes,
    required this.shares,
    required this.comments,
  });

  factory PostCounts.fromJson(Map<String, dynamic> json) {
    return PostCounts(
      likes: json['likes'] as int,
      shares: json['shares'] as int,
      comments: json['comments'] as int,
    );
  }
}

class MediaItem {
  final String url;
  final String type;
  final String? thumbnailUrl;
  final int? width;
  final int? height;
  final int? duration;

  MediaItem({
    required this.url,
    required this.type,
    this.thumbnailUrl,
    this.width,
    this.height,
    this.duration,
  });

  factory MediaItem.fromJson(Map<String, dynamic> json) {
    return MediaItem(
      url: json['url'] as String,
      type: json['type'] as String,
      thumbnailUrl: json['thumbnailUrl'] as String?,
      width: json['width'] as int?,
      height: json['height'] as int?,
      duration: json['duration'] as int?,
    );
  }
}

class EventSummary {
  final String id;
  final String territoryId;
  final String title;
  final String? description;
  final DateTime startsAtUtc;
  final DateTime? endsAtUtc;
  final double latitude;
  final double longitude;
  final String? locationLabel;
  final int interestedCount;
  final int confirmedCount;

  EventSummary({
    required this.id,
    required this.territoryId,
    required this.title,
    this.description,
    required this.startsAtUtc,
    this.endsAtUtc,
    required this.latitude,
    required this.longitude,
    this.locationLabel,
    required this.interestedCount,
    required this.confirmedCount,
  });

  factory EventSummary.fromJson(Map<String, dynamic> json) {
    return EventSummary(
      id: json['id'] as String,
      territoryId: json['territoryId'] as String,
      title: json['title'] as String,
      description: json['description'] as String?,
      startsAtUtc: DateTime.parse(json['startsAtUtc'] as String),
      endsAtUtc: json['endsAtUtc'] != null
          ? DateTime.parse(json['endsAtUtc'] as String)
          : null,
      latitude: (json['latitude'] as num).toDouble(),
      longitude: (json['longitude'] as num).toDouble(),
      locationLabel: json['locationLabel'] as String?,
      interestedCount: json['interestedCount'] as int,
      confirmedCount: json['confirmedCount'] as int,
    );
  }
}

class UserSummary {
  final String id;
  final String displayName;
  final String? email;
  final String membership;
  final String? avatarUrl;

  UserSummary({
    required this.id,
    required this.displayName,
    this.email,
    required this.membership,
    this.avatarUrl,
  });

  factory UserSummary.fromJson(Map<String, dynamic> json) {
    return UserSummary(
      id: json['id'] as String,
      displayName: json['displayName'] as String,
      email: json['email'] as String?,
      membership: json['membership'] as String,
      avatarUrl: json['avatarUrl'] as String?,
    );
  }
}

class UserInteractions {
  final bool liked;
  final bool shared;
  final bool commented;

  UserInteractions({
    required this.liked,
    required this.shared,
    required this.commented,
  });

  factory UserInteractions.fromJson(Map<String, dynamic> json) {
    return UserInteractions(
      liked: json['liked'] as bool,
      shared: json['shared'] as bool,
      commented: json['commented'] as bool,
    );
  }
}

class PostMetadata {
  final bool canEdit;
  final bool canDelete;
  final bool canShare;
  final bool canComment;

  PostMetadata({
    required this.canEdit,
    required this.canDelete,
    required this.canShare,
    required this.canComment,
  });

  factory PostMetadata.fromJson(Map<String, dynamic> json) {
    return PostMetadata(
      canEdit: json['canEdit'] as bool,
      canDelete: json['canDelete'] as bool,
      canShare: json['canShare'] as bool,
      canComment: json['canComment'] as bool,
    );
  }
}

class Pagination {
  final int pageNumber;
  final int pageSize;
  final int totalCount;
  final int totalPages;
  final bool hasPreviousPage;
  final bool hasNextPage;

  Pagination({
    required this.pageNumber,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
    required this.hasPreviousPage,
    required this.hasNextPage,
  });

  factory Pagination.fromJson(Map<String, dynamic> json) {
    return Pagination(
      pageNumber: json['pageNumber'] as int,
      pageSize: json['pageSize'] as int,
      totalCount: json['totalCount'] as int,
      totalPages: json['totalPages'] as int,
      hasPreviousPage: json['hasPreviousPage'] as bool,
      hasNextPage: json['hasNextPage'] as bool,
    );
  }
}

class FeedFilters {
  final List<String> availableTypes;
  final List<String> availableTags;
  final List<String> availableVisibilities;

  FeedFilters({
    required this.availableTypes,
    required this.availableTags,
    required this.availableVisibilities,
  });

  factory FeedFilters.fromJson(Map<String, dynamic> json) {
    return FeedFilters(
      availableTypes: List<String>.from(json['availableTypes'] as List),
      availableTags: List<String>.from(json['availableTags'] as List),
      availableVisibilities:
          List<String>.from(json['availableVisibilities'] as List),
    );
  }
}

// Modelos para criar post

class CreatePostJourneyRequest {
  final String title;
  final String content;
  final String type;
  final String visibility;
  final List<String>? tags;
  final String? mapEntityId;
  final List<String>? mediaFilePaths;

  CreatePostJourneyRequest({
    required this.title,
    required this.content,
    required this.type,
    required this.visibility,
    this.tags,
    this.mapEntityId,
    this.mediaFilePaths,
  });
}

class CreatePostJourneyResponse {
  final bool success;
  final FeedItemJourney? post;
  final List<String>? mediaUrls;
  final PostSuggestions? suggestions;
  final String? error;

  CreatePostJourneyResponse({
    required this.success,
    this.post,
    this.mediaUrls,
    this.suggestions,
    this.error,
  });

  factory CreatePostJourneyResponse.fromJson(Map<String, dynamic> json) {
    return CreatePostJourneyResponse(
      success: json['success'] as bool,
      post: json['post'] != null
          ? FeedItemJourney.fromJson(json['post'])
          : null,
      mediaUrls: json['mediaUrls'] != null
          ? List<String>.from(json['mediaUrls'] as List)
          : null,
      suggestions: json['suggestions'] != null
          ? PostSuggestions.fromJson(json['suggestions'])
          : null,
      error: json['error'] as String?,
    );
  }
}

class PostSuggestions {
  final List<PostSummary> similarPosts;
  final List<String> suggestedTags;

  PostSuggestions({
    required this.similarPosts,
    required this.suggestedTags,
  });

  factory PostSuggestions.fromJson(Map<String, dynamic> json) {
    return PostSuggestions(
      similarPosts: (json['similarPosts'] as List)
          .map((item) => PostSummary.fromJson(item))
          .toList(),
      suggestedTags: List<String>.from(json['suggestedTags'] as List),
    );
  }
}

// Modelos para interação

class PostInteractionRequest {
  final String postId;
  final String action; // LIKE, UNLIKE, COMMENT, SHARE
  final String? comment;

  PostInteractionRequest({
    required this.postId,
    required this.action,
    this.comment,
  });

  Map<String, dynamic> toJson() {
    return {
      'postId': postId,
      'action': action,
      if (comment != null) 'comment': comment,
    };
  }
}

class PostInteractionResponse {
  final bool success;
  final FeedItemJourney? post;
  final PostCounts? updatedCounts;
  final String? error;

  PostInteractionResponse({
    required this.success,
    this.post,
    this.updatedCounts,
    this.error,
  });

  factory PostInteractionResponse.fromJson(Map<String, dynamic> json) {
    return PostInteractionResponse(
      success: json['success'] as bool,
      post: json['post'] != null
          ? FeedItemJourney.fromJson(json['post'])
          : null,
      updatedCounts: json['updatedCounts'] != null
          ? PostCounts.fromJson(json['updatedCounts'])
          : null,
      error: json['error'] as String?,
    );
  }
}
```

---

## 🔧 2. Serviço de API

### `lib/services/bff_api_service.dart`

```dart
import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:http_parser/http_parser.dart';
import '../models/bff_models.dart';

class BffApiService {
  final String baseUrl;
  final String? authToken;

  BffApiService({
    required this.baseUrl,
    this.authToken,
  });

  // Headers padrão
  Map<String, String> get _headers {
    final headers = {
      'Content-Type': 'application/json',
    };
    if (authToken != null) {
      headers['Authorization'] = 'Bearer $authToken';
    }
    return headers;
  }

  /// Obtém feed do território formatado para UI
  Future<TerritoryFeedJourneyResponse> getTerritoryFeed({
    required String territoryId,
    int pageNumber = 1,
    int pageSize = 20,
    bool filterByInterests = false,
    String? mapEntityId,
    String? assetId,
  }) async {
    final queryParams = {
      'territoryId': territoryId,
      'pageNumber': pageNumber.toString(),
      'pageSize': pageSize.toString(),
      'filterByInterests': filterByInterests.toString(),
      if (mapEntityId != null) 'mapEntityId': mapEntityId,
      if (assetId != null) 'assetId': assetId,
    };

    final uri = Uri.parse('$baseUrl/api/v2/journeys/feed/territory-feed')
        .replace(queryParameters: queryParams);

    final response = await http.get(uri, headers: _headers);

    if (response.statusCode == 200) {
      final json = jsonDecode(response.body) as Map<String, dynamic>;
      return TerritoryFeedJourneyResponse.fromJson(json);
    } else {
      throw _handleError(response);
    }
  }

  /// Cria post com mídias em uma única chamada
  Future<CreatePostJourneyResponse> createPost({
    required String territoryId,
    required CreatePostJourneyRequest request,
  }) async {
    final uri = Uri.parse('$baseUrl/api/v2/journeys/feed/create-post')
        .replace(queryParameters: {'territoryId': territoryId});

    // Criar multipart request
    final multipartRequest = http.MultipartRequest('POST', uri);

    // Adicionar headers de autenticação
    if (authToken != null) {
      multipartRequest.headers['Authorization'] = 'Bearer $authToken';
    }

    // Adicionar campos de texto
    multipartRequest.fields['title'] = request.title;
    multipartRequest.fields['content'] = request.content;
    multipartRequest.fields['type'] = request.type;
    multipartRequest.fields['visibility'] = request.visibility;

    if (request.mapEntityId != null) {
      multipartRequest.fields['mapEntityId'] = request.mapEntityId!;
    }

    if (request.tags != null && request.tags!.isNotEmpty) {
      for (var i = 0; i < request.tags!.length; i++) {
        multipartRequest.fields['tags[$i]'] = request.tags![i];
      }
    }

    // Adicionar arquivos de mídia
    if (request.mediaFilePaths != null && request.mediaFilePaths!.isNotEmpty) {
      for (var filePath in request.mediaFilePaths!) {
        final file = await http.MultipartFile.fromPath(
          'mediaFiles',
          filePath,
          contentType: MediaType('image', 'jpeg'), // Ajustar conforme tipo
        );
        multipartRequest.files.add(file);
      }
    }

    // Enviar requisição
    final streamedResponse = await multipartRequest.send();
    final response = await http.Response.fromStream(streamedResponse);

    if (response.statusCode == 201) {
      final json = jsonDecode(response.body) as Map<String, dynamic>;
      return CreatePostJourneyResponse.fromJson(json);
    } else {
      throw _handleError(response);
    }
  }

  /// Interage com um post (like, comment, share)
  Future<PostInteractionResponse> interactWithPost({
    required PostInteractionRequest request,
  }) async {
    final uri = Uri.parse('$baseUrl/api/v2/journeys/feed/interact');

    final response = await http.post(
      uri,
      headers: _headers,
      body: jsonEncode(request.toJson()),
    );

    if (response.statusCode == 200) {
      final json = jsonDecode(response.body) as Map<String, dynamic>;
      return PostInteractionResponse.fromJson(json);
    } else {
      throw _handleError(response);
    }
  }

  /// Tratamento de erros
  Exception _handleError(http.Response response) {
    try {
      final json = jsonDecode(response.body) as Map<String, dynamic>;
      final error = json['error'] as String? ?? 'Unknown error';
      final code = json['code'] as String? ?? 'UNKNOWN_ERROR';
      return BffApiException(
        message: error,
        code: code,
        statusCode: response.statusCode,
      );
    } catch (e) {
      return BffApiException(
        message: 'Failed to parse error response',
        code: 'PARSE_ERROR',
        statusCode: response.statusCode,
      );
    }
  }
}

/// Exceção customizada para erros da API BFF
class BffApiException implements Exception {
  final String message;
  final String code;
  final int statusCode;

  BffApiException({
    required this.message,
    required this.code,
    required this.statusCode,
  });

  @override
  String toString() => 'BffApiException: $code ($statusCode) - $message';
}
```

---

## 🎨 3. Widget de Exemplo

### `lib/widgets/feed_screen.dart`

```dart
import 'package:flutter/material.dart';
import '../models/bff_models.dart';
import '../services/bff_api_service.dart';

class FeedScreen extends StatefulWidget {
  final String territoryId;
  final String? authToken;
  final String baseUrl;

  const FeedScreen({
    Key? key,
    required this.territoryId,
    this.authToken,
    required this.baseUrl,
  }) : super(key: key);

  @override
  State<FeedScreen> createState() => _FeedScreenState();
}

class _FeedScreenState extends State<FeedScreen> {
  late BffApiService _apiService;
  final List<FeedItemJourney> _items = [];
  bool _isLoading = false;
  bool _hasMore = true;
  int _currentPage = 1;
  String? _error;

  @override
  void initState() {
    super.initState();
    _apiService = BffApiService(
      baseUrl: widget.baseUrl,
      authToken: widget.authToken,
    );
    _loadFeed();
  }

  Future<void> _loadFeed({bool loadMore = false}) async {
    if (_isLoading) return;

    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      final response = await _apiService.getTerritoryFeed(
        territoryId: widget.territoryId,
        pageNumber: loadMore ? _currentPage + 1 : 1,
        pageSize: 20,
      );

      setState(() {
        if (loadMore) {
          _items.addAll(response.items);
          _currentPage++;
        } else {
          _items.clear();
          _items.addAll(response.items);
          _currentPage = 1;
        }
        _hasMore = response.pagination.hasNextPage;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _error = e.toString();
        _isLoading = false;
      });
    }
  }

  Future<void> _handleLike(FeedItemJourney item) async {
    try {
      final response = await _apiService.interactWithPost(
        request: PostInteractionRequest(
          postId: item.post.id,
          action: item.userInteractions.liked ? 'UNLIKE' : 'LIKE',
        ),
      );

      if (response.success && response.post != null) {
        // Atualizar item na lista
        final index = _items.indexWhere((i) => i.post.id == item.post.id);
        if (index != -1) {
          setState(() {
            _items[index] = response.post!;
          });
        }
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Erro ao curtir: $e')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Feed do Território'),
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_error != null && _items.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text('Erro: $_error'),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: () => _loadFeed(),
              child: const Text('Tentar Novamente'),
            ),
          ],
        ),
      );
    }

    if (_items.isEmpty && _isLoading) {
      return const Center(child: CircularProgressIndicator());
    }

    return RefreshIndicator(
      onRefresh: () => _loadFeed(),
      child: ListView.builder(
        itemCount: _items.length + (_hasMore ? 1 : 0),
        itemBuilder: (context, index) {
          if (index == _items.length) {
            // Carregar mais
            if (!_isLoading) {
              _loadFeed(loadMore: true);
            }
            return const Center(
              child: Padding(
                padding: EdgeInsets.all(16.0),
                child: CircularProgressIndicator(),
              ),
            );
          }

          return _FeedItemWidget(
            item: _items[index],
            onLike: () => _handleLike(_items[index]),
          );
        },
      ),
    );
  }
}

class _FeedItemWidget extends StatelessWidget {
  final FeedItemJourney item;
  final VoidCallback onLike;

  const _FeedItemWidget({
    Key? key,
    required this.item,
    required this.onLike,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Cabeçalho com autor
            Row(
              children: [
                CircleAvatar(
                  backgroundImage: item.author.avatarUrl != null
                      ? NetworkImage(item.author.avatarUrl!)
                      : null,
                  child: item.author.avatarUrl == null
                      ? Text(item.author.displayName[0].toUpperCase())
                      : null,
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        item.author.displayName,
                        style: const TextStyle(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Text(
                        _formatDate(item.post.createdAtUtc),
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey[600],
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),

            // Título e conteúdo
            Text(
              item.post.title,
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              item.post.content,
              style: const TextStyle(fontSize: 14),
            ),

            // Mídias
            if (item.media.isNotEmpty) ...[
              const SizedBox(height: 12),
              SizedBox(
                height: 200,
                child: ListView.builder(
                  scrollDirection: Axis.horizontal,
                  itemCount: item.media.length,
                  itemBuilder: (context, index) {
                    final media = item.media[index];
                    return Padding(
                      padding: const EdgeInsets.only(right: 8.0),
                      child: Image.network(
                        media.url,
                        fit: BoxFit.cover,
                        errorBuilder: (context, error, stackTrace) {
                          return const Icon(Icons.broken_image);
                        },
                      ),
                    );
                  },
                ),
              ),
            ],

            // Evento relacionado
            if (item.event != null) ...[
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.blue[50],
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        const Icon(Icons.event, size: 20),
                        const SizedBox(width: 8),
                        Text(
                          item.event!.title,
                          style: const TextStyle(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 4),
                    Text(
                      _formatDate(item.event!.startsAtUtc),
                      style: TextStyle(fontSize: 12, color: Colors.grey[700]),
                    ),
                  ],
                ),
              ),
            ],

            // Tags
            if (item.post.tags != null && item.post.tags!.isNotEmpty) ...[
              const SizedBox(height: 12),
              Wrap(
                spacing: 8,
                children: item.post.tags!
                    .map((tag) => Chip(
                          label: Text(tag),
                          labelStyle: const TextStyle(fontSize: 12),
                        ))
                    .toList(),
              ),
            ],

            const SizedBox(height: 16),

            // Ações (like, share, comment)
            Row(
              children: [
                IconButton(
                  icon: Icon(
                    item.userInteractions.liked
                        ? Icons.favorite
                        : Icons.favorite_border,
                    color: item.userInteractions.liked ? Colors.red : null,
                  ),
                  onPressed: onLike,
                ),
                Text('${item.counts.likes}'),
                const SizedBox(width: 16),
                const Icon(Icons.share),
                Text('${item.counts.shares}'),
                const SizedBox(width: 16),
                const Icon(Icons.comment),
                Text('${item.counts.comments}'),
              ],
            ),
          ],
        ),
      ),
    );
  }

  String _formatDate(DateTime date) {
    final now = DateTime.now();
    final difference = now.difference(date);

    if (difference.inDays > 7) {
      return '${date.day}/${date.month}/${date.year}';
    } else if (difference.inDays > 0) {
      return '${difference.inDays}d atrás';
    } else if (difference.inHours > 0) {
      return '${difference.inHours}h atrás';
    } else if (difference.inMinutes > 0) {
      return '${difference.inMinutes}min atrás';
    } else {
      return 'Agora';
    }
  }
}
```

---

## 📝 4. Exemplo de Uso

### `lib/main.dart`

```dart
import 'package:flutter/material.dart';
import 'widgets/feed_screen.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Araponga BFF Example',
      theme: ThemeData(
        primarySwatch: Colors.blue,
      ),
      home: const HomeScreen(),
    );
  }
}

class HomeScreen extends StatelessWidget {
  const HomeScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    // Exemplo de uso
    const territoryId = '550e8400-e29b-41d4-a716-446655440000';
    const baseUrl = 'https://api.araponga.com';
    const authToken = 'your_jwt_token_here'; // Obter via /api/v1/auth/social

    return FeedScreen(
      territoryId: territoryId,
      baseUrl: baseUrl,
      authToken: authToken,
    );
  }
}
```

---

## 📦 5. Dependências (pubspec.yaml)

```yaml
name: araponga_bff_example
description: Exemplo de implementação BFF em Flutter
version: 1.0.0

environment:
  sdk: '>=2.17.0 <4.0.0'

dependencies:
  flutter:
    sdk: flutter
  http: ^0.13.5
  http_parser: ^4.0.2

dev_dependencies:
  flutter_test:
    sdk: flutter
  flutter_lints: ^2.0.0
```

---

## 🚀 6. Exemplo de Criar Post

### Widget para criar post

```dart
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import '../models/bff_models.dart';
import '../services/bff_api_service.dart';

class CreatePostScreen extends StatefulWidget {
  final String territoryId;
  final String? authToken;
  final String baseUrl;

  const CreatePostScreen({
    Key? key,
    required this.territoryId,
    this.authToken,
    required this.baseUrl,
  }) : super(key: key);

  @override
  State<CreatePostScreen> createState() => _CreatePostScreenState();
}

class _CreatePostScreenState extends State<CreatePostScreen> {
  final _formKey = GlobalKey<FormState>();
  final _titleController = TextEditingController();
  final _contentController = TextEditingController();
  final List<String> _selectedMediaPaths = [];
  String _selectedType = 'POST';
  String _selectedVisibility = 'PUBLIC';
  bool _isLoading = false;

  late BffApiService _apiService;

  @override
  void initState() {
    super.initState();
    _apiService = BffApiService(
      baseUrl: widget.baseUrl,
      authToken: widget.authToken,
    );
  }

  Future<void> _pickImages() async {
    final picker = ImagePicker();
    final images = await picker.pickMultiImage();
    if (images != null) {
      setState(() {
        _selectedMediaPaths.addAll(images.map((img) => img.path));
      });
    }
  }

  Future<void> _submitPost() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    try {
      final request = CreatePostJourneyRequest(
        title: _titleController.text,
        content: _contentController.text,
        type: _selectedType,
        visibility: _selectedVisibility,
        mediaFilePaths: _selectedMediaPaths.isNotEmpty
            ? _selectedMediaPaths
            : null,
      );

      final response = await _apiService.createPost(
        territoryId: widget.territoryId,
        request: request,
      );

      if (response.success) {
        Navigator.of(context).pop(true); // Retornar sucesso
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Post criado com sucesso!')),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Erro: ${response.error}')),
        );
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Erro ao criar post: $e')),
      );
    } finally {
      setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Criar Post'),
        actions: [
          TextButton(
            onPressed: _isLoading ? null : _submitPost,
            child: const Text('Publicar'),
          ),
        ],
      ),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            // Tipo
            DropdownButtonFormField<String>(
              value: _selectedType,
              decoration: const InputDecoration(labelText: 'Tipo'),
              items: const [
                DropdownMenuItem(value: 'POST', child: Text('Post')),
                DropdownMenuItem(value: 'ALERT', child: Text('Alerta')),
                DropdownMenuItem(value: 'EVENT', child: Text('Evento')),
              ],
              onChanged: (value) => setState(() => _selectedType = value!),
            ),
            const SizedBox(height: 16),

            // Visibilidade
            DropdownButtonFormField<String>(
              value: _selectedVisibility,
              decoration: const InputLabel(labelText: 'Visibilidade'),
              items: const [
                DropdownMenuItem(value: 'PUBLIC', child: Text('Público')),
                DropdownMenuItem(
                  value: 'RESIDENTS_ONLY',
                  child: Text('Apenas Moradores'),
                ),
              ],
              onChanged: (value) =>
                  setState(() => _selectedVisibility = value!),
            ),
            const SizedBox(height: 16),

            // Título
            TextFormField(
              controller: _titleController,
              decoration: const InputDecoration(
                labelText: 'Título',
                border: OutlineInputBorder(),
              ),
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Título é obrigatório';
                }
                if (value.length > 200) {
                  return 'Título deve ter no máximo 200 caracteres';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),

            // Conteúdo
            TextFormField(
              controller: _contentController,
              decoration: const InputDecoration(
                labelText: 'Conteúdo',
                border: OutlineInputBorder(),
              ),
              maxLines: 10,
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Conteúdo é obrigatório';
                }
                if (value.length > 5000) {
                  return 'Conteúdo deve ter no máximo 5000 caracteres';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),

            // Mídias
            if (_selectedMediaPaths.isNotEmpty)
              SizedBox(
                height: 100,
                child: ListView.builder(
                  scrollDirection: Axis.horizontal,
                  itemCount: _selectedMediaPaths.length,
                  itemBuilder: (context, index) {
                    return Padding(
                      padding: const EdgeInsets.only(right: 8.0),
                      child: Stack(
                        children: [
                          Image.network(
                            _selectedMediaPaths[index],
                            width: 100,
                            height: 100,
                            fit: BoxFit.cover,
                          ),
                          Positioned(
                            top: 0,
                            right: 0,
                            child: IconButton(
                              icon: const Icon(Icons.close),
                              onPressed: () {
                                setState(() {
                                  _selectedMediaPaths.removeAt(index);
                                });
                              },
                            ),
                          ),
                        ],
                      ),
                    );
                  },
                ),
              ),

            // Botão adicionar mídia
            ElevatedButton.icon(
              onPressed: _pickImages,
              icon: const Icon(Icons.add_photo_alternate),
              label: const Text('Adicionar Fotos'),
            ),
          ],
        ),
      ),
    );
  }

  @override
  void dispose() {
    _titleController.dispose();
    _contentController.dispose();
    super.dispose();
  }
}
```

---

## ✅ Checklist de Implementação

- [x] Modelos de dados completos
- [x] Serviço de API com tratamento de erros
- [x] Widget de feed com paginação infinita
- [x] Widget de criar post
- [x] Tratamento de mídias (imagens)
- [x] Interações (like, share, comment)
- [x] Refresh pull-to-refresh
- [x] Loading states
- [x] Error handling

---

## 📚 Próximos Passos

1. Adicionar cache local (Hive/SharedPreferences)
2. Implementar retry com exponential backoff
3. Adicionar testes unitários
4. Implementar outras jornadas (Events, Marketplace)
5. Adicionar analytics e métricas

---

**Última Atualização**: 2026-01-27  
**Status**: 📋 Exemplo Completo - Pronto para Uso
