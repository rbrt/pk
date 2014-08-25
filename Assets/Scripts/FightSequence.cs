﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FightSequence : MonoBehaviour {

    [SerializeField] protected EnemyController enemyController;
    [SerializeField] protected EnemySpawn enemySpawn;
    [SerializeField] protected FindFightPositions findFightPositions;
    [SerializeField] protected int simulataneousAttackingEnemies;

    protected Transform playerTransform;
    protected GameObject[] fightingEnemies;

	void Start () {
        var player = GameObject.Find("PlayerCharacter");
        if (player == null){
            Debug.LogError("No PlayerCharacter found in current scene!");
        }
        else{
            playerTransform = player.transform;
        }

        fightingEnemies = new GameObject[simulataneousAttackingEnemies];
	}

	void Update () {
        BringIdleEnemiesInToTheFight();

        if (fightingEnemies.Length > 0){
            var positions = findFightPositions.GetEnemyPositions(fightingEnemies);

            for (int i = 0; i < fightingEnemies.Length; i++){
                if (fightingEnemies[i] != null){
                    fightingEnemies[i].GetComponent<Enemy>().Destination = positions[i];
                }
            }
        }
	}

    void BringIdleEnemiesInToTheFight(){
        if (enemySpawn.DoneSpawning){
            if(enemyController.GetIdleEnemy(fightingEnemies.ToList()) != null){
                while (fightingEnemies.Any(x => x == null)){
                    var enemy = enemyController.GetIdleEnemy(fightingEnemies.ToList());
                    if (enemy != null){
                        int index = fightingEnemies.ToList().IndexOf(fightingEnemies.First(x => x == null));
                        fightingEnemies[index] = enemy;
                        enemy.GetComponent<Enemy>().AttackPlayer();
                    }
                    else {
                        break;
                    }
                }
            }
            else{
                List<GameObject> tempList = new List<GameObject>();
                fightingEnemies.Where(x => x != null).ToList().ForEach(x => tempList.Add(x));
                fightingEnemies = tempList.ToArray();
            }
        }
    }

    public void HandleEnemyDeath(GameObject enemyThatDied){
        var enemy = fightingEnemies.FirstOrDefault(x => x == enemyThatDied);
        if (enemy != null){
            int index = fightingEnemies.ToList().IndexOf(enemy);
            fightingEnemies[index] = null;
        }
    }
}
