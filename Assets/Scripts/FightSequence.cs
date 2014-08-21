using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FightSequence : MonoBehaviour {

    [SerializeField] protected Transform[] fightPositions;
    [SerializeField] protected EnemyController enemyController;
    [SerializeField] protected EnemySpawn enemySpawn;

    protected Transform playerTransform;
    protected GameObject[] fightingEnemies;
    protected int simulataneousAttackingEnemies = 4;

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
	}

    void BringIdleEnemiesInToTheFight(){
        if (enemySpawn.DoneSpawning && enemyController.GetIdleEnemy(fightingEnemies.ToList()) != null){
            while (fightingEnemies.Any(x => x == null)){
                var enemy = enemyController.GetIdleEnemy(fightingEnemies.ToList());
                int index = fightingEnemies.ToList().IndexOf(fightingEnemies.First(x => x == null));
                fightingEnemies[index] = enemy;

                enemy.GetComponent<Enemy>().AttackPlayer();
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
