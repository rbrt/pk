using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour {

    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected AttackTree attackTree;

    protected bool moveLeft,
                   moveRight,
                   moveUp,
                   moveDown,
                   attacking,
                   damaged,
                   block;

    protected float hMoveSpeed = .03f,
                    vMoveSpeed = .02f,
                    punchDuration = .3f,
                    punchRange = .5f,
                    damageDuration = .2f,
                    blockDamageReduction = .2f,
                    maxYvalue = .69f,
                    minYValue = -1.25f,
                    maxXValue = 15,
                    minXValue = -3.3f;

    protected int simultaneousEnemiesToAttack = 3;

    protected AnimateDude animateDude;

    bool EngagedInAction{
        get {return attacking || damaged || block;}
    }

    void Start(){
        animateDude = GetComponentInChildren<AnimateDude>();
    }

	void Update () {
        HandleInput();

        var pos = transform.position;

        if (!attacking && !damaged){
            if (moveLeft && pos.x > minXValue){
                transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
                pos.x -= hMoveSpeed;
            }
            else if (moveRight && pos.x < maxXValue){
                transform.rotation = Quaternion.Euler(Vector3.zero);
                pos.x += hMoveSpeed;
            }

            if (moveUp && pos.y < maxYvalue){
                pos.y += vMoveSpeed;
            }
            else if (moveDown && pos.y > minYValue){
                pos.y -= vMoveSpeed;
            }
        }

        // Only change direction when blocking but stay in place
        if (!block){
            transform.position = pos;
        }

	}

    bool LeftInputDown{
        get { return Input.GetKeyDown(KeyCode.A); }
    }

    bool RightInputDown{
        get { return Input.GetKeyDown(KeyCode.D); }
    }

    bool UpInputDown{
        get { return Input.GetKeyDown(KeyCode.W); }
    }

    bool DownInputDown{
        get { return Input.GetKeyDown(KeyCode.S); }
    }

    bool Attack1InputDown{
        get { return Input.GetKeyDown(KeyCode.Space); }
    }

    bool LeftInputUp{
        get { return Input.GetKeyUp(KeyCode.A); }
    }

    bool RightInputUp{
        get { return Input.GetKeyUp(KeyCode.D); }
    }

    bool UpInputUp{
        get { return Input.GetKeyUp(KeyCode.W); }
    }

    bool DownInputUp{
        get { return Input.GetKeyUp(KeyCode.S); }
    }

    bool Attack1InputUp{
        get { return Input.GetKeyUp(KeyCode.Space); }
    }

    bool BlockInput {
        get{ return Input.GetKey(KeyCode.LeftShift) && !EngagedInAction; }
    }

    bool BlockInputUp {
        get{ return Input.GetKeyUp(KeyCode.LeftShift); }
    }

    void HandleInput(){
        // Up
        if (UpInputDown){
            moveUp = true;
            moveDown = false;
        }
        // Left
        else if (LeftInputDown){
            moveLeft = true;
            moveRight = false;
        }
        // Down
        else if (DownInputDown){
            moveUp = false;
            moveDown = true;
        }
        // Right
        else if (RightInputDown){
            moveLeft = false;
            moveRight = true;
        }
        // Punch
        else if (Attack1InputDown){
            if (!attacking){
                PlayerAttack attack = attackTree.GetAttack(AttackTree.AttackInputType.Attack1);
                if (attack != null){
                    this.StartSafeCoroutine(Attack(attack));
                }
            }
        }


        // Help a player block if they are getting stomped
        if (BlockInput){
            this.StartSafeCoroutine(Block());
        }

        // Up
        if (UpInputUp){
            moveUp = false;
        }
        // Left
        else if (LeftInputUp){
            moveLeft = false;
        }
        // Down
        else if (DownInputUp){
            moveDown = false;
        }
        // Right
        else if (RightInputUp){
            moveRight = false;
        }
        else if (BlockInputUp){
            block = false;
        }
    }

    List<Enemy.EnemyType> GetEnemyTypes(){
        return System.Enum.GetValues(typeof(Enemy.EnemyType)).Cast<Enemy.EnemyType>().ToList();
    }

    bool ColliderIsEnemy(Collider collider){
        return  GetEnemyTypes().Any(x => {
                    Enemy enemy = collider.GetComponent<Enemy>();
                    return enemy != null && enemy.TypeOfEnemy == x;
                });
    }

    IEnumerator Block(){
        block = true;
        animateDude.Block();

        while (block){
            yield return null;
        }

        animateDude.Idle();
    }

    IEnumerator Attack(PlayerAttack attack){
        attacking = true;
        animateDude.Attack(attack.AttackSprites);

        Debug.DrawRay(transform.position, (transform.right * attack.AttackRange), Color.green, 2f);

        var hits = Physics.RaycastAll(transform.position, transform.right, attack.AttackRange).Where(x => ColliderIsEnemy(x.collider)).ToList();

        if (hits.Count() > 0){
            for (int i = 0; i < Mathf.Min(simultaneousEnemiesToAttack, hits.Count()); i++){
                hits[i].collider.GetComponent<Enemy>().TakeDamage(attack.Damage, attack.AttackEffects);
            }
        }

        yield return new WaitForSeconds(attack.AttackDuration);

        animateDude.Idle();
        attacking = false;
    }

    IEnumerator Damaged(){
        damaged = true;
        animateDude.Damage();

        yield return new WaitForSeconds(damageDuration);

        animateDude.Idle();
        damaged = false;
    }

    public void TakeDamage(float damage){
        if (!block){
            this.StartSafeCoroutine(Damaged());
        }
        else{
            damage *= blockDamageReduction;
        }

        healthBar.DecrementHealth(damage);
    }
}
