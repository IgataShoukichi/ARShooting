using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScenesManager : MonoBehaviour
{
    private GameManager GM;
    private SceneController SC;

    [Tooltip("�ʏ�^�[�Q�b�g�̃J�E���g��\������Q�[���I�u�W�F�N�g")]
    [SerializeField] private GameObject normalTargetCount;
    [Tooltip("�~�T�C���^�[�Q�b�g�̃J�E���g��\������Q�[���I�u�W�F�N�g")]
    [SerializeField] private GameObject missileTargetCount;
    [Tooltip("��̃^�[�Q�b�g�̃J�E���g��\������Q�[���I�u�W�F�N�g")]
    [SerializeField] private GameObject handTargetCount;
    [Tooltip("�Q�[�����Ԃ�\������Q�[���I�u�W�F�N�g")]
    [SerializeField] private GameObject gameTime;

    [Tooltip("�z�[���V�[���ɖ߂�{�^��")]
    [SerializeField] private Button homeButton;
    [Tooltip("������x�Q�[����V�ԃ{�^��")]
    [SerializeField] private Button replayButton;

    [Tooltip("�Q�[���N�����̃V�[����")]
    [SerializeField] private string StartSceneName = "StartScenes";
    [Tooltip("�Q�[���V�[����")]
    [SerializeField] private string GameSceneName = "ShootingGame";

    void Start()
    {
        InitializeReferences();
        InitializeUI();
        InitializeButtons();
    }

    //�V���O���g���C���X�^���X�̎擾
    void InitializeReferences()
    {
        SC = SceneController.Instance;
        if (SC == null)
        {
            Debug.LogError("SceneController �̃C���X�^���X��������܂���B");
            return;
        }

        GM = GameManager.Instance;
        if (GM == null)
        {
            Debug.LogError("GameManager �̃C���X�^���X��������܂���B");
            return;
        }
    }

    //UI�̍X�V
    void InitializeUI()
    {
        UpdateTargetCountText(normalTargetCount, GM.defeatedNormal);
        UpdateTargetCountText(missileTargetCount, GM.defeatedMissile);
        UpdateTargetCountText(handTargetCount, GM.defeatedHand);

        UpdateGameTimeText(gameTime, GM.gameTime);
    }

    //�{�^���̃N���b�N�C�x���g��ݒ�
    void InitializeButtons()
    {
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(OnHomeButtonClick);
        }
        else
        {
            Debug.LogError("�z�[���{�^�������蓖�Ă��Ă��܂���B");
        }

        if (replayButton != null)
        {
            replayButton.onClick.AddListener(OnReplayButtonClick);
        }
        else
        {
            Debug.LogError("���v���C�{�^�������蓖�Ă��Ă��܂���B");
        }
    }

    //�^�[�Q�b�g�̃J�E���g���X�V���ĕ\��
    void UpdateTargetCountText(GameObject targetCountObject, int targetCount)
    {
        if (targetCountObject == null)
        {
            Debug.LogError("�^�[�Q�b�g�J�E���g�I�u�W�F�N�g�����蓖�Ă��Ă��܂���B");
            return;
        }

        Text targetCountText = targetCountObject.GetComponent<Text>();
        if (targetCountText == null)
        {
            Debug.LogError($"{targetCountObject.name} �� Text �R���|�[�l���g��������܂���B");
            return;
        }

        targetCountText.text = $"{targetCount}";
    }

    //�o�ߎ��Ԃ��X�V���ĕ\��
    void UpdateGameTimeText(GameObject gameTimeObject, float time)
    {
        if (gameTimeObject == null)
        {
            Debug.LogError("�Q�[�����ԃI�u�W�F�N�g�����蓖�Ă��Ă��܂���B");
            return;
        }

        Text gameTimeText = gameTimeObject.GetComponent<Text>();
        if (gameTimeText == null)
        {
            Debug.LogError($"{gameTimeObject.name} �� Text �R���|�[�l���g��������܂���B");
            return;
        }

        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 1000F) % 1000F);
        gameTimeText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }

    //�z�[���{�^�����N���b�N���ꂽ�Ƃ��̏���
    void OnHomeButtonClick()
    {
        SC.ChangeScene(StartSceneName);
        GM.StopBackgroundMusic();
    }

    //���v���C�{�^�����N���b�N���ꂽ�Ƃ��̏���
    void OnReplayButtonClick()
    {
        SC.ChangeScene(GameSceneName);
        GM.Initialize();
        GM.StopBackgroundMusic();
    }
}
