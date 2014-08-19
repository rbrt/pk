using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FightSequence : MonoBehaviour {

    [SerializeField] protected Transform[] fightPositions;
    [SerializeField] protected EnemyController enemyController;
    [SerializeField] protected EnemySpawn enemySpawn;

    protected Transform playerTransform;
    protected FightingEnemy[] fightingEnemies;
    protected Dictionary<Transform, bool> occupiedPositions;

	void Start () {
        var player = GameObject.Find("PlayerCharacter");
        if (player == null){
            Debug.LogError("No PlayerCharacter found in current scene!");
        }
        else{
            playerTransform = player.transform;
        }

        fightingEnemies = new FightingEnemy[fightPositions.Length];
        occupiedPositions = new Dictionary<Transform, bool>();
        fightPositions.ToList().ForEach(x => occupiedPositions[x] = false);
	}

	void Update () {
        BringIdleEnemiesInToTheFight();
	}

    void BringIdleEnemiesInToTheFight(){
        if (enemySpawn.DoneSpawning && enemyController.GetIdleEnemy(GetFightingEnemies()) != null){
            while (fightingEnemies.Any(x => x == null)){
                var enemy = enemyController.GetIdleEnemy(GetFightingEnemies());
                var targetPosition = occupiedPositions.Keys.ToList().First(x => occupiedPositions[x] == false);

                int index = fightingEnemies.ToList().IndexOf(fightingEnemies.First(x => x == null));
                fightingEnemies[index] = new FightingEnemy(playerTransform, ref targetPosition, enemy);
                occupiedPositions[targetPosition] = true;

                enemy.GetComponent<Enemy>().Destination = targetPosition.position;
            }
        }
    }

    List<GameObject> GetFightingEnemies(){
        return (from enemy in fightingEnemies where enemy != null select enemy.Enemy).ToList();
    }

    public void HandleEnemyDeath(GameObject enemyThatDied){
        int index = fightingEnemies.ToList().IndexOf(fightingEnemies.First(x => x.Enemy == enemyThatDied));
        var transform = fightingEnemies[index].TargetPosition;
        occupiedPositions[transform] = false;
        fightingEnemies[index] = null;
    }
}

// Encapsulate enemy positioning and behaviours
public class FightingEnemy{
    Transform playerTransform;
    Transform targetPosition;
    GameObject enemy;
    bool assigned = false;

    SafeCoroutine movementCoroutine,
                  actionCoroutine;

    public bool Assigned {
        get { return assigned; }
    }

    public GameObject Enemy{
        get { return enemy; }
    }

    public Transform TargetPosition{
        get { return targetPosition; }
    }

    public SafeCoroutine MovementCoroutine{
        get { return movementCoroutine; }
        set { movementCoroutine = value; }
    }

    public SafeCoroutine ActionCoroutine{
        get { return actionCoroutine; }
        set { actionCoroutine = value; }
    }

    public FightingEnemy(Transform player, ref Transform offset, GameObject enemy){
        playerTransform = player;
        targetPosition = offset;
        this.enemy = enemy;
        assigned = true;
    }

}
