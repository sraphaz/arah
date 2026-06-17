import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../providers/chat_provider.dart';

class ChatConversationScreen extends ConsumerStatefulWidget {
  const ChatConversationScreen({super.key, required this.conversationId, this.title});

  final String conversationId;
  final String? title;

  @override
  ConsumerState<ChatConversationScreen> createState() => _ChatConversationScreenState();
}

class _ChatConversationScreenState extends ConsumerState<ChatConversationScreen> {
  final _controller = TextEditingController();
  bool _sending = false;

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  Future<void> _send() async {
    if (_sending) return;
    setState(() => _sending = true);
    try {
      await ref.read(chatConversationProvider(widget.conversationId).notifier).send(_controller.text);
      _controller.clear();
    } catch (e) {
      if (mounted) {
        showErrorSnackBar(
          context,
          e is ApiException ? e.userMessage : 'Erro ao enviar mensagem.',
        );
      }
    } finally {
      if (mounted) setState(() => _sending = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(chatConversationProvider(widget.conversationId));
    final timeFormat = DateFormat('HH:mm');

    return Scaffold(
      appBar: AppBar(title: Text(widget.title ?? 'Conversa')),
      body: Column(
        children: [
          Expanded(
            child: state.isLoading && state.messages.isEmpty
                ? const Center(child: CircularProgressIndicator())
                : ListView.builder(
                    padding: const EdgeInsets.all(AppConstants.spacingMd),
                    itemCount: state.messages.length,
                    itemBuilder: (context, index) {
                      final msg = state.messages[index];
                      return Align(
                        alignment: Alignment.centerLeft,
                        child: Container(
                          margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
                          padding: const EdgeInsets.all(AppConstants.spacingSm),
                          decoration: BoxDecoration(
                            color: Theme.of(context).colorScheme.surfaceContainerHighest,
                            borderRadius: BorderRadius.circular(AppConstants.radiusMd),
                          ),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(msg.text),
                              Text(
                                timeFormat.format(msg.createdAtUtc.toLocal()),
                                style: Theme.of(context).textTheme.labelSmall,
                              ),
                            ],
                          ),
                        ),
                      );
                    },
                  ),
          ),
          SafeArea(
            child: Padding(
              padding: const EdgeInsets.all(AppConstants.spacingSm),
              child: Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _controller,
                      decoration: const InputDecoration(
                        hintText: 'Mensagem',
                        border: OutlineInputBorder(),
                      ),
                      onSubmitted: (_) => _send(),
                    ),
                  ),
                  IconButton(
                    onPressed: _sending ? null : _send,
                    icon: _sending
                        ? const SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          )
                        : const Icon(Icons.send),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
