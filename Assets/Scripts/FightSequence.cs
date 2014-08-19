using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FightSequence : MonoBehaviour {

    [SerializeField] protected Transform[] fightPositions;
    [SerializeField] protected GameObject [] enemies;   // The enemies currently involved in the fight
    protected Transform playerTransform;

	void Start () {
        var player = GameObject.Find("PlayerCharacter");
        if (player == null){
            Debug.LogError("No PlayerCharacter found in current scene!");
        }
        else{
            playerTransform = player.transform;
        }
	}

	void Update () {

	}
}
