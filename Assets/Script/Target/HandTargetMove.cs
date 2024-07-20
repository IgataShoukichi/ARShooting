using System;
using System.Collections.Generic;
using UnityEngine;

public class HandTargetMove : MonoBehaviour
{
    public static bool isNear = false;//プレイヤーに近づいている状態かどうかのフラグ
    [SerializeField] private static List<HandTargetMove> allInstances = new List<HandTargetMove>();//すべてのインスタンスを管理するリスト

    private GameManager gameManager;//ゲームマネージャーの参照
    private TargetManager myTargetManager;//自身のターゲットマネージャー
    [SerializeField] private Rigidbody rigidbodyComponent;//Rigidbodyコンポーネント（インスペクターから設定可能）
    [SerializeField] private Collider colliderComponent;//Colliderコンポーネント（インスペクターから設定可能）
    [SerializeField] private Camera mainCamera;//メインカメラの参照

    //パラメーター
    [Tooltip("プレイヤーに近づくスピード")]
    [SerializeField] private float targetSpeed = 1f;
    [Tooltip("プレイヤーとの最小の距離")]
    [SerializeField] private float minDistance = 1f;
    [Tooltip("ダメージを与える距離")]
    [SerializeField] private float moveDistance = 1f;
    [Tooltip("ダメージを与える間隔時間")]
    [SerializeField] private float damageInterval = 2f;
    private float timer;//ダメージを与えるためのタイマー
    [Tooltip("掴んだ状態での移動速度")]
    [SerializeField] private float isGrabbedMoveSpeed = 50f;

    [NonSerialized] public static bool hasVibrated = false;//バイブレーションを行ったかどうかのフラグ

    void Awake()
    {
        InitializeComponents();//コンポーネントの初期化
        gameManager = GameManager.Instance;//ゲームマネージャーのインスタンスを取得
        mainCamera = Camera.main;//メインカメラを取得

        allInstances.RemoveAll(item => item == null);//nullの要素を削除
        allInstances.Add(this);//自分自身を全てのインスタンスを管理するリストに追加

        if (isNear)
        {
            StopMovement();//プレイヤーに近づいている状態なら移動を停止する
        }
    }

    void FixedUpdate()
    {
        if (rigidbodyComponent == null)
        {
            return;//Rigidbodyが存在しない場合は処理をスキップ
        }

        float distance = Vector3.Distance(myTargetManager.playerTransform.position, transform.position);
        transform.LookAt(myTargetManager.playerTransform);//プレイヤーの方向を向く

        if (distance >= minDistance && !isNear)
        {
            MoveTowardsPlayer();//プレイヤーに向かって移動
        }
        else if (distance <= minDistance)
        {
            HandleProximityToPlayer();//プレイヤーに近づいた際の処理
        }

        if (isNear)
        {
            AccumulateTimerAndDealDamage(distance);//タイマーを累積し、ダメージを与える処理
            MoveToFrontOfCamera(distance);//カメラの前に移動する
        }
    }

    //コンポーネントの初期化
    private void InitializeComponents()
    {
        myTargetManager = GetComponent<TargetManager>();//ターゲットマネージャーを取得

        EnsureComponents();//コンポーネントの存在を確認し、必要に応じて再取得する
        if (rigidbodyComponent == null)
        {
            return;//Rigidbodyが存在しない場合は処理をスキップ
        }
    }

    //コンポーネントの存在を確認し、必要に応じて再取得する
    private void EnsureComponents()
    {
        if (rigidbodyComponent == null)
        {
            rigidbodyComponent = GetComponent<Rigidbody>();//Rigidbodyコンポーネントを取得
        }

        if (colliderComponent == null)
        {
            colliderComponent = GetComponent<Collider>();//Colliderコンポーネントを取得
        }
    }

    //プレイヤーに向かって移動する
    private void MoveTowardsPlayer()
    {
        Vector3 localDirection = new Vector3(0, 0, targetSpeed);//ローカル方向の移動ベクトルを設定
        Vector3 worldDirection = transform.TransformDirection(localDirection);//ワールド方向に変換
        rigidbodyComponent.velocity = worldDirection;//Rigidbodyの速度を設定
        colliderComponent.isTrigger = false;//Colliderを通常の状態に設定
    }

    //プレイヤーに近づいた際の処理
    private void HandleProximityToPlayer()
    {
        rigidbodyComponent.velocity = Vector3.zero;//移動を停止
        colliderComponent.isTrigger = true;//Colliderをトリガー状態に設定
        gameManager.IsGrabbed = true;//ゲームマネージャーに掴んだ状態を通知
        //PerformVibration();//バイブレーション処理を実行

        //最初のオブジェクトがisNearをtrueにした場合の処理
        if (!isNear)
        {
            isNear = true;//isNearフラグをtrueに設定
            //他のすべてのインスタンスの動作を停止する
            foreach (var instance in allInstances)
            {
                if (instance != this)
                {
                    instance.StopMovement();//動作を停止
                }
            }
        }
    }

    //動作を停止するメソッド
    private void StopMovement()
    {
        EnsureComponents();//コンポーネントの存在を確認

        if (rigidbodyComponent == null)
        {
            return;//Rigidbodyが存在しない場合は処理をスキップ
        }

        rigidbodyComponent.velocity = Vector3.zero;//移動を停止
        colliderComponent.isTrigger = true;//Colliderをトリガー状態に設定
    }

    //カメラの前に移動する
    private void MoveToFrontOfCamera(float cameraDistance)
    {
        if (cameraDistance <= moveDistance)
        {
            Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * minDistance;//カメラの前方の目標位置を計算
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, isGrabbedMoveSpeed * Time.deltaTime);//目標位置に向かって移動
        }
    }

    //タイマーを累積し、ダメージを与える
    private void AccumulateTimerAndDealDamage(float damageDistance)
    {
        timer += Time.deltaTime;//タイマーを累積
        if (timer >= damageInterval && damageDistance <= moveDistance)
        {
            myTargetManager.DealDamage();//ダメージを与える
            timer = 0f;//タイマーをリセット
        }
    }

    //バイブレーション処理を実行
    /*
    private void PerformVibration()
    {
        if (!hasVibrated)
        {
            VibrateScript.Instance.StartVibration();//バイブレーションを開始
            hasVibrated = true;//バイブレーションが実行されたことを記録
        }
    }
    */
}
