﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour {

    static PlayerController instance;
    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected AttackHandler attackHandler;
    [SerializeField] protected PlayerInputManager playerInputManager;

    protected bool moveLeft,
                   moveRight,
                   moveUp,
                   moveDown,
                   attacking,
                   damaged,
                   block,
                   dead;

    protected float hMoveSpeed = .03f,
                    vMoveSpeed = .02f,
                    punchDuration = .3f,
                    punchRange = .5f,
                    damageDuration = .2f,
                    blockDamageReduction = .1f,
                    maxYvalue = .69f,
                    minYValue = -1.25f,
                    maxXValue = 15,
                    minXValue = -3.3f,
                    currentMinX = 0,
                    currentMaxX = 0;

    protected int simultaneousEnemiesToAttack = 3;

    protected AnimateDude animateDude;

    protected AttackHandler.AttackInputType currentPlayerAttack;

    public static AttackHandler.AttackInputType CurrentPlayerAttack {
        set { instance.currentPlayerAttack = value; }
    }

    public static PlayerController Instance{
        get { return instance; }
    }

    bool EngagedInAction{
        get {return attacking || damaged || block;}
    }

    bool PlayerFacingRight{
        get { return transform.rotation.y != 0; }
    }

    void Awake(){
        minXValue = GameObject.Find("MinXBorder").transform.position.x;
        maxXValue = GameObject.Find("MaxXBorder").transform.position.x;
        currentMinX = minXValue;
        currentMaxX = maxXValue;

        instance = this;
    }

    void Start(){
        animateDude = GetComponentInChildren<AnimateDude>();
    }

	void Update () {
        if (dead){
            animateDude.Dead();
            return;
        }

        HandleInput();

        var pos = transform.position;

        if (!attacking){
            if (moveLeft && pos.x > currentMinX){
                transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
                if (!block){
                    pos.x -= hMoveSpeed;
                }
            }
            else if (moveRight && pos.x < currentMaxX){
                transform.rotation = Quaternion.Euler(Vector3.zero);
                if (!block){
                    pos.x += hMoveSpeed;
                }
            }

            if (moveUp && pos.y < maxYvalue){
                if (block){
                    pos.y += vMoveSpeed / 2;
                }
                else{
                    pos.y += vMoveSpeed;
                }
            }
            else if (moveDown && pos.y > minYValue){
                if (block){
                    pos.y -= vMoveSpeed / 2;
                }
                else{
                    pos.y -= vMoveSpeed;
                }
            }
        }

        if (!damaged){
            transform.position = pos;
        }
	}

    bool LeftInputDown{
        get { return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow); }
    }

    bool RightInputDown{
        get { return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow); }
    }

    bool UpInputDown{
        get { return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow); }
    }

    bool DownInputDown{
        get { return Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow); }
    }

    bool Attack1InputDown{
        get { return Input.GetKeyDown(KeyCode.Z); }
    }

    bool Attack2InputDown{
        get { return Input.GetKeyDown(KeyCode.X); }
    }

    bool LeftInputUp{
        get { return Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow); }
    }

    bool RightInputUp{
        get { return Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow); }
    }

    bool UpInputUp{
        get { return Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow); }
    }

    bool DownInputUp{
        get { return Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow); }
    }

    bool Attack1InputUp{
        get { return Input.GetKeyUp(KeyCode.Z); }
    }

    bool Attack2InputUp{
        get { return Input.GetKeyUp(KeyCode.X); }
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
            playerInputManager.SendInput(PlayerInputManager.InputTypes.Up);
        }
        // Left
        else if (LeftInputDown){
            moveLeft = true;
            moveRight = false;
            if (PlayerFacingRight){
                playerInputManager.SendInput(PlayerInputManager.InputTypes.Right);
            }
            else{
                playerInputManager.SendInput(PlayerInputManager.InputTypes.Left);
            }
        }
        // Down
        else if (DownInputDown){
            moveUp = false;
            moveDown = true;
            playerInputManager.SendInput(PlayerInputManager.InputTypes.Down);
        }
        // Right
        else if (RightInputDown){
            moveLeft = false;
            moveRight = true;
            if (PlayerFacingRight){
                playerInputManager.SendInput(PlayerInputManager.InputTypes.Left);
            }
            else{
                playerInputManager.SendInput(PlayerInputManager.InputTypes.Right);
            }
        }
        // Attack1
        else if (Attack1InputDown){
            /*if (!attacking){
                PlayerAttack attack = attackHandler.GetAttack(AttackHandler.AttackInputType.Attack1);
                if (attack != null){
                    this.StartSafeCoroutine(Attack(attack));
                }
            }*/
            playerInputManager.SendInput(PlayerInputManager.InputTypes.Attack1);
        }
        //Attack2
        else if (Attack2InputDown){
            /*if (!attacking){
                PlayerAttack attack = attackHandler.GetAttack(AttackHandler.AttackInputType.Attack2);
                if (attack != null){
                    this.StartSafeCoroutine(Attack(attack));
                }
            }*/
            playerInputManager.SendInput(PlayerInputManager.InputTypes.Attack2);
        }

        if (!attacking && !damaged && !block){
            if (currentPlayerAttack != AttackHandler.AttackInputType.None){
                var attack = attackHandler.GetAttack(currentPlayerAttack);
                if (attack != null){
                    this.StartSafeCoroutine(Attack(attack));
                }
                currentPlayerAttack = AttackHandler.AttackInputType.None;
            }
        }

        // Help a player block if they are getting stomped
        if (BlockInput){
            this.StartSafeCoroutine(Block());
            playerInputManager.SendInput(PlayerInputManager.InputTypes.Block);
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

    public void LockMinAndMaxX(float baseX){
        float lockOffset = 2.5f;
        currentMinX = baseX - lockOffset;
        currentMaxX = baseX + lockOffset;
    }

    public void UnlockMinAndMaxX(){
        currentMaxX = maxXValue;
        currentMinX = minXValue;
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

        Debug.DrawRay(transform.position - transform.right.normalized * .2f, (transform.right * attack.AttackRange), Color.green, 2f);

        var hits = Physics.RaycastAll(transform.position - transform.right * .2f, transform.right, attack.AttackRange).Where(x => ColliderIsEnemy(x.collider)).ToList();

        if (hits.Count() > 0){
            for (int i = 0; i < Mathf.Min(simultaneousEnemiesToAttack, hits.Count()); i++){
                hits[i].collider.GetComponent<Enemy>().TakeDamage(attack.Damage, attack.AttackEffects);
                var audioClip = attack.AttackSound;
                if (audioClip){
                    AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
                }
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

    public void Die(){
        dead = true;
        EnemyEncounter.CurrentEncounter.PlayerDied();
        animateDude.Dead();
    }
}
