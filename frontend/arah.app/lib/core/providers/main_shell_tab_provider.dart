import 'package:flutter_riverpod/flutter_riverpod.dart';

/// Índice da aba ativa no shell principal (bottom navigation).
/// Permite que outras telas (ex.: deep-link do mapa para o feed) solicitem
/// a troca de aba antes de navegar de volta para `/home`.
///
/// 0 = Feed · 1 = Explorar · 2 = Publicar · 3 = Notificações · 4 = Perfil
final mainShellTabProvider = StateProvider<int>((ref) => 0);
