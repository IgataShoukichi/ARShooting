using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections;

//�J�X�^���G�f�B�^�̃x�[�X�N���X
public abstract class CustomInspectorBase : Editor
{
    //�^�[�Q�b�g�̃N���X�̐������擾���钊�ۃ��\�b�h
    protected abstract string GetClassDescription();

    //�C���X�y�N�^�[�̕`����J�X�^�}�C�Y
    public override void OnInspectorGUI()
    {
        //�^�[�Q�b�g�̃N���X�̖��O�Ɛ�����\��
        GUILayout.Label(target.GetType().Name, EditorStyles.boldLabel);
        GUILayout.Label(GetClassDescription(), EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);

        //�f�t�H���g�̃C���X�y�N�^�[��`��
        DrawDefaultInspector();

        //�^�[�Q�b�g�̌^���擾
        System.Type type = target.GetType();

        //�ÓI�t�B�[���h���擾
        FieldInfo[] staticFields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        //�e�ÓI�t�B�[���h���C���X�y�N�^�[�ɕ\��
        foreach (FieldInfo field in staticFields)
        {
            object value = field.GetValue(null); //�ÓI�t�B�[���h�̒l���擾

            //�l�� null �łȂ��ꍇ�Ƀt�B�[���h��\��
            if (value != null)
            {
                DisplayField(field, value); //�t�B�[���h�̒l��K�؂ɕ\��
            }
        }
    }

    //�l��K�؂Ȍ`���ŕ\������w���p�[���\�b�h
    private void DisplayField(FieldInfo field, object value)
    {
        if (value is int)
        {
            EditorGUILayout.IntField(field.Name, (int)value);
        }
        else if (value is float)
        {
            EditorGUILayout.FloatField(field.Name, (float)value);
        }
        else if (value is bool)
        {
            EditorGUILayout.Toggle(field.Name, (bool)value);
        }
        else if (value is string)
        {
            EditorGUILayout.TextField(field.Name, (string)value);
        }
        else if (value is IList)
        {
            IList list = (IList)value;
            EditorGUILayout.LabelField(field.Name);
            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.LabelField($"Element {i}", list[i]?.ToString() ?? "null");
            }
            EditorGUI.indentLevel--;
        }
        else
        {
            EditorGUILayout.LabelField(field.Name, value.ToString());
        }
    }
}

//�e�^�[�Q�b�g�N���X�ɑΉ�����J�X�^���G�f�B�^
[CustomEditor(typeof(GameManager))]
public class GameManagerClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���͂��̃Q�[���̏����Ǘ����Ă��܂�";
    }
}

[CustomEditor(typeof(TargetSpawn))]
public class TargetSpawnClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���͂��̃Q�[���̃^�[�Q�b�g�̐������Ǘ����Ă��܂�";
    }
}

[CustomEditor(typeof(TargetManager))]
public class TargetManagerClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���͂��̃Q�[���̃^�[�Q�b�g�̏����Ǘ����Ă��܂�";
    }
}

[CustomEditor(typeof(NormalTargetMove))]
public class NormalTargetMoveClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���̓m�[�}���^�[�Q�b�g�𓮂����Ă��܂�";
    }
}

[CustomEditor(typeof(HandTargetMove))]
public class HandTargetMoveClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���̓n���h�^�[�Q�b�g�𓮂����Ă��܂�";
    }
}

[CustomEditor(typeof(MissileTargetMove))]
public class MissileTargetMoveClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���̓~�T�C���^�[�Q�b�g�𓮂����Ă��܂�";
    }
}

[CustomEditor(typeof(ARGunController))]
public class ARGunControllerClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���͂��̃Q�[���̏e���Ǘ����Ă��܂�";
    }
}

[CustomEditor(typeof(StartScenes))]
public class StartScenesrClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���̓X�^�[�g�V�[�����Ǘ����Ă��܂�";
    }
}

[CustomEditor(typeof(EndScenesManager))]
public class EndScenesUIClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���̓G���h�V�[�����Ǘ����Ă��܂�";
    }
}

[CustomEditor(typeof(ShootingUIcontroller))]
public class ShootingUIcontrollerClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "����HP�o�[�ƏƏ����Ǘ����Ă��܂�";
    }
}

[CustomEditor(typeof(CameraPositionAdjusterWithKalman))]
public class CameraPositionAdjusterWithKalmanClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���͋�Ԃ̂�����C�����Ă��܂�";
    }
}

[CustomEditor(typeof(CameraPositionAdjuster))]
public class CameraPositionAdjusterClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���͋�Ԃ̂�����C�����Ă��܂�";
    }
}

[CustomEditor(typeof(SceneController))]
public class SceneControllerClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "���̓V�[�����݂̂��Ǘ����Ă��܂�";
    }
}

[CustomEditor(typeof(CommandManager))]
public class CommandManagerEditor : Editor
{
    private SerializedProperty SceneControllerProperty; //CommandManager�̃V���A���C�Y���ꂽ�ϐ��̃v���p�e�B

    private void OnEnable()
    {
        SceneControllerProperty = serializedObject.FindProperty("sceneControllerTag"); //commandInput�̃v���p�e�B���擾
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //�X�N���v�g�̑I���t�B�[���h��`��
        DrawScriptField();

        // commandInput�̃t�B�[���h��`��
        EditorGUILayout.PropertyField(SceneControllerProperty);

        //�^�[�Q�b�g��CommandManager���擾
        CommandManager commandManager = (CommandManager)target;

        //���s
        EditorGUILayout.Space();

        //�N���X�̐�����\��
        EditorGUILayout.LabelField("���̓R�}���h���i���Ă��܂�", EditorStyles.boldLabel);

        //�R�}���h���̓t�B�[���h
        commandManager.commandInput = EditorGUILayout.TextField("Command Input", commandManager.commandInput);

        //�R�}���h���s�{�^��
        if (GUILayout.Button("Execute Command"))
        {
            commandManager.ExecuteCommand();
        }

        EditorGUILayout.Space();

        //�R�}���h�ꗗ��\��
        EditorGUILayout.LabelField("Command List", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("- start-s");
        EditorGUILayout.LabelField("- game-s");
        EditorGUILayout.LabelField("- end-s");

        serializedObject.ApplyModifiedProperties();
    }

    //�X�N���v�g�̑I���t�B�[���h��`�悷�郁�\�b�h
    private void DrawScriptField()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target as MonoBehaviour), typeof(MonoScript), false);

        EditorGUI.EndDisabledGroup();

        if (EditorGUI.EndChangeCheck())
        {
            MonoScript script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);
            if (script != null)
            {
                AssetDatabase.OpenAsset(script);
            }
        }
    }
}