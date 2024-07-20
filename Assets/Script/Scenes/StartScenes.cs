using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScenes : MonoBehaviour
{
    private SceneController SC;
    private GameManager GM;

    //パラメーター
    [Tooltip("ゲームシーンの名前")]
    [SerializeField] private string GameSceneName = "ShootingGame";
    [Tooltip("長押しとして認識する時間の閾値")]
    [SerializeField] private float holdThreshold = 1.0f;
    [Tooltip("長押しの時に現れるゲームの概要")]
    [SerializeField] private GameObject SummaryColumn;

    //サウンド
    [Tooltip("スタート画面のBGM")]
    [SerializeField] private AudioClip startMusic;
    [Tooltip("スタート画面のBGMの音量")]
    [SerializeField] private float startBGMVolume;
    private AudioSource audioSource;//音声再生用のAudioSource

    private bool isDisplay = false;//ゲームの概要が表示されているかどうか
    private bool touchStarted = false;//タッチが開始されたかどうかのフラグ
    private float touchStartTime;//タッチが開始された時間

    private float touchDuration = 0;//タッチの経過時間

    private void Start()
    {
        SC = SceneController.Instance;
        if (SC == null)
        {
            Debug.Log("SceneControllerを取得できませんでした。");
        }

        if (SceneController.PreviousSceneName != null)
        {
            GM = GameManager.Instance;
            if (GM == null)
            {
                Debug.Log("GameManagerを取得できませんでした。");
            }
        }

        SummaryColumn.SetActive(false);
        touchDuration = 0f;
        audioSource = gameObject.AddComponent<AudioSource>();
        //BGMをループ再生
        if (startMusic != null)
        {
            PlayBackgroundMusic(startMusic, startBGMVolume);
        }
    }

    void Update()
    {
        // タッチ入力の処理
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

        // マウス入力の処理 (Androidでも動作するように)
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
                // 概要が表示されている場合は非表示にしてリセット
                SpecialAction(isDisplay);
            }
            else
            {
                // 長押しとして認識する時間以上押し続けられた場合
                if (touchDur >= holdThreshold)
                {
                    SpecialAction(isDisplay);
                }
                else // 長押しでない場合
                {
                    StartGame(); // ゲームを開始
                }
            }

            touchStarted = false; // タッチ開始フラグをリセット
        }
    }

    // 概要の表示/非表示を実行する
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

    // ゲームを開始する
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
