using UnityEngine;
public class NormalTargetMove : MonoBehaviour
{
    private TargetManager myTargetManager;//TargetManager

    //�p�����[�^�[
    [Tooltip("�㉺�ړ��̑��x")]
    [SerializeField] private float UDspeed = 2f;
    [Tooltip("���E�ړ��̑��x")]
    [SerializeField] private float LRspeed = 2f;
    [Tooltip("�A���t�@�l�̕ω��̑��x")]
    [SerializeField] private float alphaChangeSpeed = 0.5f;
    [Tooltip("�g��k���̑��x")]
    [SerializeField] private float scaleChangeSpeed = 0.1f;
    [Tooltip("�v���C���[�ɋ߂Â��X�s�[�h")]
    [SerializeField] private float targetSpeed = 1f;
    [Tooltip("����Ƃ̍ŏ��̋���")]
    [SerializeField] private float minDistance = 1f;

    //�I�u�W�F�N�g�̓����̎�ނ��`����񋓌^
    public enum ActionPattern
    {
        UPDOWN,    //�㉺�ړ�
        LEFTRIGHT, //���E�ړ�
        ALPHA,     //�A���t�@�l�̕ω�
        SCALE      //�g��k��
    }
    private ActionPattern currentPattern;//���݂̓����̎��

    //�����̃A���t�@�l�ƃX�P�[��
    private float initialAlpha;
    private Vector3 initialScale;

    private new Renderer renderer;//�X�v���C�g�̃����_���[

    private Rigidbody rb;

    void Start()
    {
        myTargetManager = GetComponent<TargetManager>(); // TargetManager�R���|�[�l���g���擾

        rb = GetComponent<Rigidbody>();

        //�X�v���C�g�̃����_���[���擾
        //SpriteRenderer���A�^�b�`����Ă��Ȃ��ꍇ�A�V�����ǉ�����
        if (GetComponent<Renderer>() == null)
        {
            renderer = gameObject.AddComponent<Renderer>();
        }
        else
        {
            renderer = GetComponent<Renderer>();
        }
        //�����̃A���t�@�l��ۑ�
        initialAlpha = renderer.material.color.a;

        //�����̃X�P�[����ۑ�
        initialScale = transform.localScale;

        //�����_���ȓ����̎�ނ�I�����ĊJ�n
        SwitchMovementPattern();

        //�v���C���[�̈ʒu��Tag����擾
        GameObject Player = GameObject.FindWithTag("Player");
        myTargetManager.playerTransform = Player.transform;
    }

    void Update()
    {
        //�v���C���[�Ƃ̋����𑪂�
        var distance = Vector3.Distance(myTargetManager.playerTransform.transform.position, transform.position);

        //���݂̓����̎�ނɉ����ē�������s
        switch (currentPattern)
        {
            case ActionPattern.UPDOWN:
                //�㉺�ړ�
                transform.Translate(Vector3.up * Mathf.Sin(Time.time * UDspeed) * Time.deltaTime);
                break;

            case ActionPattern.LEFTRIGHT:
                //���E�ړ�
                transform.Translate(Vector3.right * Mathf.Sin(Time.time * LRspeed) * Time.deltaTime);
                break;

            case ActionPattern.ALPHA:
                //�A���t�@�l�̕ω�
                float alphaValue = Mathf.PingPong(Time.time * alphaChangeSpeed, 1f);
                Color newColor = renderer.material.color;
                newColor.a = initialAlpha * alphaValue;
                renderer.material.color = newColor;
                // �q�I�u�W�F�N�g�ɂ������A���t�@�l��K�p
                foreach (Transform child in transform)
                {
                    Renderer childRenderer = child.GetComponent<Renderer>();
                    if (childRenderer != null && childRenderer.material != null && childRenderer.material.HasProperty("_Color"))
                    {
                        Color childColor = childRenderer.material.color;
                        childColor.a = initialAlpha * alphaValue;
                        childRenderer.material.color = childColor;
                    }
                }
                break;

            case ActionPattern.SCALE:
                //�g��k��
                float scaleValue = Mathf.PingPong(Time.time * scaleChangeSpeed, 1f);
                transform.localScale = initialScale * scaleValue;
                break;

            default:
                break;
        }

        //�v���C���[�̕���������
        transform.LookAt(myTargetManager.playerTransform);

        //���̋����܂ŋ߂Â�
        if (distance > minDistance)
        {
            //�v���C���[�̕����֐i�s����
            Vector3 localDirection = new Vector3(0, 0, targetSpeed);
            Vector3 worldDirection = transform.TransformDirection(localDirection);
            rb.velocity = worldDirection;
        }
        else
        {
            //�O�i���~�߂�
            rb.velocity = Vector3.zero;
        }
    }

    void SwitchMovementPattern()//����̐؂�ւ��֐�
    {
        //�����_���ȓ����̎�ނ�I��
        currentPattern = (ActionPattern)Random.Range(0, 4);
    }
}
