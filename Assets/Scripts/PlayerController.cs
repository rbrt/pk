using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour {

    protected bool moveLeft,
                   moveRight,
                   moveUp,
                   moveDown,
                   punch,
                   damaged;

    protected float hMoveSpeed = .03f,
                    vMoveSpeed = .02f,
                    punchDuration = .3f,
                    punchRange = .5f,
                    damageDuration = .2f;

    protected int simultaneousEnemiesToAttack = 3;

    protected AnimateDude animateDude;

    void Start(){
        animateDude = GetComponentInChildren<AnimateDude>();
    }

	void Update () {
        HandleInput();

        var pos = transform.position;

        if (!punch && !damaged){
            if (moveLeft){
                transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
                pos.x -= hMoveSpeed;
            }
            else if (moveRight){
                transform.rotation = Quaternion.Euler(Vector3.zero);
                pos.x += hMoveSpeed;
            }

            if (moveUp){
                pos.y += vMoveSpeed;
            }
            else if (moveDown){
                pos.y -= vMoveSpeed;
            }
        }

        transform.position = pos;

	}

    void HandleInput(){
        // Up
        if (Input.GetKeyDown(KeyCode.W)){
            moveUp = true;
            moveDown = false;
        }
        // Left
        else if (Input.GetKeyDown(KeyCode.A)){
            moveLeft = true;
            moveRight = false;
        }
        // Down
        else if (Input.GetKeyDown(KeyCode.S)){
            moveUp = false;
            moveDown = true;
        }
        // Right
        else if (Input.GetKeyDown(KeyCode.D)){
            moveLeft = false;
            moveRight = true;
        }
        // Punch
        else if (Input.GetKeyDown(KeyCode.Space)){
            if (!punch){
                this.StartSafeCoroutine(Punch());
            }
        }

        // Up
        if (Input.GetKeyUp(KeyCode.W)){
            moveUp = false;
        }
        // Left
        else if (Input.GetKeyUp(KeyCode.A)){
            moveLeft = false;
        }
        // Down
        else if (Input.GetKeyUp(KeyCode.S)){
            moveDown = false;
        }
        // Right
        else if (Input.GetKeyUp(KeyCode.D)){
            moveRight = false;
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

    IEnumerator Punch(){
        punch = true;
        animateDude.Punch();

        Debug.DrawRay(transform.position, (transform.right * punchRange), Color.green, 2f);

        var hits = Physics.RaycastAll(transform.position, transform.right, punchRange).Where(x => ColliderIsEnemy(x.collider)).ToList();

        if (hits.Count() > 0){
            for (int i = 0; i < Mathf.Min(simultaneousEnemiesToAttack, hits.Count()); i++){
                hits[i].collider.GetComponent<Enemy>().TakeDamage();
            }
        }

        yield return new WaitForSeconds(punchDuration);

        animateDude.Idle();
        punch = false;
    }

    IEnumerator Damaged(){
        damaged = true;
        animateDude.Damage();

        yield return new WaitForSeconds(damageDuration);

        animateDude.Idle();
        damaged = false;
    }

    public void TakeDamage(){
        this.StartSafeCoroutine(Damaged());
    }
}
