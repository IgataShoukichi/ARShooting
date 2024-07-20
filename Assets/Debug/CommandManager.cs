using UnityEngine;
using System.Collections.Generic;

public class CommandManager : MonoBehaviour
{
    [SerializeField] private string sceneControllerTag = "SceneController"; //SceneControllerのタグ
    private SceneController SC;
    [SerializeField] private GameObject startScenes;

    public string commandInput;//ユーザーが入力したコマンド文字列

    //コマンド名と対応するアクションを格納する辞書
    private Dictionary<string, System.Action<string[]>> commands;

    private string SceneName;

    void Start()
    {
        //コマンド辞書を初期化し、コマンド名と対応するメソッドを登録
        commands = new Dictionary<string, System.Action<string[]>>
        {
            { "start-s", ScenesJump_Start },
            { "game-s", ScenesJump_Game },
            { "end-s", ScenesJump_End },
        };

        // SceneControllerをタグから取得する
        GameObject sceneControllerObject = GameObject.FindWithTag(sceneControllerTag);
        if (sceneControllerObject != null)
        {
            SC = sceneControllerObject.GetComponent<SceneController>();
        }
        else
        {
            Debug.LogError("SceneController not found with tag: " + sceneControllerTag);
        }
    }

    //入力されたコマンドを実行するメソッド
    public void ExecuteCommand()
    {
        //コマンド入力が空白またはnullの場合は何もしない
        if (string.IsNullOrWhiteSpace(commandInput))
            return;

        //コマンド入力をスペースで分割し、コマンド名と引数を取得
        string[] parts = commandInput.Trim().ToLower().Split(' ');
        string command = parts[0];
        string[] args = parts.Length > 1 ? parts[1..] : new string[0];

        //コマンドが辞書に存在するか確認し、存在すれば対応するアクションを実行
        if (commands.ContainsKey(command))
        {
            commands[command].Invoke(args);
            Debug.Log(command + "実行");
        }
        else
        {
            //コマンドが不明な場合のメッセージを表示
            Debug.Log("Unknown command: " + command);
        }

        //コマンド入力フィールドをクリア
        commandInput = "";
    }

    //スタートシーンに移動するメソッド
    void ScenesJump_Start(string[] args)
    {
        Debug.Log("スタートシーンに移動します");
        SceneName = "StartScenes";//シーンを指定
        SC.ChangeScene(SceneName);//指定されたシーンをロード
    }
    //ゲームシーンに移動するメソッド
    void ScenesJump_Game(string[] args)
    {
        Debug.Log("ゲームシーンに移動します");
        SceneName = "ShootingGame";
    }
    //エンドシーンに移動するメソッド
    void ScenesJump_End(string[] args)
    {
        Debug.Log("エンドシーンに移動します");
        SceneName = "EndScenes";
        SC.ChangeScene(SceneName);
    }

}
