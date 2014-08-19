using UnityEngine;
using System.Collections;

public class EnemySpawn : MonoBehaviour {

    [SerializeField] protected GameObject enemyPrefab;

    [SerializeField] protected int enemiesToSpawn,
                                   enemiesLeftToSpawn;

    [SerializeField] protected EnemyController enemyController;

    protected bool doneSpawning;

    public bool DoneSpawning {
        get { return doneSpawning; }
    }

    void Start(){
        enemiesLeftToSpawn = enemiesToSpawn;

        this.StartSafeCoroutine(GenerateEnemies());
    }

    IEnumerator GenerateEnemies(){
        float spawnTime = .2f;

        doneSpawning = false;
        
        for (int i = 0; i < enemiesToSpawn; i++){
            SpawnEnemy(enemyPrefab);

            yield return new WaitForSeconds(spawnTime);
        }

        doneSpawning = true;
    }

    // For now defualt to spawning enemie on right side of screen
    void SpawnEnemy(GameObject enemyToSpawn){
        enemyController.SpawnEnemy(enemyToSpawn);
        enemiesLeftToSpawn--;
    }

}
