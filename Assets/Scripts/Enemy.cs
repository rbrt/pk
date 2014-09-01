using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    protected PlayerController player;

    protected enum EnemyStates {Moving, Attacking, Damaged, Dead, Idle};
    public enum EnemyType {GreenHairPunk, Skinhead};

    [SerializeField] protected EnemyStates enemyState;

    [SerializeField] protected EnemyType enemyType;
    [SerializeField] protected float hMoveSpeed = .03f,
                                     vMoveSpeed = .02f,
                                     punchDuration = .2f,
                                     punchRange = .6f,
                                     afterAttackDelay = 1.5f,
                                     afterAttackDelayOffsetRange = .5f,
                                     damageDuration = .2f,
                                     generalMoveSpeed = .1f,
                                     attackDamage = 2;

    protected AnimateEnemy animateEnemy;
    protected float health = 5;
    [SerializeField] protected bool punching = false;
    protected SafeCoroutine behaviourCoroutine;
    protected FightSequence fightSequence;
    protected bool fightingPlayer;
    protected bool moving = false;
    protected bool idle = false;
    protected float destinationThreshold = .05f;

    [SerializeField] protected Vector3 destinationPosition;

    [SerializeField] protected bool inDestinationRange,
                                    inAttackRange;

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
        animateEnemy = GetComponentInChildren<AnimateEnemy>();

        behaviourCoroutine = this.StartSafeCoroutine(Primer());
	}

    void CancelBehaviourCoroutine(){
        if (behaviourCoroutine.IsPaused || behaviourCoroutine.IsRunning){
            behaviourCoroutine.Stop();
        }

        moving = false;
        idle = false;
        punching = false;
    }

	void Update () {
        inDestinationRange = InRangeOfDestinationPosition();
        inAttackRange = InRangeForAttack();

        if (enemyState != EnemyStates.Dead){

            var pos = transform.localPosition;

            if (health <= 0 && enemyState != EnemyStates.Dead){
                enemyState = EnemyStates.Dead;

                CancelBehaviourCoroutine();
                this.StartSafeCoroutine(Dead());
            }

            if (enemyState == EnemyStates.Moving){
                // Check if close enough
                if (InRangeOfDestinationPosition() && InRangeForAttack()){
                    CancelBehaviourCoroutine();
                    enemyState = EnemyStates.Attacking;
                }
                else{
                    if (!moving){
                        CancelBehaviourCoroutine();
                        moving = true;
                        behaviourCoroutine = this.StartSafeCoroutine(Move());
                    }
                    // Move towards destination

                    if (destinationPosition.y <= pos.y){
                        pos.y -= vMoveSpeed;
                    }
                    else if (destinationPosition.y > pos.y){
                        pos.y += vMoveSpeed;
                    }

                    if (destinationPosition.x <= pos.x){
                        pos.x -= hMoveSpeed;
                    }
                    else if (destinationPosition.x > pos.x){
                        pos.x += hMoveSpeed;
                    }
                }
            }
            else if (enemyState == EnemyStates.Attacking){
                if (InRangeForAttack()){
                    if (!punching){
                        CancelBehaviourCoroutine();
                        punching = true;
                        behaviourCoroutine = this.StartSafeCoroutine(Punch());
                    }
                }
                else{
                    enemyState = EnemyStates.Moving;
                }

            }
            else if (enemyState == EnemyStates.Damaged){

            }
            else if (enemyState == EnemyStates.Idle){
                if (!InRangeOfDestinationPosition()){
                    pos = Vector3.MoveTowards(transform.position, destinationPosition, .025f);
                }
                else{
                    if (!idle){
                        CancelBehaviourCoroutine();
                        idle = true;
                        behaviourCoroutine = this.StartSafeCoroutine(Idle());
                    }
                }
            }
            else if (enemyState == EnemyStates.Dead){

            }

            if (enemyState != EnemyStates.Dead){
                transform.rotation = player.transform.position.x < transform.position.x ?
                                     Quaternion.Euler(new Vector3(0, 180, 0)) :
                                     Quaternion.Euler(Vector3.zero);
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, pos, Time.deltaTime * generalMoveSpeed);
        }
	}

    bool InRangeOfDestinationPosition(){
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
        animateEnemy.Punch();
        player.TakeDamage(attackDamage);

        yield return new WaitForSeconds(punchDuration);

        if (health > 0){
            animateEnemy.Inactive();
            yield return new WaitForSeconds(afterAttackDelay + Random.Range(-afterAttackDelayOffsetRange, afterAttackDelayOffsetRange));
        }
        punching = false;
    }

    IEnumerator Move(){
        animateEnemy.Move();

        yield break;
    }

    IEnumerator Damaged(float damage){
        var previousState = enemyState;
        enemyState = EnemyStates.Damaged;
        animateEnemy.Damage();

        health -= damage;

        yield return new WaitForSeconds(damageDuration);

        if (health > 0){
            animateEnemy.Inactive();
            enemyState = previousState;
        }
    }

    IEnumerator Idle(){
        animateEnemy.Idle();

        yield break;
    }

    IEnumerator Dead(){
        animateEnemy.Dead();

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

    public void TakeDamage(float damage){
        if (enemyState != EnemyStates.Dead){
            behaviourCoroutine = this.StartSafeCoroutine(Damaged(damage));
        }
    }
}
