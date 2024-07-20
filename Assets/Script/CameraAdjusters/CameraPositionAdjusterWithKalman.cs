using UnityEngine;

public class CameraPositionAdjusterWithKalman : MonoBehaviour
{
    [Tooltip("�J������Transform��Inspector�Őݒ�")]
    [SerializeField] private Transform cameraTransform;
    [Tooltip("�Œ肷��Y���̈ʒu")]
    [SerializeField] private float fixedYPosition = 1.5f;

    private KalmanFilter kalmanFilterX;
    private KalmanFilter kalmanFilterZ;

    void Start()
    {
        // �J���}���t�B���^�[�̏������i�v���Z�X�m�C�Y�ƌv���m�C�Y�̋����U�s��̏������j
        kalmanFilterX = new KalmanFilter(0.1f, 0.1f);
        kalmanFilterZ = new KalmanFilter(0.1f, 0.1f);
    }

    void Update()
    {
        AdjustCameraPosition();
    }

    void AdjustCameraPosition()
    {
        // �J�����̌��݈ʒu���擾
        Vector3 currentPosition = cameraTransform.position;

        // Y���̈ʒu���Œ�
        currentPosition.y = fixedYPosition;

        // �J���}���t�B���^�[���g�p���Ĉʒu�𕽊���
        float filteredX = kalmanFilterX.Update(currentPosition.x);
        float filteredZ = kalmanFilterZ.Update(currentPosition.z);

        // �J�����̈ʒu���X�V
        cameraTransform.position = new Vector3(filteredX, fixedYPosition, filteredZ);
    }

    // �J���}���t�B���^�[�̃N���X
    private class KalmanFilter
    {
        private float Q;  // �v���Z�X�m�C�Y�̋����U�s��
        private float R;  // �v���m�C�Y�̋����U�s��
        private float P;  // �덷�����U�s��
        private float X;  // �l�i��ԕϐ��j

        public KalmanFilter(float processNoise, float measurementNoise)
        {
            Q = processNoise;
            R = measurementNoise;
            P = 1.0f;
            X = 0.0f;
        }

        public float Update(float measurement)
        {
            // �\���X�e�b�v
            P = P + Q;

            // �X�V�X�e�b�v
            float K = P / (P + R);
            X = X + K * (measurement - X);
            P = (1 - K) * P;

            return X;
        }
    }
}
