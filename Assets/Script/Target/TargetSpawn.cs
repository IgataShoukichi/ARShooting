using UnityEngine;

public class TargetSpawn : MonoBehaviour
{
    private GameManager GM;

    //�p�����[�^�[
    [Tooltip("��������I�u�W�F�N�g�̃v���n�u")]
    [SerializeField] private GameObject[] targetObjectPrefabs;
    [Tooltip("�I�u�W�F�N�g�𐶐�����Ԋu�i�b)")]
    [SerializeField] private float spawnInterval = 5f;
    [Tooltip("�I�u�W�F�N�g�𐶐�����͈͂̃T�C�Y")]
    [SerializeField] private float spawnAreaSize = 20f;
    [Tooltip("�v���C���[�Ƃ̍ŏ�����")]
    [SerializeField] private float spawnMinDistance = 10f;
    [Tooltip("�I�u�W�F�N�g�̏��")]
    [SerializeField] private int maxObject = 10;
    [Tooltip("��{�̏�ʎ�̏o����")]
    [SerializeField] private float baseSpawnRate = 0.02f;
    [Tooltip("�|���ꂽ�G���Ƃ̏�ʎ�̏o����������")]
    [SerializeField] private float spawnRateIncreasePerDefeatedEnemy = 0.05f;

    //�T�E���h
    [Tooltip("�������̌��ʉ�")]
    [SerializeField] private AudioClip[] spawnSounds;
    [Tooltip("�㗝�̉�")]
    [SerializeField] private AudioClip defaultSpawnSound;

    //�G�t�F�N�g
    [Tooltip("�������̃G�t�F�N�g")]
    [SerializeField] private ParticleSystem spawnParticles;

    private float timer = 0f;//�^�C�}�[

    private void Start()
    {
        //GameManager�̃C���X�^���X���擾
        GM = GameManager.Instance;
    }
    private void Update()
    {
        timer += Time.deltaTime;

        //���̊Ԋu�Ō��݂̃I�u�W�F�N�g��������ɒB���Ă��Ȃ���΃I�u�W�F�N�g�𐶐�
        if (timer >= spawnInterval && GM.spawnCount < maxObject)
        {
            Vector3 randomPosition = GetRandomPosition(); //�����_���Ȉʒu���擾

            //��ʎ�̐����m�����v�Z
            float spawnRate = baseSpawnRate + GM.defeatedEnemies * spawnRateIncreasePerDefeatedEnemy;

            //�����_���ɏ�ʎ�𐶐����邩�ǂ���������
            if (Random.value < spawnRate)
            {
                SpawnHigherTierObject(randomPosition);
            }
            else
            {
                SpawnNormalObject(randomPosition);
            }

            timer = 0f; //�^�C�}�[�����Z�b�g
        }
    }
    private void SpawnNormalObject(Vector3 position)
    {
        int randomIndex = Random.Range(0, targetObjectPrefabs.Length - 1);
        Instantiate(targetObjectPrefabs[randomIndex], position, Quaternion.identity);
        GM.spawnCount++; //�I�u�W�F�N�g�����J�E���g

        // �Ή�������ʉ����Đ�
        PlaySpawnSound(randomIndex, position);

        //�Ή�����G�t�F�N�g���Đ�
        PlaySpawnEffect(position);

    }

    private void SpawnHigherTierObject(Vector3 position)
    {
        int highestTierIndex = targetObjectPrefabs.Length - 1; // �ŏ�ʂ̃C���f�b�N�X
        Instantiate(targetObjectPrefabs[highestTierIndex], position, Quaternion.identity);
        GM.spawnCount++; //�I�u�W�F�N�g�����J�E���g

        // �Ή�������ʉ����Đ�
        PlaySpawnSound(highestTierIndex, position);

        //�Ή�����G�t�F�N�g���Đ�
        PlaySpawnEffect(position);

    }

    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f);//�͈͓��Ń����_���Ȉʒu��ݒ�(X���W)
        float randomY = Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f);//�͈͓��Ń����_���Ȉʒu��ݒ�(Y���W)
        float randomZ = Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f);//�͈͓��Ń����_���Ȉʒu��ݒ�(Z���W)

        //�����_����X���W��Z���W���߂�����ꍇ�͍Đݒ肷��
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
