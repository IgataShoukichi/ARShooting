using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class ARGunController : MonoBehaviour
{
    private GameManager GM;//GameManager
    [SerializeField] private ARCameraManager arCameraManager;//ARカメラマネージャー

    //パラメーター
    [Tooltip("銃弾のプレハブ")]
    [SerializeField] private GameObject bulletPrefab;
    [Tooltip("銃弾の速度")]
    [SerializeField] private float bulletSpeed = 5f;
    [Tooltip("パワーの最大値")]
    [SerializeField] private float maxPower = 30f;
    [Tooltip("パワー増加の間隔（秒）")]
    [SerializeField] private float incrementInterval = 0.5f;
    [Tooltip("銃弾の寿命（秒）")]
    [SerializeField] private float bulletLifetime = 5f;
    [Tooltip("パワーUIの配列")]
    [SerializeField] private GameObject[] powerUI;
    [Tooltip("UIの点滅の間隔（秒）")]
    [SerializeField] private float blinkInterval = 0.5f;
    [Tooltip("レベルの上限")]
    [SerializeField] int levelCap = 10;
    [Tooltip("最初のレベルアップに必要な経験値")]
    [SerializeField] int experienceToLevelUp = 100;
    [Tooltip("レベルアップごとに必要な経験値を増加させる割合")]
    [SerializeField] float levelUpMultiplier = 1.5f;
    [Tooltip("レベルアップ時の弾丸速度の増加率 (%)")]
    [SerializeField] private float speedBulletIncreaseRate = 10f;
    [Tooltip("レベルアップ時の弾丸威力の増加率 (%)")]
    [SerializeField] private float powerBulletIncreaseRate = 10f;

    //サウンド
    [Tooltip("力を溜めているときの音の配列")]
    [SerializeField] private AudioClip[] incrementPowerSounds;
    private int currentPowerStage = -1;
    [Tooltip("発砲音")]
    [SerializeField] private AudioClip shootSound;
    private AudioSource audioSource;

    private float lastIncrementTime;//前回パワーを増加させた時刻
    private float power = 0f;//現在のパワー

    private float lastBlinkTime;//前回UIのアクティブ状態を切り替えた時刻
    private bool lastPowerUIState = false;//最後のUIのアクティブ状態
    public int currentExperience { get; private set; }//現在の経験値
    private int currentLevel = 1;//現在のレベル

    private void Update()
    {
        if (!GM.IsGrabbed)
        {
            HandleTouchInput();

            if (!GM.UIcontroller.reticleUI.enabled)
            {
                GM.UIcontroller.reticleUI.enabled = true;
            }
        }
        else
        {
            //射撃制限中
            GM.UIcontroller.reticleUI.enabled = false;
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ShootingGame") // ゲームシーンの名前を指定
        {
            InitializeARGun();
        }
    }
    public void InitializeARGun()
    {
        currentExperience = 0;
        currentLevel = 1;

        //ARCameraManagerの参照を取得
        arCameraManager = FindObjectOfType<ARCameraManager>();
        if (arCameraManager == null)
        {
            Debug.LogError("ARCameraManagerがシーンに存在していません-ARGunController");
            return;
        }

        //GameManagerのインスタンスを取得
        GM = GameManager.Instance;

        //パワーUIを隠す
        HidePowerUI();

        //AudioSourceの初期化
        audioSource = gameObject.AddComponent<AudioSource>();

        Debug.Log("ARGunの初期化完了");
    }

    //タッチ入力の処理を行う。
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                //タッチが始まった時、パワーをリセット
                ResetPower();
            }
            else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                //長押し中にパワーを増加させ、UIを更新
                IncrementPower();
                UpdatePowerUI();
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                //タッチが終了した時、現在のパワーで弾を発射
                Shoot(power);
                HidePowerUI();
            }
        }
    }

    //パワーをリセットし、開始時刻を記録する。
    private void ResetPower()
    {
        power = 0f;
        lastIncrementTime = Time.time;
        currentPowerStage = -1;
    }

    //パワーを一定間隔で増加させる。
    private void IncrementPower()
    {
        if (Time.time - lastIncrementTime >= incrementInterval)
        {
            float stageIncrement = maxPower / powerUI.Length;
            power += stageIncrement;
            lastIncrementTime = Time.time;

            int newPowerStage = Mathf.FloorToInt(power / stageIncrement);

            if (newPowerStage != currentPowerStage)
            {
                currentPowerStage = newPowerStage;

                //力を溜めている音を再生
                if (incrementPowerSounds != null && audioSource != null && currentPowerStage < incrementPowerSounds.Length)
                {
                    audioSource.PlayOneShot(incrementPowerSounds[currentPowerStage]);
                }
            }
        }
    }

    //現在のパワーを使用して銃弾を発射する。
    private void Shoot(float power)
    {
        GM.gunPower = power;

        if (arCameraManager == null || !arCameraManager.TryGetIntrinsics(out _))
        {
            Debug.LogWarning("AR Cameraが利用できません-ARGunController");
            return;
        }

        //カメラの現在の位置と方向を取得
        Transform cameraTransform = arCameraManager.transform;
        Vector3 bulletSpawnPosition = cameraTransform.position + cameraTransform.forward;

        //銃弾の生成
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);

        //銃弾の速度を設定
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        if (bulletRigidbody != null)
        {
            bulletRigidbody.velocity = cameraTransform.forward * (bulletSpeed + power);
        }

        //銃弾を一定時間後に削除
        Destroy(bullet, bulletLifetime);

        //発砲音を再生
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    //パワーUIを更新する。
    private void UpdatePowerUI()
    {
        int powerStage = Mathf.FloorToInt(power / (maxPower / powerUI.Length));

        if (powerStage >= 0 && powerStage < powerUI.Length)
        {
            //全てのUIを非表示に
            HidePowerUI();
            //現在のパワーステージに応じてUIを表示
            powerUI[powerStage].SetActive(true);
        }
        else if (power >= maxPower)
        {
            //maxPowerに達した場合、最後のUIを点滅させる
            float currentTime = Time.time;

            if (currentTime - lastBlinkTime >= blinkInterval)
            {
                lastPowerUIState = !lastPowerUIState;
                powerUI[powerUI.Length - 1].SetActive(lastPowerUIState);
                lastBlinkTime = currentTime;
            }
        }
    }

    //パワーUIを非表示にする。
    private void HidePowerUI()
    {
        foreach (var uiElement in powerUI)
        {
            uiElement.SetActive(false);
        }
    }

    //指定された経験値を追加する。
    public void AddExperience(int AddEXP)
    {
        currentExperience += AddEXP;
        CheckLevelUp();
    }

    //レベルアップのチェックを行う
    private void CheckLevelUp()
    {
        if (currentLevel >= levelCap)
        {
            return; //現在のレベルが上限に達している場合、何もせずに終了
        }

        while (currentExperience >= experienceToLevelUp)
        {
            currentExperience -= experienceToLevelUp;
            currentLevel++;
            OnLevelUp();

            //上限に達したらループを終了
            if (currentLevel >= levelCap)
            {
                break;
            }

            experienceToLevelUp = Mathf.CeilToInt(experienceToLevelUp * levelUpMultiplier);
        }
    }

    //レベルアップ時の処理
    private void OnLevelUp()
    {
        Debug.Log($"Level Up! New Level: {currentLevel}");

        //百分率から増加倍率を計算
        float speedBulletMultiplier = 1 + (speedBulletIncreaseRate / 100f);
        float powerBulletMultiplier = 1 + (powerBulletIncreaseRate / 100f);

        //弾丸の速度と威力を増加させる
        bulletSpeed *= speedBulletMultiplier;
        maxPower *= powerBulletMultiplier;

        //GameManagerに通知
        GM.OnPlayerLevelUp(currentLevel);
    }
}
