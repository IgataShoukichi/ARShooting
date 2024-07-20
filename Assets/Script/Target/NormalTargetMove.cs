using UnityEngine;
public class NormalTargetMove : MonoBehaviour
{
    private TargetManager myTargetManager;//TargetManager

    //パラメーター
    [Tooltip("上下移動の速度")]
    [SerializeField] private float UDspeed = 2f;
    [Tooltip("左右移動の速度")]
    [SerializeField] private float LRspeed = 2f;
    [Tooltip("アルファ値の変化の速度")]
    [SerializeField] private float alphaChangeSpeed = 0.5f;
    [Tooltip("拡大縮小の速度")]
    [SerializeField] private float scaleChangeSpeed = 0.1f;
    [Tooltip("プレイヤーに近づくスピード")]
    [SerializeField] private float targetSpeed = 1f;
    [Tooltip("相手との最小の距離")]
    [SerializeField] private float minDistance = 1f;

    //オブジェクトの動きの種類を定義する列挙型
    public enum ActionPattern
    {
        UPDOWN,    //上下移動
        LEFTRIGHT, //左右移動
        ALPHA,     //アルファ値の変化
        SCALE      //拡大縮小
    }
    private ActionPattern currentPattern;//現在の動きの種類

    //初期のアルファ値とスケール
    private float initialAlpha;
    private Vector3 initialScale;

    private new Renderer renderer;//スプライトのレンダラー

    private Rigidbody rb;

    void Start()
    {
        myTargetManager = GetComponent<TargetManager>(); // TargetManagerコンポーネントを取得

        rb = GetComponent<Rigidbody>();

        //スプライトのレンダラーを取得
        //SpriteRendererがアタッチされていない場合、新しく追加する
        if (GetComponent<Renderer>() == null)
        {
            renderer = gameObject.AddComponent<Renderer>();
        }
        else
        {
            renderer = GetComponent<Renderer>();
        }
        //初期のアルファ値を保存
        initialAlpha = renderer.material.color.a;

        //初期のスケールを保存
        initialScale = transform.localScale;

        //ランダムな動きの種類を選択して開始
        SwitchMovementPattern();

        //プレイヤーの位置をTagから取得
        GameObject Player = GameObject.FindWithTag("Player");
        myTargetManager.playerTransform = Player.transform;
    }

    void Update()
    {
        //プレイヤーとの距離を測る
        var distance = Vector3.Distance(myTargetManager.playerTransform.transform.position, transform.position);

        //現在の動きの種類に応じて動作を実行
        switch (currentPattern)
        {
            case ActionPattern.UPDOWN:
                //上下移動
                transform.Translate(Vector3.up * Mathf.Sin(Time.time * UDspeed) * Time.deltaTime);
                break;

            case ActionPattern.LEFTRIGHT:
                //左右移動
                transform.Translate(Vector3.right * Mathf.Sin(Time.time * LRspeed) * Time.deltaTime);
                break;

            case ActionPattern.ALPHA:
                //アルファ値の変化
                float alphaValue = Mathf.PingPong(Time.time * alphaChangeSpeed, 1f);
                Color newColor = renderer.material.color;
                newColor.a = initialAlpha * alphaValue;
                renderer.material.color = newColor;
                // 子オブジェクトにも同じアルファ値を適用
                foreach (Transform child in transform)
                {
                    Renderer childRenderer = child.GetComponent<Renderer>();
                    if (childRenderer != null && childRenderer.material != null && childRenderer.material.HasProperty("_Color"))
                    {
                        Color childColor = childRenderer.material.color;
                        childColor.a = initialAlpha * alphaValue;
                        childRenderer.material.color = childColor;
                    }
                }
                break;

            case ActionPattern.SCALE:
                //拡大縮小
                float scaleValue = Mathf.PingPong(Time.time * scaleChangeSpeed, 1f);
                transform.localScale = initialScale * scaleValue;
                break;

            default:
                break;
        }

        //プレイヤーの方向を向く
        transform.LookAt(myTargetManager.playerTransform);

        //一定の距離まで近づく
        if (distance > minDistance)
        {
            //プレイヤーの方向へ進行する
            Vector3 localDirection = new Vector3(0, 0, targetSpeed);
            Vector3 worldDirection = transform.TransformDirection(localDirection);
            rb.velocity = worldDirection;
        }
        else
        {
            //前進を止める
            rb.velocity = Vector3.zero;
        }
    }

    void SwitchMovementPattern()//動作の切り替え関数
    {
        //ランダムな動きの種類を選択
        currentPattern = (ActionPattern)Random.Range(0, 4);
    }
}
