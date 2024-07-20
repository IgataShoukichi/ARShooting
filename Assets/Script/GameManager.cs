using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //�V���O���g���C���X�^���X��ێ�����ÓI�ϐ�
    private static GameManager _instance;
    private static readonly object _lock = new object();

    //�V���O���g���C���X�^���X�̃v���p�e�B
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
                        //�Q�[���I�u�W�F�N�g��V�K�쐬���AGameManager���A�^�b�`
                        GameObject singletonObject = new GameObject("GameManager");
                        _instance = singletonObject.AddComponent<GameManager>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _instance;
            }
        }
    }

    //�p�����[�^�[
    [Tooltip("�v���C���[�̍ő�̗�")]
    [SerializeField] public int playerMaxHealth = 250;
    [Tooltip("�G���h�V�[���̖��O")]
    [SerializeField] private string EndGameSceneName = "EndScenes";
    [Tooltip("�Ռ���^�������̃^�[�Q�b�g�ւ̃_���[�W")]
    [SerializeField] private int impactDamage = 10;
    [Tooltip("�U����臒l")]
    [SerializeField] public float shakeThreshold = 1.5f;
    [Tooltip("�Ռ��̔��a")]
    [SerializeField] public float impactRadius = 5.0f;
    [Tooltip("�Ռ��̋���")]
    [SerializeField] public float impactForce = 100.0f;
    [Tooltip("�񕜂��郌�x��")]
    [SerializeField] public int healingLevel = 5;

    //�I�[�f�B�I�֘A
    [Tooltip("�Q�[������BGM")]
    [SerializeField] private AudioClip backgroundMusic;
    [Tooltip("�G���h�V�[����BGM")]
    [SerializeField] private AudioClip endSceneMusic;
    [Tooltip("BGM�̃{�����[��")]
    [SerializeField] private float BGMVolume;
    [Tooltip("�_���[�W���󂯂����̉�")]
    [SerializeField] private AudioClip damageSound;
    [Tooltip("���x�����オ�������̉�")]
    [SerializeField] private AudioClip levelUpSound;
    [Tooltip("�Ռ���^�������̉�")]
    [SerializeField] private AudioClip impactSound;

    //�I�[�f�B�I�\�[�X
    private AudioSource audioSource;

    //�p�u���b�N�ϐ�
    [NonSerialized] public float gunPower;
    [NonSerialized] public int currentPlayerHealth;
    [NonSerialized] public int defeatedEnemies = 0;
    [NonSerialized] public int defeatedNormal = 0;
    [NonSerialized] public int defeatedMissile = 0;
    [NonSerialized] public int defeatedHand = 0;

    //�v���C�x�[�g�ϐ�
    private bool _isNotShoot = false;
    private bool _isGrabbed = false;
    private bool noHealth = false;
    private bool isGameRunning = false;
    public float gameTime { get; private set; }

    //�Q��
    private SceneController SC;
    private TargetSpawn targetSpawn;
    private ARGunController arGunController;
    public ShootingUIcontroller UIcontroller;
    public GameObject canvasObject;

    //spawnCount��testImpact�̒�`
    public int spawnCount = 0;
    public bool testImpact;

    //�v���p�e�B
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

    //�Q�[���I�u�W�F�N�g��Awake�C�x���g
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

    //OnDestroy�C�x���g
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    //�Q�[���J�n���̏�����
    private void Start()
    {
        Initialize();
    }

    //���t���[���X�V
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

    //�������֐�
    public void Initialize()
    {
        Application.targetFrameRate = 60;//60FPS�ɐݒ�

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

    //�L�����o�X�̃Z�b�g�A�b�v
    private void SetupCanvas()
    {
        if (canvasObject == null)
        {
            canvasObject = GameObject.FindWithTag("Canvas");
            Debug.Log("canvasObject��" + canvasObject);
        }
        canvasObject.SetActive(true);

        UIcontroller = canvasObject.GetComponent<ShootingUIcontroller>();
        if (UIcontroller == null)
        {
            Debug.LogError("ShootingUIcontroller��������܂���B");
        }
        else
        {
            UIcontroller.enabled = true;
        }

        ResetGrabState();

    }

    //�e�R���|�[�l���g�̃Z�b�g�A�b�v
    private void SetupComponents()
    {
        arGunController = GetComponent<ARGunController>();
        if (arGunController == null)
        {
            Debug.LogError("ARGunController��������܂���B");
        }
        else
        {
            arGunController.enabled = true;
        }

        targetSpawn = GetComponent<TargetSpawn>();
        if (targetSpawn == null)
        {
            Debug.LogError("TargetSpawn��������܂���B");
        }
        else
        {
            targetSpawn.enabled = true;
        }
    }

    //�Q�[���ϐ��̏�����
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

    //�v���C���[�̗̑͂����Z�b�g
    private void ResetPlayerHealth()
    {
        currentPlayerHealth = playerMaxHealth;
        noHealth = false;
        NowPlayerHealth();
    }

    //�I�[�f�B�I�\�[�X�̃Z�b�g�A�b�v
    private void SetupAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    //GameManager���o�R���Čo���l��ARGun�ɓn��
    public void ViaGM(int ReceiveEXP)
    {
        arGunController.AddExperience(ReceiveEXP);
    }

    //�v���C���[�̃��x���A�b�v���̏���
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

    //�v���C���[�̉����Đ�
    private void PlayPlayerSound(AudioClip audioClip)
    {
        if (audioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    //�U�������m
    private bool DetectShake()
    {
        Vector3 acceleration = Input.acceleration;
        return acceleration.sqrMagnitude >= (shakeThreshold * shakeThreshold);
    }

    //�Ռ���K�p
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

    //�͂ݏ�Ԃ����Z�b�g
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

    //�v���C���[�̗̑͂��X�V
    public void NowPlayerHealth()
    {
        UIcontroller.PlayerHPBar((float)currentPlayerHealth / playerMaxHealth);
        PlayPlayerSound(damageSound);
    }

    //�v���C���[�̗̑͂��Ȃ��Ȃ������̏���
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

    //�Q�[���I�����̏���
    private void EndGame()
    {
        isGameRunning = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SC.ChangeScene(EndGameSceneName);
    }

    //BGM���Đ�
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

    //BGM���~
    public void StopBackgroundMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    //�V�[���ǂݍ��ݎ��̏���
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == EndGameSceneName && endSceneMusic != null)
        {
            PlayBackgroundMusic(endSceneMusic, BGMVolume);
        }
    }
}