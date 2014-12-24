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
			var prefabs = encounter.EnemyPrefabs.ToList();
			prefabs.Add(null);
			encounter.EnemyPrefabs = prefabs;

			var counts = encounter.EnemiesToSpawn.ToList();
			counts.Add(0);
			encounter.EnemiesToSpawn = counts;

			encounter.EnemyTypesCount++;
		}

		if (GUILayout.Button("Clear")){
			encounter.EnemiesToSpawn = new List<int>();
			encounter.EnemyPrefabs = new List<GameObject>();
			encounter.EnemyTypesCount = 0;
		}

		// Handle shrinking of Lists

		// Handle growing of Lists

		// Alter dictionary accordingly
		EditorGUILayout.EndHorizontal();

		var keys = encounter.EnemyPrefabs;
		var values = encounter.EnemiesToSpawn;
		bool refreshKeys = false,
			 refreshValues = false;

		List<int> indicesToRemove() = new List<int>();

		for (int i = 0; i < typesCount; i++){

			EditorGUILayout.BeginHorizontal();

			GUILayout.Label("Enemy: ");
			GameObject currentKey = keys[i];
			currentKey = EditorGUILayout.ObjectField(currentKey, typeof(GameObject)) as GameObject;
			if (currentKey != keys[i]){
				refreshKeys = true;
				keys[i] = currentKey;
				Debug.Log(currentKey.ToString());
			}

			GUILayout.Label("Count: ");
			int currentValue = values[i];
			currentValue = EditorGUILayout.IntField(currentValue, EditorStyles.boldLabel);
			if (currentValue != values[i]){
				refreshValues = true;
				values[i] = currentValue;
			}

			if (GUILayout.Button("Del")){
				indicesToRemove.Add(i)

				// Move this to code post loop
				keys.RemoveAt(i);
				values.RemoveAt(i);
				refreshKeys = true;
				refreshValues = true;
				typesCount = encounter.EnemyTypesCount--;
			}

			EditorGUILayout.EndHorizontal();
		}

		if (refreshKeys){
			encounter.EnemyPrefabs = keys;
		}

		if (refreshValues){
			encounter.EnemiesToSpawn = values;
		}

	}
}
