using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(EnemyEncounter))]
public class EnemyEncounterInspector : Editor {

	int lastCount;

	public override void OnInspectorGUI(){
		var encounter = (target as EnemyEncounter);
		int typesCount = encounter.EnemyTypesCount;

		GUILayout.Label("Enemy Types");

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add")){
			var prefabs = encounter.EnemyPrefabs;
			prefabs.Add(null);
			encounter.EnemyPrefabs = prefabs;

			var counts = encounter.EnemiesToSpawn;
			counts.Add(0);
			encounter.EnemiesToSpawn = counts;

			encounter.EnemyTypesCount++;
		}

		if (GUILayout.Button("Clear")){
			encounter.EnemiesToSpawn = new List<int>();
			encounter.EnemyPrefabs = new List<GameObject>();
			encounter.EnemyTypesCount = 0;
		}

		EditorGUILayout.EndHorizontal();

		var keys = encounter.EnemyPrefabs;
		var values = encounter.EnemiesToSpawn;

		if (keys == null || values == null){
			return;
		}

		bool refreshKeys = false,
			 refreshValues = false;

		List<int> indicesToRemove = new List<int>();

		GUILayout.Space(5);

		EditorGUILayout.BeginVertical(EditorStyles.textArea);
		GUILayout.Space(5);

		for (int i = 0; i < typesCount; i++){

			EditorGUILayout.BeginHorizontal();

			if (i < keys.Count){
				GUILayout.Label("Enemy: ");
				GameObject currentKey = keys[i];
				currentKey = EditorGUILayout.ObjectField(currentKey, typeof(GameObject), allowSceneObjects: true) as GameObject;

				if (currentKey != keys[i]){
					if (keys.Contains(currentKey)){
						Debug.LogWarning("Cannot add same enemy type for multiple keys");
					}
					else{
						refreshKeys = true;
						keys[i] = currentKey;
						Debug.Log(currentKey.ToString());
					}
				}
			}

			if (i < values.Count){
				GUILayout.Label("Count: ");
				int currentValue = values[i];
				currentValue = EditorGUILayout.IntField(currentValue, EditorStyles.label);
				if (currentValue != values[i]){
					refreshValues = true;
					values[i] = currentValue;
				}
			}

			if (GUILayout.Button("Del")){
				indicesToRemove.Add(i);
			}

			EditorGUILayout.EndHorizontal();
			GUILayout.Space(5);
		}

		EditorGUILayout.EndVertical();

		if (indicesToRemove.Count > 0){
			refreshKeys = true;
			refreshValues = true;

			for (int i = indicesToRemove.Count - 1; i >= 0; i--){
				keys.RemoveAt(indicesToRemove[i]);
				values.RemoveAt(indicesToRemove[i]);
				typesCount = encounter.EnemyTypesCount--;
			}
		}

		if (refreshKeys){
			encounter.EnemyPrefabs = keys;
		}

		if (refreshValues){
			encounter.EnemiesToSpawn = values;
		}

	}
}
