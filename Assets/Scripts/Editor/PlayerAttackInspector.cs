using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

[CustomEditor(typeof(PlayerAttack))]
public class PlayerAttackInspector : Editor {



    // Would be nice if this could refresh the project view after updating the prefab, and if
    // it could update the prefab after cleaning up a deletion

    public override void OnInspectorGUI(){
        base.DrawDefaultInspector();
        PlayerAttack playerAttack = target as PlayerAttack;

        // Update if the attack lists were altered
        bool update = playerAttack.Clean();

        GUILayout.Space(20);

        GUILayout.Label("Attack 1 Moves:");

        if (GUILayout.Button("Add Attack 1 Move")){
            CreateChildPlayerAttack(playerAttack, playerAttack.AddAttack1Move);
        }

        if (DrawPlayerAttacks(playerAttack.Attack1Moves.Where(x => x != null).ToList())){
            update = true;
        }


        GUILayout.Space(20);

        GUILayout.Label("Attack 2 Moves:");

        if (GUILayout.Button("Add Attack 2 Move")){
            CreateChildPlayerAttack(playerAttack, playerAttack.AddAttack2Move);
        }

        if (DrawPlayerAttacks(playerAttack.Attack2Moves.Where(x => x != null).ToList())){
            update = true;
        }

        if (update){
            Debug.Log("updatin");
            var instance = PrefabUtility.FindPrefabRoot(playerAttack.AccessAttackTree.gameObject);
            PrefabUtility.ReplacePrefab(instance, PrefabUtility.GetPrefabParent(instance), ReplacePrefabOptions.ConnectToPrefab);
        }
    }

    static bool DrawPlayerAttacks(List<NextMove> attacks){
        bool update = false;
        attacks.ForEach(move => {
            EditorGUILayout.BeginHorizontal();

                float cutoffTime = move.CutoffTime;
                float newCutoffTime = 0;
                float.TryParse(GUILayout.TextField(""+move.CutoffTime), out newCutoffTime);

                if (cutoffTime != newCutoffTime){
                    move.CutoffTime = newCutoffTime;
                    update = true;
                }

                PlayerAttack nextMove = move.AccessNextMove;
                PlayerAttack newMove = EditorGUILayout.ObjectField(move.AccessNextMove, typeof(GameObject), false) as PlayerAttack;

                if (nextMove != newMove){
                    move.AccessNextMove = newMove;
                    update = true;
                }

            EditorGUILayout.EndHorizontal();
        });

        return update;
    }

    public static void CreateChildPlayerAttack(PlayerAttack parent, Action<PlayerAttack> function){
        GameObject attack = new GameObject();
        attack.transform.parent = parent.transform;
        PlayerAttack item = attack.AddComponent<PlayerAttack>();

        item.Init();
        function(item);
        item.AccessAttackTree = parent.AccessAttackTree;

        var instance = PrefabUtility.FindPrefabRoot(parent.gameObject);
        PrefabUtility.ReplacePrefab(instance, PrefabUtility.GetPrefabParent(instance), ReplacePrefabOptions.ConnectToPrefab);
    }
}
