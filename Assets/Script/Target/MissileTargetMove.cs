using Unity.VisualScripting;
using UnityEngine;

public class MissileTargetMove : MonoBehaviour
{
    private TargetManager myTargetManager;//���g��TargetManager

    //�p�����[�^�[
    [Tooltip("�v���C���[�ɋ߂Â��X�s�[�h")]
    [SerializeField] private float targetSpeed = 1.5f;

    [Tooltip("����Ƃ̍ŏ��̋���")]
    [SerializeField] private float minDistance = 0.5f;

    [Tooltip("�Z���t�G�t�F�N�g")]
    [SerializeField] private ParticleSystem selfEffect;

    private Rigidbody myRig;//���̃I�u�W�F�N�g��Rigidbody
    private bool self;//���ł��ǂ����𔻒f����ϐ�

    void Start()
    {
        //���g��TargetManager���擾
        myTargetManager = GetComponent<TargetManager>();

        //Rigidbody���擾
        myRig = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //�v���C���[�Ƃ̋����𑪂�
        var distance = Vector3.Distance(myTargetManager.playerTransform.position, transform.position);

        //�v���C���[�̕���������
        transform.LookAt(myTargetManager.playerTransform);

        //���̋����܂ŋ߂Â�
        if (distance >= minDistance)
        {
            //�v���C���[�̕����֐i�s����
            Vector3 localDirection = new Vector3(0, 0, targetSpeed);
            Vector3 worldDirection = transform.TransformDirection(localDirection);
            myRig.velocity = worldDirection;
        }
        else //distance < minDistance �̏ꍇ
        {
            //�ړ����~�߂�
            myRig.velocity = Vector3.zero;

            //�v���C���[�Ƀ_���[�W��^����
            myTargetManager.DealDamage();

            //�����ŃJ�E���g�Ȃ�
            self = true;

            //�����̎��̃G�t�F�N�g�Đ�
            myTargetManager.PlayEffect(selfEffect);

            //�I�u�W�F�N�g�����ł�����
            StartCoroutine(myTargetManager.DieCoroutine(myTargetManager.myType, self));
        }
    }
}
