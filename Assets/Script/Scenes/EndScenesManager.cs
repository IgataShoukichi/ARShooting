using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScenesManager : MonoBehaviour
{
    private GameManager GM;
    private SceneController SC;

    [Tooltip("通常ターゲットのカウントを表示するゲームオブジェクト")]
    [SerializeField] private GameObject normalTargetCount;
    [Tooltip("ミサイルターゲットのカウントを表示するゲームオブジェクト")]
    [SerializeField] private GameObject missileTargetCount;
    [Tooltip("手のターゲットのカウントを表示するゲームオブジェクト")]
    [SerializeField] private GameObject handTargetCount;
    [Tooltip("ゲーム時間を表示するゲームオブジェクト")]
    [SerializeField] private GameObject gameTime;

    [Tooltip("ホームシーンに戻るボタン")]
    [SerializeField] private Button homeButton;
    [Tooltip("もう一度ゲームを遊ぶボタン")]
    [SerializeField] private Button replayButton;

    [Tooltip("ゲーム起動時のシーン名")]
    [SerializeField] private string StartSceneName = "StartScenes";
    [Tooltip("ゲームシーン名")]
    [SerializeField] private string GameSceneName = "ShootingGame";

    void Start()
    {
        InitializeReferences();
        InitializeUI();
        InitializeButtons();
    }

    //シングルトンインスタンスの取得
    void InitializeReferences()
    {
        SC = SceneController.Instance;
        if (SC == null)
        {
            Debug.LogError("SceneController のインスタンスが見つかりません。");
            return;
        }

        GM = GameManager.Instance;
        if (GM == null)
        {
            Debug.LogError("GameManager のインスタンスが見つかりません。");
            return;
        }
    }

    //UIの更新
    void InitializeUI()
    {
        UpdateTargetCountText(normalTargetCount, GM.defeatedNormal);
        UpdateTargetCountText(missileTargetCount, GM.defeatedMissile);
        UpdateTargetCountText(handTargetCount, GM.defeatedHand);

        UpdateGameTimeText(gameTime, GM.gameTime);
    }

    //ボタンのクリックイベントを設定
    void InitializeButtons()
    {
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(OnHomeButtonClick);
        }
        else
        {
            Debug.LogError("ホームボタンが割り当てられていません。");
        }

        if (replayButton != null)
        {
            replayButton.onClick.AddListener(OnReplayButtonClick);
        }
        else
        {
            Debug.LogError("リプレイボタンが割り当てられていません。");
        }
    }

    //ターゲットのカウントを更新して表示
    void UpdateTargetCountText(GameObject targetCountObject, int targetCount)
    {
        if (targetCountObject == null)
        {
            Debug.LogError("ターゲットカウントオブジェクトが割り当てられていません。");
            return;
        }

        Text targetCountText = targetCountObject.GetComponent<Text>();
        if (targetCountText == null)
        {
            Debug.LogError($"{targetCountObject.name} に Text コンポーネントが見つかりません。");
            return;
        }

        targetCountText.text = $"{targetCount}";
    }

    //経過時間を更新して表示
    void UpdateGameTimeText(GameObject gameTimeObject, float time)
    {
        if (gameTimeObject == null)
        {
            Debug.LogError("ゲーム時間オブジェクトが割り当てられていません。");
            return;
        }

        Text gameTimeText = gameTimeObject.GetComponent<Text>();
        if (gameTimeText == null)
        {
            Debug.LogError($"{gameTimeObject.name} に Text コンポーネントが見つかりません。");
            return;
        }

        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 1000F) % 1000F);
        gameTimeText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }

    //ホームボタンがクリックされたときの処理
    void OnHomeButtonClick()
    {
        SC.ChangeScene(StartSceneName);
        GM.StopBackgroundMusic();
    }

    //リプレイボタンがクリックされたときの処理
    void OnReplayButtonClick()
    {
        SC.ChangeScene(GameSceneName);
        GM.Initialize();
        GM.StopBackgroundMusic();
    }
}
