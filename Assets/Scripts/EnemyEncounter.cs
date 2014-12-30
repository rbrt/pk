    ﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyEncounter : MonoBehaviour {

    [HideInInspector]
    [SerializeField] protected int enemyTypesCount;
    [HideInInspector]
    [SerializeField] protected GameObject[] enemyPrefabs;
    [HideInInspector]
    [SerializeField] protected int[] enemiesToSpawn;

    [SerializeField] protected EnemyController enemyController;

    protected bool doneSpawning;
    protected int totalEnemyCount,
                  enemiesLeftToSpawn;

    // Dictionaries don't serialize....
    protected Dictionary<GameObject, int> enemyTypeToCountMappings;

    void Awake(){
        enemyTypeToCountMappings = new Dictionary<GameObject, int>();
        for (int i = 0; i < enemyPrefabs.Length; i++){
            enemyTypeToCountMappings[enemyPrefabs[i]] = enemiesToSpawn[i];
        }
    }

    public bool DoneSpawning {
        get { return doneSpawning; }
    }

    public int EnemyTypesCount{
        get { return enemyTypesCount; }
        set { enemyTypesCount = value; }
    }

    public List<GameObject> EnemyPrefabs{
        get { return enemyPrefabs.ToList(); }
        set { enemyPrefabs = value.ToArray(); }
    }

    public List<int> EnemiesToSpawn{
        get { return enemiesToSpawn.ToList(); }
        set { enemiesToSpawn = value.ToArray(); }
    }

    public void SpawnEnemies(){
        this.StartSafeCoroutine(GenerateEnemies());
    }

    // Spawn function gets called but nothing happens
    IEnumerator GenerateEnemies(){
        Debug.Log("yeaaaahhhh");

        float spawnTime = .2f;
        enemiesLeftToSpawn = totalEnemyCount;

        doneSpawning = false;

        for (int i = 0; i < enemiesLeftToSpawn; i++){
            var validEnemyTypes = enemyTypeToCountMappings.Keys
                                                          .Where(x => enemyTypeToCountMappings[x] > 0)
                                                          .ToList();

            GameObject prefabType = validEnemyTypes[Random.Range(0, validEnemyTypes.Count - 1)];
            enemyTypeToCountMappings[prefabType]--;
            SpawnEnemy(prefabType);

            yield return new WaitForSeconds(spawnTime);
        }

        doneSpawning = true;
    }

    // For now default to spawning enemy on right side of screen
    void SpawnEnemy(GameObject enemyToSpawn){
        enemyController.SpawnEnemy(enemyToSpawn);
        enemiesLeftToSpawn--;
    }
}
