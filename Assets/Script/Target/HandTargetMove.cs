using System;
using System.Collections.Generic;
using UnityEngine;

public class HandTargetMove : MonoBehaviour
{
    public static bool isNear = false;//�v���C���[�ɋ߂Â��Ă����Ԃ��ǂ����̃t���O
    [SerializeField] private static List<HandTargetMove> allInstances = new List<HandTargetMove>();//���ׂẴC���X�^���X���Ǘ����郊�X�g

    private GameManager gameManager;//�Q�[���}�l�[�W���[�̎Q��
    private TargetManager myTargetManager;//���g�̃^�[�Q�b�g�}�l�[�W���[
    [SerializeField] private Rigidbody rigidbodyComponent;//Rigidbody�R���|�[�l���g�i�C���X�y�N�^�[����ݒ�\�j
    [SerializeField] private Collider colliderComponent;//Collider�R���|�[�l���g�i�C���X�y�N�^�[����ݒ�\�j
    [SerializeField] private Camera mainCamera;//���C���J�����̎Q��

    //�p�����[�^�[
    [Tooltip("�v���C���[�ɋ߂Â��X�s�[�h")]
    [SerializeField] private float targetSpeed = 1f;
    [Tooltip("�v���C���[�Ƃ̍ŏ��̋���")]
    [SerializeField] private float minDistance = 1f;
    [Tooltip("�_���[�W��^���鋗��")]
    [SerializeField] private float moveDistance = 1f;
    [Tooltip("�_���[�W��^����Ԋu����")]
    [SerializeField] private float damageInterval = 2f;
    private float timer;//�_���[�W��^���邽�߂̃^�C�}�[
    [Tooltip("�͂񂾏�Ԃł̈ړ����x")]
    [SerializeField] private float isGrabbedMoveSpeed = 50f;

    [NonSerialized] public static bool hasVibrated = false;//�o�C�u���[�V�������s�������ǂ����̃t���O

    void Awake()
    {
        InitializeComponents();//�R���|�[�l���g�̏�����
        gameManager = GameManager.Instance;//�Q�[���}�l�[�W���[�̃C���X�^���X���擾
        mainCamera = Camera.main;//���C���J�������擾

        allInstances.RemoveAll(item => item == null);//null�̗v�f���폜
        allInstances.Add(this);//�������g��S�ẴC���X�^���X���Ǘ����郊�X�g�ɒǉ�

        if (isNear)
        {
            StopMovement();//�v���C���[�ɋ߂Â��Ă����ԂȂ�ړ����~����
        }
    }

    void FixedUpdate()
    {
        if (rigidbodyComponent == null)
        {
            return;//Rigidbody�����݂��Ȃ��ꍇ�͏������X�L�b�v
        }

        float distance = Vector3.Distance(myTargetManager.playerTransform.position, transform.position);
        transform.LookAt(myTargetManager.playerTransform);//�v���C���[�̕���������

        if (distance >= minDistance && !isNear)
        {
            MoveTowardsPlayer();//�v���C���[�Ɍ������Ĉړ�
        }
        else if (distance <= minDistance)
        {
            HandleProximityToPlayer();//�v���C���[�ɋ߂Â����ۂ̏���
        }

        if (isNear)
        {
            AccumulateTimerAndDealDamage(distance);//�^�C�}�[��ݐς��A�_���[�W��^���鏈��
            MoveToFrontOfCamera(distance);//�J�����̑O�Ɉړ�����
        }
    }

    //�R���|�[�l���g�̏�����
    private void InitializeComponents()
    {
        myTargetManager = GetComponent<TargetManager>();//�^�[�Q�b�g�}�l�[�W���[���擾

        EnsureComponents();//�R���|�[�l���g�̑��݂��m�F���A�K�v�ɉ����čĎ擾����
        if (rigidbodyComponent == null)
        {
            return;//Rigidbody�����݂��Ȃ��ꍇ�͏������X�L�b�v
        }
    }

    //�R���|�[�l���g�̑��݂��m�F���A�K�v�ɉ����čĎ擾����
    private void EnsureComponents()
    {
        if (rigidbodyComponent == null)
        {
            rigidbodyComponent = GetComponent<Rigidbody>();//Rigidbody�R���|�[�l���g���擾
        }

        if (colliderComponent == null)
        {
            colliderComponent = GetComponent<Collider>();//Collider�R���|�[�l���g���擾
        }
    }

    //�v���C���[�Ɍ������Ĉړ�����
    private void MoveTowardsPlayer()
    {
        Vector3 localDirection = new Vector3(0, 0, targetSpeed);//���[�J�������̈ړ��x�N�g����ݒ�
        Vector3 worldDirection = transform.TransformDirection(localDirection);//���[���h�����ɕϊ�
        rigidbodyComponent.velocity = worldDirection;//Rigidbody�̑��x��ݒ�
        colliderComponent.isTrigger = false;//Collider��ʏ�̏�Ԃɐݒ�
    }

    //�v���C���[�ɋ߂Â����ۂ̏���
    private void HandleProximityToPlayer()
    {
        rigidbodyComponent.velocity = Vector3.zero;//�ړ����~
        colliderComponent.isTrigger = true;//Collider���g���K�[��Ԃɐݒ�
        gameManager.IsGrabbed = true;//�Q�[���}�l�[�W���[�ɒ͂񂾏�Ԃ�ʒm
        //PerformVibration();//�o�C�u���[�V�������������s

        //�ŏ��̃I�u�W�F�N�g��isNear��true�ɂ����ꍇ�̏���
        if (!isNear)
        {
            isNear = true;//isNear�t���O��true�ɐݒ�
            //���̂��ׂẴC���X�^���X�̓�����~����
            foreach (var instance in allInstances)
            {
                if (instance != this)
                {
                    instance.StopMovement();//������~
                }
            }
        }
    }

    //������~���郁�\�b�h
    private void StopMovement()
    {
        EnsureComponents();//�R���|�[�l���g�̑��݂��m�F

        if (rigidbodyComponent == null)
        {
            return;//Rigidbody�����݂��Ȃ��ꍇ�͏������X�L�b�v
        }

        rigidbodyComponent.velocity = Vector3.zero;//�ړ����~
        colliderComponent.isTrigger = true;//Collider���g���K�[��Ԃɐݒ�
    }

    //�J�����̑O�Ɉړ�����
    private void MoveToFrontOfCamera(float cameraDistance)
    {
        if (cameraDistance <= moveDistance)
        {
            Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * minDistance;//�J�����̑O���̖ڕW�ʒu���v�Z
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, isGrabbedMoveSpeed * Time.deltaTime);//�ڕW�ʒu�Ɍ������Ĉړ�
        }
    }

    //�^�C�}�[��ݐς��A�_���[�W��^����
    private void AccumulateTimerAndDealDamage(float damageDistance)
    {
        timer += Time.deltaTime;//�^�C�}�[��ݐ�
        if (timer >= damageInterval && damageDistance <= moveDistance)
        {
            myTargetManager.DealDamage();//�_���[�W��^����
            timer = 0f;//�^�C�}�[�����Z�b�g
        }
    }

    //�o�C�u���[�V�������������s
    /*
    private void PerformVibration()
    {
        if (!hasVibrated)
        {
            VibrateScript.Instance.StartVibration();//�o�C�u���[�V�������J�n
            hasVibrated = true;//�o�C�u���[�V���������s���ꂽ���Ƃ��L�^
        }
    }
    */
}
