import 'package:flutter/material.dart';

/// Renderiza um glifo da família Material Symbols Rounded usando ligatures,
/// alinhando os nomes sem depender do carregamento da fonte variável em tests.
class ArahMaterialSymbol extends StatelessWidget {
  const ArahMaterialSymbol(
    this.name, {
    super.key,
    required this.size,
    required this.color,
    this.fill = 0,
    this.weight = 500,
    this.grade = 0,
    this.opticalSize = 24,
  });

  final String name;
  final double size;
  final Color color;
  final int fill;
  final double weight;
  final double grade;
  final double opticalSize;

  @override
  Widget build(BuildContext context) {
    final offset = _resolveOpticalOffset(name, size);
    return SizedBox.square(
      dimension: size,
      child: Center(
        child: Transform.translate(
          offset: offset,
          child: Icon(
            _resolveIcon(name, filled: fill > 0),
            size: size,
            color: color,
          ),
        ),
      ),
    );
  }

  Offset _resolveOpticalOffset(String value, double iconSize) {
    return Offset.zero;
  }

  IconData _resolveIcon(String value, {required bool filled}) {
    switch (value) {
      case 'add':
        return Icons.add;
      case 'bookmark_border':
        return Icons.bookmark_border;
      case 'check_circle':
        return filled ? Icons.check_circle : Icons.check_circle_outline;
      case 'chevron_right':
        return Icons.chevron_right;
      case 'cottage':
        return filled ? Icons.cottage : Icons.cottage_outlined;
      case 'eco':
        return filled ? Icons.eco : Icons.eco_outlined;
      case 'explore':
        return filled ? Icons.explore : Icons.explore_outlined;
      case 'favorite':
        return filled ? Icons.favorite : Icons.favorite_border;
      case 'forest':
        return Icons.forest;
      case 'forum':
        return filled ? Icons.forum : Icons.forum_outlined;
      case 'grid_view':
        return filled ? Icons.grid_view : Icons.grid_view_outlined;
      case 'ios_share':
        return Icons.ios_share;
      case 'location_on':
        return filled ? Icons.location_on : Icons.location_on_outlined;
      case 'lock':
        return filled ? Icons.lock : Icons.lock_outline;
      case 'mode_comment':
        return filled ? Icons.mode_comment : Icons.mode_comment_outlined;
      case 'notifications':
        return filled ? Icons.notifications : Icons.notifications_outlined;
      case 'person':
        return filled ? Icons.person : Icons.person_outline;
      case 'push_pin':
        return filled ? Icons.push_pin : Icons.push_pin_outlined;
      case 'send':
        return filled ? Icons.send : Icons.send_outlined;
      case 'unfold_more':
        return Icons.unfold_more;
      case 'warning':
        return filled ? Icons.warning : Icons.warning_amber_outlined;
      default:
        return Icons.circle;
    }
  }
}
