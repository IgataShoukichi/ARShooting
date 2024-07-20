using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static string previousSceneName; //前のシーン名を保持する変数
    private static SceneController instance; //シングルトンインスタンスを保持する変数

    //シングルトンインスタンスへのアクセスプロパティ
    public static SceneController Instance
    {
        get { return instance; }
    }

    //前のシーン名へのアクセスプロパティ
    public static string PreviousSceneName
    {
        get { return previousSceneName; }
    }

    private void Awake()
    {
        //シングルトンインスタンスの設定
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);//シーンをまたいでオブジェクトを保持する
        }
        else
        {
            Destroy(gameObject);//既にインスタンスが存在する場合、重複を防ぐために破棄する
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;//シーンがロードされた時のイベントにOnSceneLoadedメソッドを登録
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;//オブジェクトが破棄される時に登録したイベントを解除
    }

    public void ChangeScene(string newSceneName)
    {
        //現在のシーン名を保存
        previousSceneName = SceneManager.GetActiveScene().name;

        //新しいシーンに移動
        SceneManager.LoadScene(newSceneName);
    }

    //シーンがロードされた後の処理
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //ここに必要に応じた処理を追加する
    }
}
