using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlayerAttackTemplate))]
public class PlayerAttackTemplateInspector : Editor {

    public override void OnInspectorGUI(){

        GUILayout.Label(target.name + ":", EditorStyles.boldLabel);

        DrawDefaultInspector();

    }

    [MenuItem("PlayerAttack/Create PlayerAttack Template")]
    public static void CreatePlayerAttackTemplate(){
        string path = "Assets/ScriptableObjects/PlayerAttacks/New PlayerAttackTemplate.asset";
        var item = ScriptableObject.CreateInstance("PlayerAttackTemplate");

        AssetDatabase.CreateAsset(item, path);
        AssetDatabase.SaveAssets();

        Selection.activeObject = item;

    }

}
