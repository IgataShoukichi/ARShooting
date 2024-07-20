using UnityEngine;
using System.Collections;

public class CameraPositionAdjuster : MonoBehaviour
{
    [Tooltip("�J������Transform��Inspector�Őݒ�")]
    [SerializeField] private Transform cameraTransform;
    [Tooltip("�����Ԋu�i�b�j")]
    [SerializeField] private float interval = 1.0f;
    [Tooltip("�Œ肷��Y���̈ʒu")]
    [SerializeField] private float fixedYPosition = 1.5f;
    [Tooltip("�X���[�W���O�̌W��")]
    [SerializeField] private float smoothingFactor = 0.1f;

    void Start()
    {
        StartCoroutine(AdjustCameraPositionRoutine());
    }

    IEnumerator AdjustCameraPositionRoutine()
    {
        while (true)
        {
            AdjustCameraPosition();
            yield return new WaitForSeconds(interval);
        }
    }

    void AdjustCameraPosition()
    {
        // �J�����̌��݈ʒu���擾
        Vector3 currentPosition = cameraTransform.position;

        // Y���̈ʒu���Œ�
        Vector3 targetPosition = new Vector3(currentPosition.x, fixedYPosition, currentPosition.z);

        // �X���[�W���O����
        Vector3 smoothedPosition = Vector3.Lerp(currentPosition, targetPosition, smoothingFactor);

        // �J�����̈ʒu���X�V
        cameraTransform.position = smoothedPosition;
    }
}
