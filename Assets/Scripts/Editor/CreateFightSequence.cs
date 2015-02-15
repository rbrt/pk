using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateFightSequence : MonoBehaviour {

	[MenuItem("Custom/Create New Fight Sequence")]
	public static void CreateNewFightSequence(){
		var newSequence = new GameObject("New Fight");
		newSequence.transform.position = Vector3.zero;
		newSequence.transform.rotation = Quaternion.Euler(Vector3.zero);

		var fightSequence = new GameObject("FightSequence");
		fightSequence.transform.parent = newSequence.transform;
		var fightSequenceComponent = fightSequence.AddComponent<FightSequence>();

		var enemyEncounter = new GameObject("EnemyEncounter");
		enemyEncounter.transform.parent = newSequence.transform;
		var enemyEncounterComponent = enemyEncounter.AddComponent<EnemyEncounter>();
		enemyEncounterComponent.EnemyControllerAccess = GameObject.FindObjectOfType<EnemyController>();
		enemyEncounterComponent.FightSequenceAccess = fightSequenceComponent;

		var encounterCollider = newSequence.AddComponent<BoxCollider>();
		encounterCollider.size = new Vector3(2, 5, 5);
		encounterCollider.isTrigger = true;

		var encounterTrigger = newSequence.AddComponent<EnemyEncounterTrigger>();
		encounterTrigger.EnemyEncounterAccess = enemyEncounter.GetComponent<EnemyEncounter>();

		fightSequenceComponent.SetFightSequenceReferences(GameObject.FindObjectOfType<EnemyController>(),
														  enemyEncounter.GetComponent<EnemyEncounter>(),
														  GameObject.FindObjectOfType<FindFightPositions>());

	}

}
