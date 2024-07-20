using UnityEngine;
using System.Collections;

public class CameraPositionAdjuster : MonoBehaviour
{
    [Tooltip("カメラのTransformをInspectorで設定")]
    [SerializeField] private Transform cameraTransform;
    [Tooltip("調整間隔（秒）")]
    [SerializeField] private float interval = 1.0f;
    [Tooltip("固定するY軸の位置")]
    [SerializeField] private float fixedYPosition = 1.5f;
    [Tooltip("スムージングの係数")]
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
        // カメラの現在位置を取得
        Vector3 currentPosition = cameraTransform.position;

        // Y軸の位置を固定
        Vector3 targetPosition = new Vector3(currentPosition.x, fixedYPosition, currentPosition.z);

        // スムージング処理
        Vector3 smoothedPosition = Vector3.Lerp(currentPosition, targetPosition, smoothingFactor);

        // カメラの位置を更新
        cameraTransform.position = smoothedPosition;
    }
}
