using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class EnemyEncounterTrigger : MonoBehaviour {

	GameObject playerObject;
	[SerializeField] protected EnemyEncounter enemyEncounter;

	public EnemyEncounter EnemyEncounterAccess{
		get { return enemyEncounter; }
		set { enemyEncounter = value; }
	}

	void Awake(){
		if (!GetComponent<Collider>().isTrigger){
			Debug.LogError("Collider must be a trigger", this.gameObject);
		}

		playerObject = GameObject.FindObjectOfType<PlayerController>().gameObject;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject == playerObject){
			enemyEncounter.StartEncounter();
			GetComponent<Collider>().enabled = false;
		}
	}

}
