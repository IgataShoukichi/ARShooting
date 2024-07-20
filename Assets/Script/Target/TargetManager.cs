using UnityEngine;
using System.Collections;

public class TargetManager : MonoBehaviour
{
    private GameManager GM;//GameManager
    public Transform playerTransform;//�v���C���[��Transform 

    // �G�̎�ނ��`����񋓌^
    public enum EnemyType { Default, Normal, Missile, Hand }

    // �p�����[�^�[
    [Tooltip("���g�̎��")]
    [SerializeField] public EnemyType myType = EnemyType.Default;
    [Tooltip("�ő�̗�")]
    [SerializeField] private int maxHealth = 100;
    [Tooltip("�e����󂯂�_���[�W")]
    [SerializeField] private int bulletDamage = 50;
    [Tooltip("�_���[�W��^����^�O")]
    [SerializeField] private string damageTag = "Bullet";
    [Tooltip("�v���C���[���l������o���l")]
    [SerializeField] private int GrantEXP = 10;
    [Tooltip("�v���C���[�ւ̃_���[�W")]
    [SerializeField] private int damageForPlayer = 25;

    // �T�E���h
    [Tooltip("�_���[�W���󂯂����̉�")]
    [SerializeField] private AudioClip damageSound;
    [Tooltip("���ł���Ƃ��̉�")]
    [SerializeField] private AudioClip deathSound;
    [Tooltip("�펞�炵�Ă��鉹")]
    [SerializeField] private AudioClip alwaysSound;
    [Tooltip("�펞���Đ��̊Ԋu")]
    [SerializeField] private float alwaysSoundInterval = 3f;
    private float nextAlwaysSoundTime;

    // �G�t�F�N�g
    [Tooltip("�_���[�W���̃G�t�F�N�g")]
    [SerializeField] private ParticleSystem damageEffect;
    [Tooltip("���S���̃G�t�F�N�g")]
    [SerializeField] private ParticleSystem deathEffect;

    private bool self = false;//���g�ɂ��_���[�W���ǂ���
    private int _currentHealth;//���݂̗̑�

    //�v���p�e�B
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
        //���Ԋu�ŉ����Đ�
        if (Time.time >= nextAlwaysSoundTime)
        {
            PlaySound(alwaysSound);
            nextAlwaysSoundTime = Time.time + alwaysSoundInterval + Random.value;
        }
    }

    //�_���[�W���󂯂郁�\�b�h
    public void TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount;//�_���[�W��̗͂��猸�Z
        PlaySound(damageSound);
        PlayEffect(damageEffect);

        if (_currentHealth <= 0)
        {
            StartCoroutine(DieCoroutine(myType, self));//�̗͂�0�ȉ��ɂȂ����玀�S���������s
        }
    }

    //�^�[�Q�b�g�̃��Z�b�g
    private void ResetTarget()
    {
        _currentHealth = maxHealth;//�ŏ��͍ő�̗͂ŊJ�n
        playerTransform = GameObject.FindWithTag("Player").transform;//�v���C���[��Transform���擾
        self = false;
    }

    // ���S����
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
                    Debug.LogWarning("EnemyType���ݒ肳��Ă��܂���");
                    break;
            }
        }

        //�I�u�W�F�N�g���A�N�e�B�u�ɂ���
        gameObject.SetActive(false);

        //�T�E���h�̍Đ����I���̂�҂�
        yield return new WaitForSeconds(deathSound.length);

        Destroy(gameObject);
    }

    //�v���C���[�Ƀ_���[�W��^���郁�\�b�h
    public void DealDamage()
    {
        GM.currentPlayerHealth -= damageForPlayer;
        GM.NowPlayerHealth();
    }

    //�T�E���h���Đ����郁�\�b�h
    private void PlaySound(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
        }
    }

    //�G�t�F�N�g���Đ����郁�\�b�h
    public void PlayEffect(ParticleSystem particleSystem)
    {
        if (particleSystem != null)
        {
            ParticleSystem effect = Instantiate(particleSystem, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }
    }

    //�Փ˔���
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(damageTag))
        {
            TakeDamage(bulletDamage + (int)GM.gunPower);
            Destroy(col.gameObject);
        }
    }
}
