using Unity.VisualScripting;
using UnityEngine;

public class MissileTargetMove : MonoBehaviour
{
    private TargetManager myTargetManager;//自身のTargetManager

    //パラメーター
    [Tooltip("プレイヤーに近づくスピード")]
    [SerializeField] private float targetSpeed = 1.5f;

    [Tooltip("相手との最小の距離")]
    [SerializeField] private float minDistance = 0.5f;

    [Tooltip("セルフエフェクト")]
    [SerializeField] private ParticleSystem selfEffect;

    private Rigidbody myRig;//このオブジェクトのRigidbody
    private bool self;//自滅かどうかを判断する変数

    void Start()
    {
        //自身のTargetManagerを取得
        myTargetManager = GetComponent<TargetManager>();

        //Rigidbodyを取得
        myRig = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //プレイヤーとの距離を測る
        var distance = Vector3.Distance(myTargetManager.playerTransform.position, transform.position);

        //プレイヤーの方向を向く
        transform.LookAt(myTargetManager.playerTransform);

        //一定の距離まで近づく
        if (distance >= minDistance)
        {
            //プレイヤーの方向へ進行する
            Vector3 localDirection = new Vector3(0, 0, targetSpeed);
            Vector3 worldDirection = transform.TransformDirection(localDirection);
            myRig.velocity = worldDirection;
        }
        else //distance < minDistance の場合
        {
            //移動を止める
            myRig.velocity = Vector3.zero;

            //プレイヤーにダメージを与える
            myTargetManager.DealDamage();

            //自爆でカウントなし
            self = true;

            //自爆の時のエフェクト再生
            myTargetManager.PlayEffect(selfEffect);

            //オブジェクトを消滅させる
            StartCoroutine(myTargetManager.DieCoroutine(myTargetManager.myType, self));
        }
    }
}
