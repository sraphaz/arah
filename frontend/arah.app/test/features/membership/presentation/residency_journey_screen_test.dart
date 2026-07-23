import 'package:arah_app/features/membership/presentation/screens/residency_journey_screen.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('buildResidencyMessageWithProof uploads only once across retries',
      () async {
    var uploadCalls = 0;

    final firstResult = await buildResidencyMessageWithProof(
      rawMessage: '',
      proofImagePath: '/tmp/proof.png',
      proofFileName: 'proof.png',
      uploadedProofMediaId: null,
      uploadProof: ({
        required String filePath,
        required String fileName,
      }) async {
        uploadCalls += 1;
        return 'media-1';
      },
    );

    final retryResult = await buildResidencyMessageWithProof(
      rawMessage: '',
      proofImagePath: '/tmp/proof.png',
      proofFileName: 'proof.png',
      uploadedProofMediaId: firstResult.uploadedProofMediaId,
      uploadProof: ({
        required String filePath,
        required String fileName,
      }) async {
        uploadCalls += 1;
        return 'media-2';
      },
    );

    expect(uploadCalls, 1);
    expect(firstResult.message, '[comprovante: media:media-1]');
    expect(firstResult.uploadedProofMediaId, 'media-1');
    expect(retryResult.message, '[comprovante: media:media-1]');
    expect(retryResult.uploadedProofMediaId, 'media-1');
  });

  test(
      'residencyFallbackErrorMessage uses request copy after a successful upload',
      () {
    expect(
      residencyFallbackErrorMessage(
        uploadCompleted: true,
        uploadErrorMessage: 'upload',
        requestErrorMessage: 'request',
      ),
      'request',
    );
    expect(
      residencyFallbackErrorMessage(
        uploadCompleted: false,
        uploadErrorMessage: 'upload',
        requestErrorMessage: 'request',
      ),
      'upload',
    );
  });

  test(
      'residencyProofUploadCompleted tracks whether proof upload already finished',
      () {
    expect(
      residencyProofUploadCompleted(
        proofImagePath: '/tmp/proof.png',
        uploadedProofMediaId: null,
      ),
      isFalse,
    );
    expect(
      residencyProofUploadCompleted(
        proofImagePath: '/tmp/proof.png',
        uploadedProofMediaId: 'media-1',
      ),
      isTrue,
    );
    expect(
      residencyProofUploadCompleted(
        proofImagePath: null,
        uploadedProofMediaId: null,
      ),
      isTrue,
    );
  });
}
