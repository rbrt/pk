﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour {

    [SerializeField] protected List<GameObject> enemyList;
    [SerializeField] protected FightSequence fightSequence;
    [SerializeField] protected Transform leftSpawnLocation,
                                         rightSpawnLocation;

    public List<GameObject> EnemyList {
        get { return enemyList; }
    }

    // Returns an available, idle enemy to be used to join the fight
    public GameObject GetIdleEnemy(List<GameObject> currentEnemies){
        var possibleEnemies = enemyList.Where(x => !currentEnemies.Contains(x)).ToList();

        if (possibleEnemies.Count > 1){
            return possibleEnemies[Random.Range(0, possibleEnemies.Count-1)];
        }

        return null;
    }

    public enum SpawnBehaviour {Random, Left, Right};

    public void SpawnEnemy(GameObject enemyToInstantiate){
        SpawnEnemy(enemyToInstantiate, SpawnBehaviour.Right);
    }

    // Spawns an enemy off either side of the screen
    public void SpawnEnemy(GameObject enemyToInstantiate, SpawnBehaviour spawnBehaviour){
        // Assume right side of the screen
        Vector3 position = rightSpawnLocation.position;

        if (spawnBehaviour == SpawnBehaviour.Random){
            position = (Random.Range(0,100) > 50 ? leftSpawnLocation : rightSpawnLocation).position;
        }
        else if (spawnBehaviour == SpawnBehaviour.Left){
            position = leftSpawnLocation.position;
        }
        
        position.y += (Random.Range(-100, 100) / 100f);

        var enemy = GameObject.Instantiate(enemyToInstantiate, position, Quaternion.Euler(Vector3.zero)) as GameObject;
        enemy.GetComponent<Enemy>().SetFightSequence(fightSequence);
        enemyList.Add(enemy);
    }
}
