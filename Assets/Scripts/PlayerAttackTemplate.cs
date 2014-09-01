using UnityEngine;
using System.Collections;

public class PlayerAttackTemplate : ScriptableObject {

    [SerializeField] protected float damage,
                                     baseAttackTime,    // How long before other actions are allowed
                                     attackRange,
                                     attackDuration;

    [SerializeField] protected Sprite[] attackSprites;

    public float Damage{
        get { return damage; }
        set { damage = value; }
    }

    public float BaseAttackTime{
        get { return baseAttackTime; }
        set { baseAttackTime = value; }
    }

    public float AttackRange{
        get { return attackRange; }
        set { attackRange = value; }
    }

    public float AttackDuration{
        get { return attackDuration; }
        set { attackDuration = value; }
    }

}
