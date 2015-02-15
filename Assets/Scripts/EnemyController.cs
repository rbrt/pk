using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour {

    // Idle range: x = (-2, 2) y = (-.5, .5)
    [SerializeField] protected GameObject player;
    [SerializeField] protected List<GameObject> enemyList;
    [SerializeField] protected FightSequence fightSequence;
    [SerializeField] protected Transform leftSpawnLocation,
                                         rightSpawnLocation;


    protected int enemyCount = 0;

    protected float idleXCoord = 1.9f,
                    maxIdleYCoord = .69f,
                    minIdleYCoord = -1.25f;

    public List<GameObject> EnemyList {
        get { return enemyList; }
    }

    public GameObject GetIdleEnemy(List<GameObject> currentEnemies){
        var possibleEnemies = enemyList.Where(x =>
            !currentEnemies.Contains(x) &&
            !x.GetComponent<Enemy>().IsDead
        ).ToList();

        if (possibleEnemies.Count > 0){
            return possibleEnemies[Random.Range(0, possibleEnemies.Count-1)];
        }

        return null;
    }

    Vector3 FindIdlePosition(){
        var position = player.transform.position;
        position.x += (Random.Range((idleXCoord * .8f), idleXCoord)) * (Random.Range(0,100) > 50 ? -1 : 1);
        position.y += (Random.Range(minIdleYCoord, maxIdleYCoord));
        return position;
    }

    public void SetFightSequence(FightSequence fightSequence){
        this.fightSequence = fightSequence;
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

        position.y += (Random.Range(-50, 50) / 100f);

        var enemy = GameObject.Instantiate(enemyToInstantiate, position, Quaternion.Euler(Vector3.zero)) as GameObject;
        enemy.name += enemyCount++;
        enemy.GetComponent<Enemy>().SetFightSequence(fightSequence);
        enemy.GetComponent<Enemy>().Destination = FindIdlePosition();
        enemyList.Add(enemy);
    }
}
