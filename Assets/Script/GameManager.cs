using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //シングルトンインスタンスを保持する静的変数
    private static GameManager _instance;
    private static readonly object _lock = new object();

    //シングルトンインスタンスのプロパティ
    public static GameManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        //ゲームオブジェクトを新規作成し、GameManagerをアタッチ
                        GameObject singletonObject = new GameObject("GameManager");
                        _instance = singletonObject.AddComponent<GameManager>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _instance;
            }
        }
    }

    //パラメーター
    [Tooltip("プレイヤーの最大体力")]
    [SerializeField] public int playerMaxHealth = 250;
    [Tooltip("エンドシーンの名前")]
    [SerializeField] private string EndGameSceneName = "EndScenes";
    [Tooltip("衝撃を与えた時のターゲットへのダメージ")]
    [SerializeField] private int impactDamage = 10;
    [Tooltip("振動の閾値")]
    [SerializeField] public float shakeThreshold = 1.5f;
    [Tooltip("衝撃の半径")]
    [SerializeField] public float impactRadius = 5.0f;
    [Tooltip("衝撃の強さ")]
    [SerializeField] public float impactForce = 100.0f;
    [Tooltip("回復するレベル")]
    [SerializeField] public int healingLevel = 5;

    //オーディオ関連
    [Tooltip("ゲーム中のBGM")]
    [SerializeField] private AudioClip backgroundMusic;
    [Tooltip("エンドシーンのBGM")]
    [SerializeField] private AudioClip endSceneMusic;
    [Tooltip("BGMのボリューム")]
    [SerializeField] private float BGMVolume;
    [Tooltip("ダメージを受けた時の音")]
    [SerializeField] private AudioClip damageSound;
    [Tooltip("レベルが上がった時の音")]
    [SerializeField] private AudioClip levelUpSound;
    [Tooltip("衝撃を与えた時の音")]
    [SerializeField] private AudioClip impactSound;

    //オーディオソース
    private AudioSource audioSource;

    //パブリック変数
    [NonSerialized] public float gunPower;
    [NonSerialized] public int currentPlayerHealth;
    [NonSerialized] public int defeatedEnemies = 0;
    [NonSerialized] public int defeatedNormal = 0;
    [NonSerialized] public int defeatedMissile = 0;
    [NonSerialized] public int defeatedHand = 0;

    //プライベート変数
    private bool _isNotShoot = false;
    private bool _isGrabbed = false;
    private bool noHealth = false;
    private bool isGameRunning = false;
    public float gameTime { get; private set; }

    //参照
    private SceneController SC;
    private TargetSpawn targetSpawn;
    private ARGunController arGunController;
    public ShootingUIcontroller UIcontroller;
    public GameObject canvasObject;

    //spawnCountとtestImpactの定義
    public int spawnCount = 0;
    public bool testImpact;

    //プロパティ
    public bool IsGrabbed
    {
        get => _isGrabbed;
        set => _isGrabbed = value;
    }

    public bool IsNotShoot
    {
        get => _isNotShoot;
        set => _isNotShoot = value;
    }

    //ゲームオブジェクトのAwakeイベント
    private void Awake()
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
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

    //OnDestroyイベント
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    //ゲーム開始時の初期化
    private void Start()
    {
        Initialize();
    }

    //毎フレーム更新
    private void Update()
    {
        if (testImpact)
        {
            ApplyImpact();
            ResetGrabState();
            testImpact = false;
        }

        if (IsGrabbed && DetectShake())
        {
            ApplyImpact();
            ResetGrabState();
        }

        if (isGameRunning)
        {
            gameTime += Time.deltaTime;
        }

        if (currentPlayerHealth <= 0 && !noHealth)
        {
            WhenNoHealth();
        }
    }

    //初期化関数
    public void Initialize()
    {
        Application.targetFrameRate = 60;//60FPSに設定

        SC = SceneController.Instance;
        SetupComponents();
        InitializeGameVariables();
        SetupAudioSource();
        SetupCanvas();
        ResetPlayerHealth();

        if (!UIcontroller.reticleUI.enabled)
        {
            UIcontroller.reticleUI.enabled = true;
        }

        if (backgroundMusic != null)
        {
            PlayBackgroundMusic(backgroundMusic, BGMVolume);
        }
    }

    //キャンバスのセットアップ
    private void SetupCanvas()
    {
        if (canvasObject == null)
        {
            canvasObject = GameObject.FindWithTag("Canvas");
            Debug.Log("canvasObjectは" + canvasObject);
        }
        canvasObject.SetActive(true);

        UIcontroller = canvasObject.GetComponent<ShootingUIcontroller>();
        if (UIcontroller == null)
        {
            Debug.LogError("ShootingUIcontrollerが見つかりません。");
        }
        else
        {
            UIcontroller.enabled = true;
        }

        ResetGrabState();

    }

    //各コンポーネントのセットアップ
    private void SetupComponents()
    {
        arGunController = GetComponent<ARGunController>();
        if (arGunController == null)
        {
            Debug.LogError("ARGunControllerが見つかりません。");
        }
        else
        {
            arGunController.enabled = true;
        }

        targetSpawn = GetComponent<TargetSpawn>();
        if (targetSpawn == null)
        {
            Debug.LogError("TargetSpawnが見つかりません。");
        }
        else
        {
            targetSpawn.enabled = true;
        }
    }

    //ゲーム変数の初期化
    private void InitializeGameVariables()
    {
        defeatedEnemies = 0;
        defeatedNormal = 0;
        defeatedMissile = 0;
        defeatedHand = 0;
        gameTime = 0f;
        isGameRunning = true;

        HandTargetMove.hasVibrated = false;
        HandTargetMove.isNear = false;
        spawnCount = 0;
    }

    //プレイヤーの体力をリセット
    private void ResetPlayerHealth()
    {
        currentPlayerHealth = playerMaxHealth;
        noHealth = false;
        NowPlayerHealth();
    }

    //オーディオソースのセットアップ
    private void SetupAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    //GameManagerを経由して経験値をARGunに渡す
    public void ViaGM(int ReceiveEXP)
    {
        arGunController.AddExperience(ReceiveEXP);
    }

    //プレイヤーのレベルアップ時の処理
    public void OnPlayerLevelUp(int currentLevel)
    {
        PlayPlayerSound(levelUpSound);
        if (currentLevel == healingLevel)
        {
            currentPlayerHealth += currentPlayerHealth;
            if (currentPlayerHealth >= playerMaxHealth)
            {
                currentPlayerHealth = playerMaxHealth;
            }
            UIcontroller.PlayerHPBar((float)currentPlayerHealth / playerMaxHealth);
        }
    }

    //プレイヤーの音を再生
    private void PlayPlayerSound(AudioClip audioClip)
    {
        if (audioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    //振動を検知
    private bool DetectShake()
    {
        Vector3 acceleration = Input.acceleration;
        return acceleration.sqrMagnitude >= (shakeThreshold * shakeThreshold);
    }

    //衝撃を適用
    private void ApplyImpact()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (collider.transform.position - transform.position).normalized;
                rb.AddForce(direction * impactForce, ForceMode.Impulse);
                TargetManager target = collider.GetComponent<TargetManager>();
                if (target != null)
                {
                    target.TakeDamage(impactDamage);
                    PlayPlayerSound(impactSound);
                }
            }
        }
        HandTargetMove.isNear = false;
        HandTargetMove.hasVibrated = false;
    }

    //掴み状態をリセット
    private void ResetGrabState()
    {

        if (IsGrabbed)
        {
            IsGrabbed = false;
        }
        if (!UIcontroller.reticleUI.enabled)
        {
            UIcontroller.reticleUI.enabled = true;
        }
        //VibrateScript.Instance.StopVibration();
    }

    //プレイヤーの体力を更新
    public void NowPlayerHealth()
    {
        UIcontroller.PlayerHPBar((float)currentPlayerHealth / playerMaxHealth);
        PlayPlayerSound(damageSound);
    }

    //プレイヤーの体力がなくなった時の処理
    private void WhenNoHealth()
    {
        noHealth = true;
        canvasObject?.SetActive(false);

        if (UIcontroller != null) UIcontroller.enabled = false;
        if (arGunController != null) arGunController.enabled = false;
        if (targetSpawn != null) targetSpawn.enabled = false;

        ResetGrabState();
        EndGame();
    }

    //ゲーム終了時の処理
    private void EndGame()
    {
        isGameRunning = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SC.ChangeScene(EndGameSceneName);
    }

    //BGMを再生
    private void PlayBackgroundMusic(AudioClip musicClip, float volume)
    {
        if (audioSource != null)
        {
            StopBackgroundMusic();
            audioSource.clip = musicClip;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    //BGMを停止
    public void StopBackgroundMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    //シーン読み込み時の処理
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == EndGameSceneName && endSceneMusic != null)
        {
            PlayBackgroundMusic(endSceneMusic, BGMVolume);
        }
    }
}