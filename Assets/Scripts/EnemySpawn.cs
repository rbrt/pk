using UnityEngine;
using System.Collections;

public class EnemySpawn : MonoBehaviour {

    [SerializeField] protected GameObject enemyPrefab;

    [SerializeField] protected int enemiesToSpawn,
                                   enemiesLeftToSpawn;

    [SerializeField] protected EnemyController enemyController;

    void Start(){
        enemiesLeftToSpawn = enemiesToSpawn;

        this.StartSafeCoroutine(GenerateEnemies());
    }

    IEnumerator GenerateEnemies(){
        float spawnTime = .2f;

        for (int i = 0; i < enemiesToSpawn; i++){
            SpawnEnemy(enemyPrefab);

            yield return new WaitForSeconds(spawnTime);
        }
    }

    // For now defualt to spawning enemie on right side of screen
    void SpawnEnemy(GameObject enemyToSpawn){
        enemyController.SpawnEnemy(enemyToSpawn);
        enemiesLeftToSpawn--;
    }

}
