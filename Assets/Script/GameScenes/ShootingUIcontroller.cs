using UnityEngine;
using UnityEngine.UI;

public class ShootingUIcontroller : MonoBehaviour
{
    //UI�v���n�u�̎Q��
    [Tooltip("HPBar�̃v���n�u")]
    [SerializeField] private GameObject HPBarPrefab;//HP�o�[�̃v���n�u
    [Tooltip("ReticleUI�̃v���n�u")]
    [SerializeField] private GameObject reticleUIPrefab;//���e�B�N��UI�̃v���n�u
    public Image HPBar;//���ۂ�HP�o�[��Image�R���|�[�l���g
    public Image reticleUI;//���ۂ̃��e�B�N��UI��Image�R���|�[�l���g

    //�V���O���g���C���X�^���X�̕ێ�
    private static ShootingUIcontroller instance;

    //�V���O���g���C���X�^���X�ւ̃A�N�Z�X�p�v���p�e�B
    public static ShootingUIcontroller Instance
    {
        get
        {
            //�C���X�^���X�����݂��Ȃ��ꍇ�̓G���[���b�Z�[�W���o��
            if (instance == null)
            {
                Debug.LogError("ShootingUIcontroller �C���X�^���X�����݂��܂���");
            }
            return instance;
        }
    }

    void Awake()
    {
        //�V���O���g���p�^�[�����g�p���ďd���C���X�^���X��h��
        if (instance == null)
        {
            instance = this;//�C���X�^���X��ݒ�
            DontDestroyOnLoad(this.gameObject);//�V�[�����܂����ł��I�u�W�F�N�g��j�����Ȃ�
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);//�����̃C���X�^���X������ꍇ�͂��̃I�u�W�F�N�g��j��
            return;
        }

        //UI�v�f�𐶐�����ь������郁�\�b�h���Ăяo��
        GenerateAndFindUIElements();
    }

    //UI�v�f�𐶐�����ь������郁�\�b�h
    private void GenerateAndFindUIElements()
    {
        //Canvas �I�u�W�F�N�g���^�O�Ō���
        GameObject canvasObject = GameObject.FindWithTag("Canvas");

        //Canvas �I�u�W�F�N�g�����݂��Ȃ��ꍇ�̃G���[�n���h�����O
        if (canvasObject == null)
        {
            Debug.LogError("Canvas �I�u�W�F�N�g��������܂���BCanvas �� 'Canvas' �^�O���t���Ă��邱�Ƃ��m�F���Ă��������B");
            return;
        }

        //HPBar �����ݒ�̏ꍇ�̓v���n�u���琶��
        if (HPBar == null)
        {
            //HPBar �v���n�u�� Canvas �̎q�Ƃ��Đ���
            GameObject hpBarObject = Instantiate(HPBarPrefab, canvasObject.transform);
            if (hpBarObject != null)
            {
                HPBar = hpBarObject.GetComponent<Image>();//HPBar �� Image �R���|�[�l���g���擾
                if (HPBar == null)
                {
                    Debug.LogError("HPBar �� Image �R���|�[�l���g��������܂���");
                    return;
                }
            }
            else
            {
                Debug.LogError("HPBar �v���n�u�������ł��܂���");
                return;
            }
        }

        //reticleUI �����ݒ�̏ꍇ�̓v���n�u���琶��
        if (reticleUI == null)
        {
            //reticleUI �v���n�u�� Canvas �̎q�Ƃ��Đ���
            GameObject reticleUIObject = Instantiate(reticleUIPrefab, canvasObject.transform);
            if (reticleUIObject != null)
            {
                reticleUI = reticleUIObject.GetComponent<Image>();//reticleUI �� Image �R���|�[�l���g���擾
                if (reticleUI == null)
                {
                    Debug.LogError("ReticleUI �� Image �R���|�[�l���g��������܂���");
                    return;
                }
            }
            else
            {
                Debug.LogError("ReticleUI �v���n�u�������ł��܂���");
                return;
            }
        }
    }

    //�v���C���[��HP�o�[���X�V���郁�\�b�h
    public void PlayerHPBar(float HPBarNum)
    {
        //HPBar �� null �łȂ����Ƃ��m�F���Ă��� fillAmount ��ݒ�
        if (HPBar != null)
        {
            HPBar.fillAmount = HPBarNum;
        }
        else
        {
            Debug.LogError("HPBar ���ݒ肳��Ă��܂���");
        }
    }
}
