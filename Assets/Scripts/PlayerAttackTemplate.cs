using UnityEngine;
using System.Collections;

public class PlayerAttackTemplate : ScriptableObject {

    [SerializeField] protected float damage,
                                     baseAttackTime,    // How long before other actions are allowed
                                     attackRange,
                                     attackDuration;

    [SerializeField] protected Sprite[] attackSprites;

    public void SetWithTemplate(ref PlayerAttack playerAttack){
        playerAttack.Damage = damage;
        playerAttack.BaseAttackTime = baseAttackTime;
        playerAttack.AttackRange = attackRange;
        playerAttack.AttackDuration = attackDuration;
        playerAttack.AttackSprites = attackSprites;
    }

}
