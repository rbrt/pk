using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerAttack : MonoBehaviour {

    [SerializeField] protected float damage,
                                     baseAttackTime,    // How long before other actions are allowed
                                     attackRange,
                                     attackDuration;    // How long the attack should take

    [SerializeField] protected AttackTree attackTree;

    [SerializeField] protected Sprite[] attackSprites;

    [SerializeField] protected AttackEffect[] attackEffects;

    protected float startTime;

    [HideInInspector]
    [SerializeField] protected NextMove[] attack1Moves,
                                          attack2Moves;

    public AttackTree AccessAttackTree{
        get { return attackTree; }
        set { attackTree = value; }
    }

    public NextMove[] Attack1Moves{
        get { return attack1Moves; }
    }

    public NextMove[] Attack2Moves{
        get { return attack2Moves; }
    }

    public NextMove[] AllMoves{
        get {
            List<NextMove> allMoves = new List<NextMove>();
            allMoves.AddRange(attack1Moves.ToList());
            allMoves.AddRange(attack2Moves.ToList());

            return allMoves.ToArray();
        }
    }

    public AttackEffect[] AttackEffects{
        get { return attackEffects; }
    }

    public float AttackRange{
        get { return attackRange; }
        set { attackRange = value; }
    }

    public Sprite[] AttackSprites{
        get { return attackSprites; }
        set { attackSprites = value; }
    }

    public float StartTime{
        get { return startTime; }
        set { startTime = value; }
    }

    public float AttackDuration{
        get { return attackDuration; }
        set { attackDuration = value; }
    }

    public float BaseAttackTime {
        get { return baseAttackTime; }
        set { baseAttackTime = value; }
    }

    public float Damage {
        get { return damage; }
        set { damage = value; }
    }

    public bool Clean(){
        int attack1Count = attack1Moves.Length;
        int attack2Count = attack2Moves.Length;

        attack1Moves = attack1Moves.Where(x => x.AccessNextMove != null).ToArray();
        attack2Moves = attack2Moves.Where(x => x.AccessNextMove != null).ToArray();

        return attack1Count != attack1Moves.Length ||  attack2Count != attack2Moves.Length;
    }

    public void AddAttack1Move(PlayerAttack attack){
        attack.name = "New Attack1Move";
        var moves = attack1Moves.ToList();
        moves.Add(new NextMove(attack));
        attack1Moves = moves.ToArray();
    }

    public void AddAttack2Move(PlayerAttack attack){
        attack.name = "New Attack2Move";
        var moves = attack2Moves.ToList();
        moves.Add(new NextMove(attack));
        attack2Moves = moves.ToArray();
    }

    public bool PassedBaseAttackTime(){
        float elapsedTime = Time.time - startTime;
        return elapsedTime > baseAttackTime;
    }

    public PlayerAttack GetNextAttack(AttackTree.AttackInputType attackInputType){
        NextMove[] attackMoves;

        if (attackInputType == AttackTree.AttackInputType.Attack1){
            attackMoves = attack1Moves;
        }
        else if (attackInputType == AttackTree.AttackInputType.Attack2){
            attackMoves = attack2Moves;
        }

        float elapsedTime = Time.time - startTime;
        var attacks = attackMoves.Where(x => elapsedTime < x.CutoffTime).OrderBy(x => x.CutoffTime).ToList();
        if (attacks.Count > 0){
            return attacks[0].AccessNextMove;
        }

        return null;
    }

    public PlayerAttack(){
        Init();
    }

    public void Init(){
        attack1Moves = new NextMove[0];
        attack2Moves = new NextMove[0];
        startTime = 0;
        damage = 0;
    }
}

[System.Serializable]
public class NextMove {
    [SerializeField] protected PlayerAttack nextMove;
    [SerializeField] protected float cutoffTime;

    public NextMove(PlayerAttack attack){
        nextMove = attack;
        cutoffTime = 0;
    }

    public PlayerAttack AccessNextMove{
        get { return nextMove; }
        set { nextMove = value; }
    }

    public float CutoffTime{
        get { return cutoffTime; }
        set { cutoffTime = value; }
    }
}
