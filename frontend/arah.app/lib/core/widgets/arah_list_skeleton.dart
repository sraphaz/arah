import 'package:flutter/material.dart';

import '../config/constants.dart';
import 'arah_card.dart';
import 'shimmer_skeleton.dart';

/// Skeleton genérico para listas (substitui CircularProgressIndicator central).
class ArahListSkeleton extends StatelessWidget {
  const ArahListSkeleton({
    super.key,
    this.itemCount = 4,
    this.itemHeight = 72,
  });

  final int itemCount;
  final double itemHeight;

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      physics: const AlwaysScrollableScrollPhysics(),
      itemCount: itemCount,
      itemBuilder: (_, __) => Padding(
        padding: const EdgeInsets.symmetric(
          horizontal: AppConstants.spacingMd,
          vertical: AppConstants.spacingXs,
        ),
        child: ArahCard(
          padding: const EdgeInsets.all(AppConstants.spacingMd),
          child: ShimmerBox(
            width: double.infinity,
            height: itemHeight - AppConstants.spacingMd * 2,
            borderRadius: BorderRadius.circular(AppConstants.radiusSm),
          ),
        ),
      ),
    );
  }
}
