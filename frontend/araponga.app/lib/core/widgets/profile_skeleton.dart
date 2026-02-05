import 'package:flutter/material.dart';

import '../config/constants.dart';
import 'shimmer_skeleton.dart';

/// Skeleton da tela de perfil (avatar + linhas + list tiles).
class ProfileSkeleton extends StatelessWidget {
  const ProfileSkeleton({super.key});

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingLg),
      child: Column(
        children: [
          const SizedBox(height: AppConstants.spacingLg),
          Center(
            child: ShimmerBox(
              width: 96,
              height: 96,
              borderRadius: BorderRadius.all(Radius.circular(48)),
            ),
          ),
          const SizedBox(height: AppConstants.spacingMd),
          Center(
            child: ShimmerBox(
              width: 160,
              height: 24,
              borderRadius: BorderRadius.circular(4),
            ),
          ),
          const SizedBox(height: AppConstants.spacingSm),
          Center(
            child: ShimmerBox(
              width: 120,
              height: 16,
              borderRadius: BorderRadius.circular(4),
            ),
          ),
          const SizedBox(height: AppConstants.spacingXl),
          ShimmerBox(
            width: double.infinity,
            height: 48,
            borderRadius: BorderRadius.circular(8),
          ),
          const SizedBox(height: AppConstants.spacingSm),
          ShimmerBox(
            width: double.infinity,
            height: 48,
            borderRadius: BorderRadius.circular(8),
          ),
          const SizedBox(height: AppConstants.spacingSm),
          ShimmerBox(
            width: double.infinity,
            height: 48,
            borderRadius: BorderRadius.circular(8),
          ),
        ],
      ),
    );
  }
}
