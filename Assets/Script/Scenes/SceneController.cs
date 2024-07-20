using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static string previousSceneName; //�O�̃V�[������ێ�����ϐ�
    private static SceneController instance; //�V���O���g���C���X�^���X��ێ�����ϐ�

    //�V���O���g���C���X�^���X�ւ̃A�N�Z�X�v���p�e�B
    public static SceneController Instance
    {
        get { return instance; }
    }

    //�O�̃V�[�����ւ̃A�N�Z�X�v���p�e�B
    public static string PreviousSceneName
    {
        get { return previousSceneName; }
    }

    private void Awake()
    {
        //�V���O���g���C���X�^���X�̐ݒ�
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);//�V�[�����܂����ŃI�u�W�F�N�g��ێ�����
        }
        else
        {
            Destroy(gameObject);//���ɃC���X�^���X�����݂���ꍇ�A�d����h�����߂ɔj������
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;//�V�[�������[�h���ꂽ���̃C�x���g��OnSceneLoaded���\�b�h��o�^
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;//�I�u�W�F�N�g���j������鎞�ɓo�^�����C�x���g������
    }

    public void ChangeScene(string newSceneName)
    {
        //���݂̃V�[������ۑ�
        previousSceneName = SceneManager.GetActiveScene().name;

        //�V�����V�[���Ɉړ�
        SceneManager.LoadScene(newSceneName);
    }

    //�V�[�������[�h���ꂽ��̏���
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //�����ɕK�v�ɉ�����������ǉ�����
    }
}
