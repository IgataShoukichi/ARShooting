using UnityEngine;
using UnityEngine.UI;

public class ShootingUIcontroller : MonoBehaviour
{
    //UIプレハブの参照
    [Tooltip("HPBarのプレハブ")]
    [SerializeField] private GameObject HPBarPrefab;//HPバーのプレハブ
    [Tooltip("ReticleUIのプレハブ")]
    [SerializeField] private GameObject reticleUIPrefab;//レティクルUIのプレハブ
    public Image HPBar;//実際のHPバーのImageコンポーネント
    public Image reticleUI;//実際のレティクルUIのImageコンポーネント

    //シングルトンインスタンスの保持
    private static ShootingUIcontroller instance;

    //シングルトンインスタンスへのアクセス用プロパティ
    public static ShootingUIcontroller Instance
    {
        get
        {
            //インスタンスが存在しない場合はエラーメッセージを出力
            if (instance == null)
            {
                Debug.LogError("ShootingUIcontroller インスタンスが存在しません");
            }
            return instance;
        }
    }

    void Awake()
    {
        //シングルトンパターンを使用して重複インスタンスを防ぐ
        if (instance == null)
        {
            instance = this;//インスタンスを設定
            DontDestroyOnLoad(this.gameObject);//シーンをまたいでもオブジェクトを破棄しない
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);//既存のインスタンスがある場合はこのオブジェクトを破棄
            return;
        }

        //UI要素を生成および検索するメソッドを呼び出し
        GenerateAndFindUIElements();
    }

    //UI要素を生成および検索するメソッド
    private void GenerateAndFindUIElements()
    {
        //Canvas オブジェクトをタグで検索
        GameObject canvasObject = GameObject.FindWithTag("Canvas");

        //Canvas オブジェクトが存在しない場合のエラーハンドリング
        if (canvasObject == null)
        {
            Debug.LogError("Canvas オブジェクトが見つかりません。Canvas に 'Canvas' タグが付いていることを確認してください。");
            return;
        }

        //HPBar が未設定の場合はプレハブから生成
        if (HPBar == null)
        {
            //HPBar プレハブを Canvas の子として生成
            GameObject hpBarObject = Instantiate(HPBarPrefab, canvasObject.transform);
            if (hpBarObject != null)
            {
                HPBar = hpBarObject.GetComponent<Image>();//HPBar の Image コンポーネントを取得
                if (HPBar == null)
                {
                    Debug.LogError("HPBar の Image コンポーネントが見つかりません");
                    return;
                }
            }
            else
            {
                Debug.LogError("HPBar プレハブが生成できません");
                return;
            }
        }

        //reticleUI が未設定の場合はプレハブから生成
        if (reticleUI == null)
        {
            //reticleUI プレハブを Canvas の子として生成
            GameObject reticleUIObject = Instantiate(reticleUIPrefab, canvasObject.transform);
            if (reticleUIObject != null)
            {
                reticleUI = reticleUIObject.GetComponent<Image>();//reticleUI の Image コンポーネントを取得
                if (reticleUI == null)
                {
                    Debug.LogError("ReticleUI の Image コンポーネントが見つかりません");
                    return;
                }
            }
            else
            {
                Debug.LogError("ReticleUI プレハブが生成できません");
                return;
            }
        }
    }

    //プレイヤーのHPバーを更新するメソッド
    public void PlayerHPBar(float HPBarNum)
    {
        //HPBar が null でないことを確認してから fillAmount を設定
        if (HPBar != null)
        {
            HPBar.fillAmount = HPBarNum;
        }
        else
        {
            Debug.LogError("HPBar が設定されていません");
        }
    }
}
