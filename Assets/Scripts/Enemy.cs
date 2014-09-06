using UnityEngine;
using System.Collections;
using System.Linq;

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
    protected float health = 500;
    [SerializeField] protected bool punching = false;
    protected SafeCoroutine behaviourCoroutine;
    protected FightSequence fightSequence;
    protected bool fightingPlayer;
    protected bool moving = false;
    protected bool idle = false;
    protected float destinationThreshold = .05f;
    protected float attackDelayStartTime = 0;

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
                return;
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
            yield return this.StartSafeCoroutine(
                                WaitAfterAttack(afterAttackDelay + Random.Range(-afterAttackDelayOffsetRange, afterAttackDelayOffsetRange))
                              );
        }
        punching = false;
    }


    IEnumerator WaitAfterAttack(float delay){
        attackDelayStartTime = Time.time;

        while (Time.time - attackDelayStartTime < delay){
            yield return null;
        }
    }

    IEnumerator Move(){
        animateEnemy.Move();

        yield break;
    }

    IEnumerator Damaged(float damage, AttackEffect[] attackEffects){
        var previousState = enemyState;
        enemyState = EnemyStates.Damaged;
        animateEnemy.Damage();

        attackDelayStartTime = Time.time;

        health -= damage;

        yield return new WaitForSeconds(damageDuration);

        if (attackEffects.Length > 0){
            yield return this.StartSafeCoroutine(WaitForAttackEffects(attackEffects));
        }

        if (health > 0){
            animateEnemy.Inactive();
            enemyState = previousState;
        }
    }

    IEnumerator WaitForAttackEffects(AttackEffect[] attackEffects){
        SafeCoroutine[] coroutines = new SafeCoroutine[attackEffects.Length];
        for (int i = 0; i < attackEffects.Length; i++){
            coroutines[i] = attackEffects[i].InvokeEffect(gameObject);
        }

        while (coroutines.Any(x => x.IsRunning)){
            yield return null;
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

    public void TakeDamage(float damage, AttackEffect[] attackEffects){
        if (enemyState != EnemyStates.Dead){
            behaviourCoroutine = this.StartSafeCoroutine(Damaged(damage, attackEffects));
        }
    }
}
