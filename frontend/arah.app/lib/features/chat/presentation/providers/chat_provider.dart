import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/chat_models.dart';
import '../../data/repositories/chat_repository.dart';

enum ChatListTab { channels, groups }

class ChatListState {
  const ChatListState({
    this.channels = const [],
    this.groups = const [],
    this.isLoading = false,
    this.error,
    this.tab = ChatListTab.channels,
  });

  final List<ChatConversationSummary> channels;
  final List<ChatConversationSummary> groups;
  final bool isLoading;
  final Object? error;
  final ChatListTab tab;

  List<ChatConversationSummary> get activeItems =>
      tab == ChatListTab.channels ? channels : groups;
}

class ChatListNotifier extends StateNotifier<ChatListState> {
  ChatListNotifier(this._ref) : super(const ChatListState()) {
    refresh();
  }

  final Ref _ref;

  ChatRepository get _repo => ChatRepository(client: _ref.read(bffClientProvider));

  void setTab(ChatListTab tab) {
    state = ChatListState(
      channels: state.channels,
      groups: state.groups,
      tab: tab,
    );
    refresh();
  }

  Future<void> refresh() async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) {
      state = const ChatListState();
      return;
    }
    state = ChatListState(
      channels: state.channels,
      groups: state.groups,
      isLoading: true,
      tab: state.tab,
    );
    try {
      if (state.tab == ChatListTab.channels) {
        final channels = await _repo.listChannels(territoryId);
        state = ChatListState(channels: channels, groups: state.groups, tab: state.tab);
      } else {
        final groups = await _repo.listGroups(territoryId);
        state = ChatListState(channels: state.channels, groups: groups, tab: state.tab);
      }
    } catch (e) {
      state = ChatListState(error: e, tab: state.tab);
    }
  }

  Future<ChatConversationSummary> createGroup(String name) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) {
      throw StateError('Território não selecionado');
    }
    final group = await _repo.createGroup(territoryId: territoryId, name: name);
    await refresh();
    return group;
  }
}

class ChatConversationState {
  const ChatConversationState({
    this.messages = const [],
    this.isLoading = false,
    this.error,
  });

  final List<ChatMessage> messages;
  final bool isLoading;
  final Object? error;
}

class ChatConversationNotifier extends StateNotifier<ChatConversationState> {
  ChatConversationNotifier(this._ref, this.conversationId) : super(const ChatConversationState()) {
    refresh();
  }

  final Ref _ref;
  final String conversationId;

  ChatRepository get _repo => ChatRepository(client: _ref.read(bffClientProvider));

  Future<void> refresh() async {
    state = ChatConversationState(messages: state.messages, isLoading: true);
    try {
      final messages = await _repo.getMessages(conversationId);
      state = ChatConversationState(messages: messages.reversed.toList());
    } catch (e) {
      state = ChatConversationState(error: e);
    }
  }

  Future<void> send(String text) async {
    if (text.trim().isEmpty) return;
    final message = await _repo.sendMessage(conversationId: conversationId, text: text.trim());
    state = ChatConversationState(messages: [...state.messages, message]);
  }
}

final chatListProvider =
    StateNotifierProvider.autoDispose<ChatListNotifier, ChatListState>((ref) {
  return ChatListNotifier(ref);
});

final chatConversationProvider = StateNotifierProvider.autoDispose
    .family<ChatConversationNotifier, ChatConversationState, String>((ref, conversationId) {
  return ChatConversationNotifier(ref, conversationId);
});
