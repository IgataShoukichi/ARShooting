using UnityEngine;
using System.Collections.Generic;

public class CommandManager : MonoBehaviour
{
    [SerializeField] private string sceneControllerTag = "SceneController"; //SceneController�̃^�O
    private SceneController SC;
    [SerializeField] private GameObject startScenes;

    public string commandInput;//���[�U�[�����͂����R�}���h������

    //�R�}���h���ƑΉ�����A�N�V�������i�[���鎫��
    private Dictionary<string, System.Action<string[]>> commands;

    private string SceneName;

    void Start()
    {
        //�R�}���h���������������A�R�}���h���ƑΉ����郁�\�b�h��o�^
        commands = new Dictionary<string, System.Action<string[]>>
        {
            { "start-s", ScenesJump_Start },
            { "game-s", ScenesJump_Game },
            { "end-s", ScenesJump_End },
        };

        // SceneController���^�O����擾����
        GameObject sceneControllerObject = GameObject.FindWithTag(sceneControllerTag);
        if (sceneControllerObject != null)
        {
            SC = sceneControllerObject.GetComponent<SceneController>();
        }
        else
        {
            Debug.LogError("SceneController not found with tag: " + sceneControllerTag);
        }
    }

    //���͂��ꂽ�R�}���h�����s���郁�\�b�h
    public void ExecuteCommand()
    {
        //�R�}���h���͂��󔒂܂���null�̏ꍇ�͉������Ȃ�
        if (string.IsNullOrWhiteSpace(commandInput))
            return;

        //�R�}���h���͂��X�y�[�X�ŕ������A�R�}���h���ƈ������擾
        string[] parts = commandInput.Trim().ToLower().Split(' ');
        string command = parts[0];
        string[] args = parts.Length > 1 ? parts[1..] : new string[0];

        //�R�}���h�������ɑ��݂��邩�m�F���A���݂���ΑΉ�����A�N�V���������s
        if (commands.ContainsKey(command))
        {
            commands[command].Invoke(args);
            Debug.Log(command + "���s");
        }
        else
        {
            //�R�}���h���s���ȏꍇ�̃��b�Z�[�W��\��
            Debug.Log("Unknown command: " + command);
        }

        //�R�}���h���̓t�B�[���h���N���A
        commandInput = "";
    }

    //�X�^�[�g�V�[���Ɉړ����郁�\�b�h
    void ScenesJump_Start(string[] args)
    {
        Debug.Log("�X�^�[�g�V�[���Ɉړ����܂�");
        SceneName = "StartScenes";//�V�[�����w��
        SC.ChangeScene(SceneName);//�w�肳�ꂽ�V�[�������[�h
    }
    //�Q�[���V�[���Ɉړ����郁�\�b�h
    void ScenesJump_Game(string[] args)
    {
        Debug.Log("�Q�[���V�[���Ɉړ����܂�");
        SceneName = "ShootingGame";
    }
    //�G���h�V�[���Ɉړ����郁�\�b�h
    void ScenesJump_End(string[] args)
    {
        Debug.Log("�G���h�V�[���Ɉړ����܂�");
        SceneName = "EndScenes";
        SC.ChangeScene(SceneName);
    }

}
