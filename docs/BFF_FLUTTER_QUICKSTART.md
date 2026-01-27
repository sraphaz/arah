# Quick Start - Flutter BFF API

**Data**: 2026-01-27  
**Versão**: 2.0.0

Exemplo mínimo e direto para começar a usar a API BFF em Flutter.

---

## 🚀 Exemplo Mínimo

### 1. Adicionar Dependências

```yaml
# pubspec.yaml
dependencies:
  http: ^0.13.5
```

### 2. Código Mínimo

```dart
import 'dart:convert';
import 'package:http/http.dart' as http;

class BffService {
  final String baseUrl;
  final String? token;

  BffService({required this.baseUrl, this.token});

  // Obter feed do território
  Future<Map<String, dynamic>> getFeed(String territoryId) async {
    final uri = Uri.parse('$baseUrl/api/v2/journeys/feed/territory-feed')
        .replace(queryParameters: {
      'territoryId': territoryId,
      'pageNumber': '1',
      'pageSize': '20',
    });

    final response = await http.get(uri, headers: {
      if (token != null) 'Authorization': 'Bearer $token',
    });

    if (response.statusCode == 200) {
      return jsonDecode(response.body) as Map<String, dynamic>;
    } else {
      throw Exception('Erro: ${response.statusCode}');
    }
  }
}

// Uso
void main() async {
  final service = BffService(
    baseUrl: 'https://api.araponga.com',
    token: 'seu_token_aqui',
  );

  try {
    final feed = await service.getFeed('territory-id-aqui');
    print('Feed carregado: ${feed['items'].length} itens');
  } catch (e) {
    print('Erro: $e');
  }
}
```

---

## 📱 Exemplo com Widget

```dart
import 'package:flutter/material.dart';
import 'dart:convert';
import 'package:http/http.dart' as http;

class FeedPage extends StatefulWidget {
  @override
  _FeedPageState createState() => _FeedPageState();
}

class _FeedPageState extends State<FeedPage> {
  List<dynamic> items = [];
  bool loading = true;

  @override
  void initState() {
    super.initState();
    _loadFeed();
  }

  Future<void> _loadFeed() async {
    setState(() => loading = true);

    try {
      final uri = Uri.parse('https://api.araponga.com/api/v2/journeys/feed/territory-feed')
          .replace(queryParameters: {
        'territoryId': 'seu-territory-id',
        'pageNumber': '1',
        'pageSize': '20',
      });

      final response = await http.get(uri, headers: {
        'Authorization': 'Bearer seu-token',
      });

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        setState(() {
          items = data['items'] as List;
          loading = false;
        });
      }
    } catch (e) {
      setState(() => loading = false);
      print('Erro: $e');
    }
  }

  @override
  Widget build(BuildContext context) {
    if (loading) {
      return Center(child: CircularProgressIndicator());
    }

    return ListView.builder(
      itemCount: items.length,
      itemBuilder: (context, index) {
        final item = items[index];
        return Card(
          child: ListTile(
            title: Text(item['post']['title']),
            subtitle: Text(item['post']['content']),
            trailing: Text('${item['counts']['likes']} likes'),
          ),
        );
      },
    );
  }
}
```

---

## 🔐 Autenticação

```dart
// Obter token
Future<String> getAuthToken() async {
  final response = await http.post(
    Uri.parse('https://api.araponga.com/api/v1/auth/social'),
    headers: {'Content-Type': 'application/json'},
    body: jsonEncode({
      'provider': 'GOOGLE',
      'token': 'seu-google-token',
    }),
  );

  if (response.statusCode == 200) {
    final data = jsonDecode(response.body);
    return data['token'] as String;
  }
  throw Exception('Falha na autenticação');
}
```

---

## ✅ Pronto!

Agora você pode usar a API BFF no seu app Flutter. Para exemplos completos, veja [BFF_FLUTTER_EXAMPLE.md](./BFF_FLUTTER_EXAMPLE.md).
