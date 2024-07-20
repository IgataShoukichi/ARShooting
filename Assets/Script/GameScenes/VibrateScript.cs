using UnityEngine;

public class VibrateScript
{
    private bool isVibrating = false; //バイブレーションの状態を追跡するフラグ
    private static VibrateScript instance; //VibrateScriptクラスのインスタンス
    private AndroidJavaObject vibrator; //Vibratorサービス

    //VibrateScriptクラスのインスタンスを取得するプロパティ
    public static VibrateScript Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new VibrateScript();
            }
            return instance;
        }
    }

    //Androidの要素を取得する関数
    private void elementsAcquisition()
    {
        // AndroidのUnityPlayerクラスを取得
        AndroidJavaObject unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        // 現在のアクティビティを取得
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        // Vibratorサービスを取得
        vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
    }

    //Androidのバイブレーションを実行するメソッド
    public void StartVibration()
    {
        //バイブレーションが停止しており、かつAndroidプラットフォームである場合に実行
        if (!isVibrating && Application.platform == RuntimePlatform.Android)
        {
            elementsAcquisition();
            //バイブレーションを開始
            vibrator.Call("vibrate", 1000);
            isVibrating = true; //バイブレーションが開始されたことを記録
        }
        else
        {
            Debug.Log("Androidではないプラットフォームです"); //Androidプラットフォームでない場合にログを出力
        }
    }

    //バイブレーションを停止するメソッド
    public void StopVibration()
    {
        //バイブレーションが実行中であり、かつAndroidプラットフォームである場合に実行
        if (isVibrating && Application.platform == RuntimePlatform.Android)
        {
            elementsAcquisition();
            //バイブレーションを停止
            vibrator.Call("cancel");
            isVibrating = false; //バイブレーションが停止されたことを記録
        }
        else
        {
            Debug.Log("Androidではないプラットフォームです"); //Androidプラットフォームでない場合にログを出力
        }
    }
}