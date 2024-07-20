using UnityEngine;

public class TargetSpawn : MonoBehaviour
{
    private GameManager GM;

    //パラメーター
    [Tooltip("生成するオブジェクトのプレハブ")]
    [SerializeField] private GameObject[] targetObjectPrefabs;
    [Tooltip("オブジェクトを生成する間隔（秒)")]
    [SerializeField] private float spawnInterval = 5f;
    [Tooltip("オブジェクトを生成する範囲のサイズ")]
    [SerializeField] private float spawnAreaSize = 20f;
    [Tooltip("プレイヤーとの最小距離")]
    [SerializeField] private float spawnMinDistance = 10f;
    [Tooltip("オブジェクトの上限")]
    [SerializeField] private int maxObject = 10;
    [Tooltip("基本の上位種の出現率")]
    [SerializeField] private float baseSpawnRate = 0.02f;
    [Tooltip("倒された敵ごとの上位種の出現率増加量")]
    [SerializeField] private float spawnRateIncreasePerDefeatedEnemy = 0.05f;

    //サウンド
    [Tooltip("生成時の効果音")]
    [SerializeField] private AudioClip[] spawnSounds;
    [Tooltip("代理の音")]
    [SerializeField] private AudioClip defaultSpawnSound;

    //エフェクト
    [Tooltip("生成時のエフェクト")]
    [SerializeField] private ParticleSystem spawnParticles;

    private float timer = 0f;//タイマー

    private void Start()
    {
        //GameManagerのインスタンスを取得
        GM = GameManager.Instance;
    }
    private void Update()
    {
        timer += Time.deltaTime;

        //一定の間隔で現在のオブジェクト数が上限に達していなければオブジェクトを生成
        if (timer >= spawnInterval && GM.spawnCount < maxObject)
        {
            Vector3 randomPosition = GetRandomPosition(); //ランダムな位置を取得

            //上位種の生成確率を計算
            float spawnRate = baseSpawnRate + GM.defeatedEnemies * spawnRateIncreasePerDefeatedEnemy;

            //ランダムに上位種を生成するかどうかを決定
            if (Random.value < spawnRate)
            {
                SpawnHigherTierObject(randomPosition);
            }
            else
            {
                SpawnNormalObject(randomPosition);
            }

            timer = 0f; //タイマーをリセット
        }
    }
    private void SpawnNormalObject(Vector3 position)
    {
        int randomIndex = Random.Range(0, targetObjectPrefabs.Length - 1);
        Instantiate(targetObjectPrefabs[randomIndex], position, Quaternion.identity);
        GM.spawnCount++; //オブジェクト数をカウント

        // 対応する効果音を再生
        PlaySpawnSound(randomIndex, position);

        //対応するエフェクトを再生
        PlaySpawnEffect(position);

    }

    private void SpawnHigherTierObject(Vector3 position)
    {
        int highestTierIndex = targetObjectPrefabs.Length - 1; // 最上位のインデックス
        Instantiate(targetObjectPrefabs[highestTierIndex], position, Quaternion.identity);
        GM.spawnCount++; //オブジェクト数をカウント

        // 対応する効果音を再生
        PlaySpawnSound(highestTierIndex, position);

        //対応するエフェクトを再生
        PlaySpawnEffect(position);

    }

    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f);//範囲内でランダムな位置を設定(X座標)
        float randomY = Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f);//範囲内でランダムな位置を設定(Y座標)
        float randomZ = Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f);//範囲内でランダムな位置を設定(Z座標)

        //ランダムなX座標とZ座標が近すぎる場合は再設定する
        while (Mathf.Abs(randomX) < spawnMinDistance && Mathf.Abs(randomZ) < spawnMinDistance)
        {
            randomX = Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f);
            randomZ = Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f);
        }

        return new Vector3(randomX, randomY, randomZ);
    }

    private void PlaySpawnSound(int index, Vector3 soundPosition)
    {
        if (index >= 0 && index < spawnSounds.Length)
        {
            AudioSource.PlayClipAtPoint(spawnSounds[index], soundPosition);
        }
        else
        {
            AudioSource.PlayClipAtPoint(defaultSpawnSound, soundPosition);
        }
    }
    private void PlaySpawnEffect(Vector3 particlePosition)
    {
        ParticleSystem effect = Instantiate(spawnParticles, particlePosition, Quaternion.identity);
        effect.Play();
        Destroy(effect.gameObject, effect.main.duration);
    }
}
