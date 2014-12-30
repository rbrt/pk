using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class EnemyEncounterTrigger : MonoBehaviour {

	GameObject playerObject;
	[SerializeField] protected EnemyEncounter enemyEncounter;

	void Awake(){
		if (!GetComponent<Collider>().isTrigger){
			Debug.LogError("Collider must be a trigger", this.gameObject);
		}

		playerObject = GameObject.FindObjectOfType<PlayerController>().gameObject;
	}

	void OnTriggerEnter(Collider other){
		Debug.Log(other.gameObject == playerObject);

		if (other.gameObject == playerObject){
			enemyEncounter.SpawnEnemies();
		}
	}

}
