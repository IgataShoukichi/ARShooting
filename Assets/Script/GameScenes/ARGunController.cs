using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class ARGunController : MonoBehaviour
{
    private GameManager GM;//GameManager
    [SerializeField] private ARCameraManager arCameraManager;//AR�J�����}�l�[�W���[

    //�p�����[�^�[
    [Tooltip("�e�e�̃v���n�u")]
    [SerializeField] private GameObject bulletPrefab;
    [Tooltip("�e�e�̑��x")]
    [SerializeField] private float bulletSpeed = 5f;
    [Tooltip("�p���[�̍ő�l")]
    [SerializeField] private float maxPower = 30f;
    [Tooltip("�p���[�����̊Ԋu�i�b�j")]
    [SerializeField] private float incrementInterval = 0.5f;
    [Tooltip("�e�e�̎����i�b�j")]
    [SerializeField] private float bulletLifetime = 5f;
    [Tooltip("�p���[UI�̔z��")]
    [SerializeField] private GameObject[] powerUI;
    [Tooltip("UI�̓_�ł̊Ԋu�i�b�j")]
    [SerializeField] private float blinkInterval = 0.5f;
    [Tooltip("���x���̏��")]
    [SerializeField] int levelCap = 10;
    [Tooltip("�ŏ��̃��x���A�b�v�ɕK�v�Ȍo���l")]
    [SerializeField] int experienceToLevelUp = 100;
    [Tooltip("���x���A�b�v���ƂɕK�v�Ȍo���l�𑝉������銄��")]
    [SerializeField] float levelUpMultiplier = 1.5f;
    [Tooltip("���x���A�b�v���̒e�ۑ��x�̑����� (%)")]
    [SerializeField] private float speedBulletIncreaseRate = 10f;
    [Tooltip("���x���A�b�v���̒e�ۈЗ͂̑����� (%)")]
    [SerializeField] private float powerBulletIncreaseRate = 10f;

    //�T�E���h
    [Tooltip("�͂𗭂߂Ă���Ƃ��̉��̔z��")]
    [SerializeField] private AudioClip[] incrementPowerSounds;
    private int currentPowerStage = -1;
    [Tooltip("���C��")]
    [SerializeField] private AudioClip shootSound;
    private AudioSource audioSource;

    private float lastIncrementTime;//�O��p���[�𑝉�����������
    private float power = 0f;//���݂̃p���[

    private float lastBlinkTime;//�O��UI�̃A�N�e�B�u��Ԃ�؂�ւ�������
    private bool lastPowerUIState = false;//�Ō��UI�̃A�N�e�B�u���
    public int currentExperience { get; private set; }//���݂̌o���l
    private int currentLevel = 1;//���݂̃��x��

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
            //�ˌ�������
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
        if (scene.name == "ShootingGame") // �Q�[���V�[���̖��O���w��
        {
            InitializeARGun();
        }
    }
    public void InitializeARGun()
    {
        currentExperience = 0;
        currentLevel = 1;

        //ARCameraManager�̎Q�Ƃ��擾
        arCameraManager = FindObjectOfType<ARCameraManager>();
        if (arCameraManager == null)
        {
            Debug.LogError("ARCameraManager���V�[���ɑ��݂��Ă��܂���-ARGunController");
            return;
        }

        //GameManager�̃C���X�^���X���擾
        GM = GameManager.Instance;

        //�p���[UI���B��
        HidePowerUI();

        //AudioSource�̏�����
        audioSource = gameObject.AddComponent<AudioSource>();

        Debug.Log("ARGun�̏���������");
    }

    //�^�b�`���͂̏������s���B
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                //�^�b�`���n�܂������A�p���[�����Z�b�g
                ResetPower();
            }
            else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                //���������Ƀp���[�𑝉������AUI���X�V
                IncrementPower();
                UpdatePowerUI();
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                //�^�b�`���I���������A���݂̃p���[�Œe�𔭎�
                Shoot(power);
                HidePowerUI();
            }
        }
    }

    //�p���[�����Z�b�g���A�J�n�������L�^����B
    private void ResetPower()
    {
        power = 0f;
        lastIncrementTime = Time.time;
        currentPowerStage = -1;
    }

    //�p���[�����Ԋu�ő���������B
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

                //�͂𗭂߂Ă��鉹���Đ�
                if (incrementPowerSounds != null && audioSource != null && currentPowerStage < incrementPowerSounds.Length)
                {
                    audioSource.PlayOneShot(incrementPowerSounds[currentPowerStage]);
                }
            }
        }
    }

    //���݂̃p���[���g�p���ďe�e�𔭎˂���B
    private void Shoot(float power)
    {
        GM.gunPower = power;

        if (arCameraManager == null || !arCameraManager.TryGetIntrinsics(out _))
        {
            Debug.LogWarning("AR Camera�����p�ł��܂���-ARGunController");
            return;
        }

        //�J�����̌��݂̈ʒu�ƕ������擾
        Transform cameraTransform = arCameraManager.transform;
        Vector3 bulletSpawnPosition = cameraTransform.position + cameraTransform.forward;

        //�e�e�̐���
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);

        //�e�e�̑��x��ݒ�
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        if (bulletRigidbody != null)
        {
            bulletRigidbody.velocity = cameraTransform.forward * (bulletSpeed + power);
        }

        //�e�e����莞�Ԍ�ɍ폜
        Destroy(bullet, bulletLifetime);

        //���C�����Đ�
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    //�p���[UI���X�V����B
    private void UpdatePowerUI()
    {
        int powerStage = Mathf.FloorToInt(power / (maxPower / powerUI.Length));

        if (powerStage >= 0 && powerStage < powerUI.Length)
        {
            //�S�Ă�UI���\����
            HidePowerUI();
            //���݂̃p���[�X�e�[�W�ɉ�����UI��\��
            powerUI[powerStage].SetActive(true);
        }
        else if (power >= maxPower)
        {
            //maxPower�ɒB�����ꍇ�A�Ō��UI��_�ł�����
            float currentTime = Time.time;

            if (currentTime - lastBlinkTime >= blinkInterval)
            {
                lastPowerUIState = !lastPowerUIState;
                powerUI[powerUI.Length - 1].SetActive(lastPowerUIState);
                lastBlinkTime = currentTime;
            }
        }
    }

    //�p���[UI���\���ɂ���B
    private void HidePowerUI()
    {
        foreach (var uiElement in powerUI)
        {
            uiElement.SetActive(false);
        }
    }

    //�w�肳�ꂽ�o���l��ǉ�����B
    public void AddExperience(int AddEXP)
    {
        currentExperience += AddEXP;
        CheckLevelUp();
    }

    //���x���A�b�v�̃`�F�b�N���s��
    private void CheckLevelUp()
    {
        if (currentLevel >= levelCap)
        {
            return; //���݂̃��x��������ɒB���Ă���ꍇ�A���������ɏI��
        }

        while (currentExperience >= experienceToLevelUp)
        {
            currentExperience -= experienceToLevelUp;
            currentLevel++;
            OnLevelUp();

            //����ɒB�����烋�[�v���I��
            if (currentLevel >= levelCap)
            {
                break;
            }

            experienceToLevelUp = Mathf.CeilToInt(experienceToLevelUp * levelUpMultiplier);
        }
    }

    //���x���A�b�v���̏���
    private void OnLevelUp()
    {
        Debug.Log($"Level Up! New Level: {currentLevel}");

        //�S�������瑝���{�����v�Z
        float speedBulletMultiplier = 1 + (speedBulletIncreaseRate / 100f);
        float powerBulletMultiplier = 1 + (powerBulletIncreaseRate / 100f);

        //�e�ۂ̑��x�ƈЗ͂𑝉�������
        bulletSpeed *= speedBulletMultiplier;
        maxPower *= powerBulletMultiplier;

        //GameManager�ɒʒm
        GM.OnPlayerLevelUp(currentLevel);
    }
}
