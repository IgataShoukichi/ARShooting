using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScenes : MonoBehaviour
{
    private SceneController SC;
    private GameManager GM;

    //�p�����[�^�[
    [Tooltip("�Q�[���V�[���̖��O")]
    [SerializeField] private string GameSceneName = "ShootingGame";
    [Tooltip("�������Ƃ��ĔF�����鎞�Ԃ�臒l")]
    [SerializeField] private float holdThreshold = 1.0f;
    [Tooltip("�������̎��Ɍ����Q�[���̊T�v")]
    [SerializeField] private GameObject SummaryColumn;

    //�T�E���h
    [Tooltip("�X�^�[�g��ʂ�BGM")]
    [SerializeField] private AudioClip startMusic;
    [Tooltip("�X�^�[�g��ʂ�BGM�̉���")]
    [SerializeField] private float startBGMVolume;
    private AudioSource audioSource;//�����Đ��p��AudioSource

    private bool isDisplay = false;//�Q�[���̊T�v���\������Ă��邩�ǂ���
    private bool touchStarted = false;//�^�b�`���J�n���ꂽ���ǂ����̃t���O
    private float touchStartTime;//�^�b�`���J�n���ꂽ����

    private float touchDuration = 0;//�^�b�`�̌o�ߎ���

    private void Start()
    {
        SC = SceneController.Instance;
        if (SC == null)
        {
            Debug.Log("SceneController���擾�ł��܂���ł����B");
        }

        if (SceneController.PreviousSceneName != null)
        {
            GM = GameManager.Instance;
            if (GM == null)
            {
                Debug.Log("GameManager���擾�ł��܂���ł����B");
            }
        }

        SummaryColumn.SetActive(false);
        touchDuration = 0f;
        audioSource = gameObject.AddComponent<AudioSource>();
        //BGM�����[�v�Đ�
        if (startMusic != null)
        {
            PlayBackgroundMusic(startMusic, startBGMVolume);
        }
    }

    void Update()
    {
        // �^�b�`���͂̏���
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStarted = true;
                touchStartTime = Time.time;
                touchDuration = 0f;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchDuration = Time.time - touchStartTime;
                HandleTouchEnd(touchDuration);
            }
        }

        // �}�E�X���͂̏��� (Android�ł����삷��悤��)
        if (Input.GetMouseButtonDown(0))
        {
            touchStarted = true;
            touchStartTime = Time.time;
            touchDuration = 0f;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchDuration = Time.time - touchStartTime;
            HandleTouchEnd(touchDuration);
        }
    }

    private void HandleTouchEnd(float touchDur)
    {
        if (touchStarted)
        {
            if (isDisplay)
            {
                // �T�v���\������Ă���ꍇ�͔�\���ɂ��ă��Z�b�g
                SpecialAction(isDisplay);
            }
            else
            {
                // �������Ƃ��ĔF�����鎞�Ԉȏ㉟��������ꂽ�ꍇ
                if (touchDur >= holdThreshold)
                {
                    SpecialAction(isDisplay);
                }
                else // �������łȂ��ꍇ
                {
                    StartGame(); // �Q�[�����J�n
                }
            }

            touchStarted = false; // �^�b�`�J�n�t���O�����Z�b�g
        }
    }

    // �T�v�̕\��/��\�������s����
    private void SpecialAction(bool display)
    {
        if (!display)
        {
            SummaryColumn.SetActive(true);
            isDisplay = true;
        }
        else
        {
            SummaryColumn.SetActive(false);
            isDisplay = false;
        }
    }

    // �Q�[�����J�n����
    public void StartGame()
    {
        SC.ChangeScene(GameSceneName);
        if (GM != null)
        {
            GM.Initialize();
        }
    }

    private void PlayBackgroundMusic(AudioClip musicClip, float volume)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = musicClip;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }
}
