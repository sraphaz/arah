import 'package:flutter/material.dart';

import '../config/constants.dart';
import 'arah_card.dart';

/// Placeholder cinza com animação de pulsação (skeleton loading).
class ShimmerBox extends StatefulWidget {
  const ShimmerBox({
    super.key,
    required this.width,
    required this.height,
    this.borderRadius,
  });

  final double width;
  final double height;
  final BorderRadius? borderRadius;

  @override
  State<ShimmerBox> createState() => _ShimmerBoxState();
}

class _ShimmerBoxState extends State<ShimmerBox>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _animation;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 1200),
    )..repeat(reverse: true);
    _animation = Tween<double>(begin: 0.35, end: 0.65).animate(
      CurvedAnimation(parent: _controller, curve: Curves.easeInOut),
    );
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final baseColor = Theme.of(context).colorScheme.surfaceContainerHighest;
    return AnimatedBuilder(
      animation: _animation,
      builder: (context, child) {
        return Container(
          width: widget.width,
          height: widget.height,
          decoration: BoxDecoration(
            borderRadius: widget.borderRadius ??
                BorderRadius.circular(AppConstants.radiusSm),
            color: baseColor.withValues(alpha: 0.5 + _animation.value * 0.35),
          ),
        );
      },
    );
  }
}

/// Skeleton no formato de card do feed (avatar + linhas).
class FeedCardSkeleton extends StatelessWidget {
  const FeedCardSkeleton({super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(
        horizontal: AppConstants.spacingMd,
        vertical: AppConstants.spacingSm + 2,
      ),
      child: ArahCard(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const ShimmerBox(
                  width: 40,
                  height: 40,
                  borderRadius: BorderRadius.all(Radius.circular(20)),
                ),
                const SizedBox(width: AppConstants.spacingMd),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      ShimmerBox(
                        width: double.infinity,
                        height: 16,
                        borderRadius: BorderRadius.circular(AppConstants.radiusXs),
                      ),
                      const SizedBox(height: AppConstants.spacingXs + 2),
                      ShimmerBox(
                        width: 120,
                        height: 12,
                        borderRadius: BorderRadius.circular(AppConstants.radiusXs),
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppConstants.spacingMd),
            ShimmerBox(
              width: double.infinity,
              height: 14,
              borderRadius: BorderRadius.circular(AppConstants.radiusXs),
            ),
            const SizedBox(height: AppConstants.spacingXs + 2),
            ShimmerBox(
              width: double.infinity,
              height: 14,
              borderRadius: BorderRadius.circular(AppConstants.radiusXs),
            ),
            const SizedBox(height: AppConstants.spacingXs + 2),
            ShimmerBox(
              width: 200,
              height: 14,
              borderRadius: BorderRadius.circular(AppConstants.radiusXs),
            ),
            const SizedBox(height: AppConstants.spacingMd),
            Row(
              children: [
                ShimmerBox(
                  width: 80,
                  height: 36,
                  borderRadius: BorderRadius.circular(AppConstants.radiusSm),
                ),
                const SizedBox(width: AppConstants.spacingSm),
                ShimmerBox(
                  width: 80,
                  height: 36,
                  borderRadius: BorderRadius.circular(AppConstants.radiusSm),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

/// Lista de N cards skeleton para o feed.
class FeedSkeleton extends StatelessWidget {
  const FeedSkeleton({super.key, this.itemCount = 4});

  final int itemCount;

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      physics: const NeverScrollableScrollPhysics(),
      shrinkWrap: true,
      itemCount: itemCount,
      itemBuilder: (_, __) => const FeedCardSkeleton(),
    );
  }
}
