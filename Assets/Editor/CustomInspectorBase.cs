using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections;

//カスタムエディタのベースクラス
public abstract class CustomInspectorBase : Editor
{
    //ターゲットのクラスの説明を取得する抽象メソッド
    protected abstract string GetClassDescription();

    //インスペクターの描画をカスタマイズ
    public override void OnInspectorGUI()
    {
        //ターゲットのクラスの名前と説明を表示
        GUILayout.Label(target.GetType().Name, EditorStyles.boldLabel);
        GUILayout.Label(GetClassDescription(), EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);

        //デフォルトのインスペクターを描画
        DrawDefaultInspector();

        //ターゲットの型を取得
        System.Type type = target.GetType();

        //静的フィールドを取得
        FieldInfo[] staticFields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        //各静的フィールドをインスペクターに表示
        foreach (FieldInfo field in staticFields)
        {
            object value = field.GetValue(null); //静的フィールドの値を取得

            //値が null でない場合にフィールドを表示
            if (value != null)
            {
                DisplayField(field, value); //フィールドの値を適切に表示
            }
        }
    }

    //値を適切な形式で表示するヘルパーメソッド
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

//各ターゲットクラスに対応するカスタムエディタ
[CustomEditor(typeof(GameManager))]
public class GameManagerClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私はこのゲームの情報を管理しています";
    }
}

[CustomEditor(typeof(TargetSpawn))]
public class TargetSpawnClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私はこのゲームのターゲットの生成を管理しています";
    }
}

[CustomEditor(typeof(TargetManager))]
public class TargetManagerClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私はこのゲームのターゲットの情報を管理しています";
    }
}

[CustomEditor(typeof(NormalTargetMove))]
public class NormalTargetMoveClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私はノーマルターゲットを動かしています";
    }
}

[CustomEditor(typeof(HandTargetMove))]
public class HandTargetMoveClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私はハンドターゲットを動かしています";
    }
}

[CustomEditor(typeof(MissileTargetMove))]
public class MissileTargetMoveClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私はミサイルターゲットを動かしています";
    }
}

[CustomEditor(typeof(ARGunController))]
public class ARGunControllerClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私はこのゲームの銃を管理しています";
    }
}

[CustomEditor(typeof(StartScenes))]
public class StartScenesrClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私はスタートシーンを管理しています";
    }
}

[CustomEditor(typeof(EndScenesManager))]
public class EndScenesUIClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私はエンドシーンを管理しています";
    }
}

[CustomEditor(typeof(ShootingUIcontroller))]
public class ShootingUIcontrollerClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私はHPバーと照準を管理しています";
    }
}

[CustomEditor(typeof(CameraPositionAdjusterWithKalman))]
public class CameraPositionAdjusterWithKalmanClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私は空間のずれを修正しています";
    }
}

[CustomEditor(typeof(CameraPositionAdjuster))]
public class CameraPositionAdjusterClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私は空間のずれを修正しています";
    }
}

[CustomEditor(typeof(SceneController))]
public class SceneControllerClassEditor : CustomInspectorBase
{
    protected override string GetClassDescription()
    {
        return "私はシーン情報のみを管理しています";
    }
}

[CustomEditor(typeof(CommandManager))]
public class CommandManagerEditor : Editor
{
    private SerializedProperty SceneControllerProperty; //CommandManagerのシリアライズされた変数のプロパティ

    private void OnEnable()
    {
        SceneControllerProperty = serializedObject.FindProperty("sceneControllerTag"); //commandInputのプロパティを取得
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //スクリプトの選択フィールドを描画
        DrawScriptField();

        // commandInputのフィールドを描画
        EditorGUILayout.PropertyField(SceneControllerProperty);

        //ターゲットのCommandManagerを取得
        CommandManager commandManager = (CommandManager)target;

        //改行
        EditorGUILayout.Space();

        //クラスの説明を表示
        EditorGUILayout.LabelField("私はコマンドを司っています", EditorStyles.boldLabel);

        //コマンド入力フィールド
        commandManager.commandInput = EditorGUILayout.TextField("Command Input", commandManager.commandInput);

        //コマンド実行ボタン
        if (GUILayout.Button("Execute Command"))
        {
            commandManager.ExecuteCommand();
        }

        EditorGUILayout.Space();

        //コマンド一覧を表示
        EditorGUILayout.LabelField("Command List", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("- start-s");
        EditorGUILayout.LabelField("- game-s");
        EditorGUILayout.LabelField("- end-s");

        serializedObject.ApplyModifiedProperties();
    }

    //スクリプトの選択フィールドを描画するメソッド
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