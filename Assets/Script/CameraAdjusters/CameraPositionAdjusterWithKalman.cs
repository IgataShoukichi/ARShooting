using UnityEngine;

public class CameraPositionAdjusterWithKalman : MonoBehaviour
{
    [Tooltip("カメラのTransformをInspectorで設定")]
    [SerializeField] private Transform cameraTransform;
    [Tooltip("固定するY軸の位置")]
    [SerializeField] private float fixedYPosition = 1.5f;

    private KalmanFilter kalmanFilterX;
    private KalmanFilter kalmanFilterZ;

    void Start()
    {
        // カルマンフィルターの初期化（プロセスノイズと計測ノイズの共分散行列の初期化）
        kalmanFilterX = new KalmanFilter(0.1f, 0.1f);
        kalmanFilterZ = new KalmanFilter(0.1f, 0.1f);
    }

    void Update()
    {
        AdjustCameraPosition();
    }

    void AdjustCameraPosition()
    {
        // カメラの現在位置を取得
        Vector3 currentPosition = cameraTransform.position;

        // Y軸の位置を固定
        currentPosition.y = fixedYPosition;

        // カルマンフィルターを使用して位置を平滑化
        float filteredX = kalmanFilterX.Update(currentPosition.x);
        float filteredZ = kalmanFilterZ.Update(currentPosition.z);

        // カメラの位置を更新
        cameraTransform.position = new Vector3(filteredX, fixedYPosition, filteredZ);
    }

    // カルマンフィルターのクラス
    private class KalmanFilter
    {
        private float Q;  // プロセスノイズの共分散行列
        private float R;  // 計測ノイズの共分散行列
        private float P;  // 誤差共分散行列
        private float X;  // 値（状態変数）

        public KalmanFilter(float processNoise, float measurementNoise)
        {
            Q = processNoise;
            R = measurementNoise;
            P = 1.0f;
            X = 0.0f;
        }

        public float Update(float measurement)
        {
            // 予測ステップ
            P = P + Q;

            // 更新ステップ
            float K = P / (P + R);
            X = X + K * (measurement - X);
            P = (1 - K) * P;

            return X;
        }
    }
}
