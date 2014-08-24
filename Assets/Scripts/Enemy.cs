using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    protected PlayerController player;

    protected enum EnemyStates {Moving, Attacking, Damaged, Dead, Idle};
    public enum EnemyType {GreenHairPunk, Skinhead};
    protected EnemyStates enemyState;

    [SerializeField] protected EnemyType enemyType;
    [SerializeField] protected float hMoveSpeed = .03f,
                                     vMoveSpeed = .02f,
                                     punchDuration = .2f,
                                     punchRange = .5f,
                                     afterPunchDelay = .3f,
                                     damageDuration = .2f;

    protected AnimateDude animateDude;
    protected int health = 5;
    protected bool punching = false;
    protected SafeCoroutine behaviourCoroutine;
    protected FightSequence fightSequence;
    protected bool fightingPlayer;
    protected float destinationThreshold = .2f;

    [SerializeField] protected Vector3 destinationPosition;

    public void SetFightSequence(FightSequence sequence){
        fightSequence = sequence;
    }

    public EnemyType TypeOfEnemy{
        get { return enemyType; }
    }

    public Vector3 Destination{
        get { return destinationPosition; }
        set { destinationPosition = value; }
    }

    public bool IsDead{
        get { return enemyState == EnemyStates.Dead; }
    }

	void Start () {
        player = GameObject.Find("PlayerCharacter").GetComponent<PlayerController>();
        enemyState = EnemyStates.Idle;
        animateDude = GetComponentInChildren<AnimateDude>();

        behaviourCoroutine = this.StartSafeCoroutine(Primer());
	}

	void Update () {
        if (enemyState != EnemyStates.Dead){

            var pos = transform.localPosition;

            if (health <= 0 && enemyState != EnemyStates.Dead){
                enemyState = EnemyStates.Dead;
                if (behaviourCoroutine.IsPaused || behaviourCoroutine.IsRunning){
                    behaviourCoroutine.Stop();
                }

                this.StartSafeCoroutine(Dead());
            }

            if (enemyState == EnemyStates.Moving){
                // Check if close enough
                if (InRangeForAttack()){
                    enemyState = EnemyStates.Attacking;
                }
                else{
                    // move down
                    if (player.transform.position.y < pos.y){
                        pos.y -= vMoveSpeed;
                    }
                    // move up
                    else if (player.transform.position.y > pos.y){
                        pos.y += vMoveSpeed;
                    }

                    // move left
                    if (player.transform.position.x < pos.x){
                        pos.x -= hMoveSpeed;
                    }
                    // move right
                    else if (player.transform.position.x > pos.x){
                        pos.x += hMoveSpeed;
                    }
                }
            }
            else if (enemyState == EnemyStates.Attacking){
                if (InRangeForAttack() && !punching){
                    behaviourCoroutine = this.StartSafeCoroutine(Punch());
                }
                else{
                    enemyState = EnemyStates.Moving;
                }

            }
            else if (enemyState == EnemyStates.Damaged){

            }
            else if (enemyState == EnemyStates.Idle){
                if (!InRangeOfIdlePosition()){
                    pos = Vector3.MoveTowards(transform.position, destinationPosition, .025f);
                }
                else{
                    // Be idle
                }
            }
            else if (enemyState == EnemyStates.Dead){

            }

            if (enemyState != EnemyStates.Dead){
                transform.rotation = player.transform.position.x < transform.position.x ?
                                     Quaternion.Euler(new Vector3(0, 180, 0)) :
                                     Quaternion.Euler(Vector3.zero);
            }

            transform.localPosition = pos;
        }
	}

    bool InRangeOfIdlePosition(){
        return Vector3.Distance(transform.position, destinationPosition) < destinationThreshold;
    }

    bool InRangeForAttack(){
        return Vector3.Distance(transform.position, player.transform.position) < punchRange;
    }

    IEnumerator Primer(){
        yield break;
    }

    IEnumerator Punch(){
        punching = true;
        animateDude.Punch();
        player.TakeDamage();

        yield return new WaitForSeconds(punchDuration);

        if (health > 0){
            animateDude.Idle();
            yield return new WaitForSeconds(afterPunchDelay);
        }
        punching = false;
    }

    IEnumerator Damaged(){
        var previousState = enemyState;
        enemyState = EnemyStates.Damaged;
        animateDude.Damage();

        health--;

        yield return new WaitForSeconds(damageDuration);

        if (health > 0){
            animateDude.Idle();
            enemyState = previousState;
        }
    }

    IEnumerator Dead(){
        animateDude.Dead();

        if (fightSequence){
            fightSequence.HandleEnemyDeath(gameObject);
        }
        else{
            Debug.Log(gameObject.name + " has no associated FightSequence", gameObject);
        }
        yield break;
    }

    public void AttackPlayer(){
        enemyState = EnemyStates.Moving;
    }

    public void TakeDamage(){
        if (enemyState != EnemyStates.Dead){
            behaviourCoroutine = this.StartSafeCoroutine(Damaged());
        }
    }
}
