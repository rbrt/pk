using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerAttack : MonoBehaviour {

    [SerializeField] protected float damage;

    //[HideInInspector]
    [SerializeField] protected AttackTree attackTree;

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
