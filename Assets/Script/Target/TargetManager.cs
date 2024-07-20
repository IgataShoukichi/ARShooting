using UnityEngine;
using System.Collections;

public class TargetManager : MonoBehaviour
{
    private GameManager GM;//GameManager
    public Transform playerTransform;//プレイヤーのTransform 

    // 敵の種類を定義する列挙型
    public enum EnemyType { Default, Normal, Missile, Hand }

    // パラメーター
    [Tooltip("自身の種類")]
    [SerializeField] public EnemyType myType = EnemyType.Default;
    [Tooltip("最大体力")]
    [SerializeField] private int maxHealth = 100;
    [Tooltip("弾から受けるダメージ")]
    [SerializeField] private int bulletDamage = 50;
    [Tooltip("ダメージを与えるタグ")]
    [SerializeField] private string damageTag = "Bullet";
    [Tooltip("プレイヤーが獲得する経験値")]
    [SerializeField] private int GrantEXP = 10;
    [Tooltip("プレイヤーへのダメージ")]
    [SerializeField] private int damageForPlayer = 25;

    // サウンド
    [Tooltip("ダメージを受けた時の音")]
    [SerializeField] private AudioClip damageSound;
    [Tooltip("消滅するときの音")]
    [SerializeField] private AudioClip deathSound;
    [Tooltip("常時鳴らしている音")]
    [SerializeField] private AudioClip alwaysSound;
    [Tooltip("常時音再生の間隔")]
    [SerializeField] private float alwaysSoundInterval = 3f;
    private float nextAlwaysSoundTime;

    // エフェクト
    [Tooltip("ダメージ時のエフェクト")]
    [SerializeField] private ParticleSystem damageEffect;
    [Tooltip("死亡時のエフェクト")]
    [SerializeField] private ParticleSystem deathEffect;

    private bool self = false;//自身によるダメージかどうか
    private int _currentHealth;//現在の体力

    //プロパティ
    public int MaxHealth => maxHealth;
    public int CurrentHealth => _currentHealth;

    private void Start()
    {
        GM = GameManager.Instance;
        nextAlwaysSoundTime = Time.time + alwaysSoundInterval;
        ResetTarget();
    }

    private void Update()
    {
        //一定間隔で音を再生
        if (Time.time >= nextAlwaysSoundTime)
        {
            PlaySound(alwaysSound);
            nextAlwaysSoundTime = Time.time + alwaysSoundInterval + Random.value;
        }
    }

    //ダメージを受けるメソッド
    public void TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount;//ダメージを体力から減算
        PlaySound(damageSound);
        PlayEffect(damageEffect);

        if (_currentHealth <= 0)
        {
            StartCoroutine(DieCoroutine(myType, self));//体力が0以下になったら死亡処理を実行
        }
    }

    //ターゲットのリセット
    private void ResetTarget()
    {
        _currentHealth = maxHealth;//最初は最大体力で開始
        playerTransform = GameObject.FindWithTag("Player").transform;//プレイヤーのTransformを取得
        self = false;
    }

    // 死亡処理
    public IEnumerator DieCoroutine(EnemyType who, bool self)
    {
        PlaySound(deathSound);

        GM.spawnCount--;
        if (!self)
        {
            GM.ViaGM(GrantEXP);
            PlayEffect(deathEffect);
            GM.defeatedEnemies++;
            switch (who)
            {
                case EnemyType.Normal:
                    GM.defeatedNormal++;
                    break;
                case EnemyType.Missile:
                    GM.defeatedMissile++;
                    break;
                case EnemyType.Hand:
                    GM.defeatedHand++;
                    break;
                case EnemyType.Default:
                default:
                    Debug.LogWarning("EnemyTypeが設定されていません");
                    break;
            }
        }

        //オブジェクトを非アクティブにする
        gameObject.SetActive(false);

        //サウンドの再生が終わるのを待つ
        yield return new WaitForSeconds(deathSound.length);

        Destroy(gameObject);
    }

    //プレイヤーにダメージを与えるメソッド
    public void DealDamage()
    {
        GM.currentPlayerHealth -= damageForPlayer;
        GM.NowPlayerHealth();
    }

    //サウンドを再生するメソッド
    private void PlaySound(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
        }
    }

    //エフェクトを再生するメソッド
    public void PlayEffect(ParticleSystem particleSystem)
    {
        if (particleSystem != null)
        {
            ParticleSystem effect = Instantiate(particleSystem, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }
    }

    //衝突判定
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(damageTag))
        {
            TakeDamage(bulletDamage + (int)GM.gunPower);
            Destroy(col.gameObject);
        }
    }
}
