using UnityEngine;
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
    protected int totalEnemyCount;

    // Dictionaries don't serialize....
    protected Dictionary<GameObject, int> enemyTypeToCountMappings;

    void Awake(){
        enemyTypeToCountMappings = new Dictionary<GameObject, int>();
        totalEnemyCount = 0;

        for (int i = 0; i < enemyPrefabs.Length; i++){
            enemyTypeToCountMappings[enemyPrefabs[i]] = enemiesToSpawn[i];
            totalEnemyCount += enemiesToSpawn[i];
        }
    }

    public EnemyController EnemyControllerAccess{
        get { return enemyController; }
        set { enemyController = value; }
    }

    public bool DoneSpawning {
        get { return doneSpawning; }
    }

    public int EnemyTypesCount{
        get { return enemyTypesCount; }
        set { enemyTypesCount = value; }
    }

    public List<GameObject> EnemyPrefabs{
        get {
            if (enemyPrefabs == null){
                enemyPrefabs = new GameObject[0];
            }
            return enemyPrefabs.ToList();
        }
        set { enemyPrefabs = value.ToArray(); }
    }

    public List<int> EnemiesToSpawn{
        get {
            if (enemiesToSpawn == null){
                enemiesToSpawn = new int[0];
            }
            return enemiesToSpawn.ToList();
        }
        set { enemiesToSpawn = value.ToArray(); }
    }

    public void SpawnEnemies(){
        this.StartSafeCoroutine(GenerateEnemies());
    }

    IEnumerator GenerateEnemies(){
        float spawnTime = .2f;
        int enemiesLeftToSpawn = totalEnemyCount;

        doneSpawning = false;

        for (int i = 0; i < enemiesLeftToSpawn; i++){
            var validEnemyTypes = enemyTypeToCountMappings.Keys
                                                          .Where(x => enemyTypeToCountMappings[x] > 0)
                                                          .ToList();

            GameObject prefabType = validEnemyTypes[Random.Range(0, validEnemyTypes.Count - 1)];
            enemyTypeToCountMappings[prefabType]--;

            enemyController.SpawnEnemy(prefabType);

            yield return new WaitForSeconds(spawnTime);
        }

        doneSpawning = true;
    }

}
