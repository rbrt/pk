using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackTree : MonoBehaviour {

    protected PlayerAttack currentAttack;

    [SerializeField] protected PlayerAttack attack1,
                                            attack2;

    public enum AttackInputType {Attack1, Attack2};

    Dictionary<AttackInputType, PlayerAttack> baseAttacks;

    void Awake(){
        baseAttacks = new Dictionary<AttackInputType, PlayerAttack>
                            {
                                { AttackInputType.Attack1, attack1 },
                                { AttackInputType.Attack2, attack2 }
                            };
    }


	public PlayerAttack Attack1(){
        return GetAttack(AttackInputType.Attack1);
    }

    public PlayerAttack Attack2(){
        return GetAttack(AttackInputType.Attack2);
    }

    public PlayerAttack GetAttack(AttackInputType attackInputType){
        // Attack in progress already
        if (currentAttack != null){
            //  Has it been long enough for another attack
            if(currentAttack.PassedBaseAttackTime()){
                var attack = currentAttack.GetNextAttack(attackInputType);

                // No move next on the tree, start from base corresponding to button pressed
                if (attack == null){
                    currentAttack = baseAttacks[attackInputType];
                }
                // Advance along the tree
                else{
                    currentAttack = attack;
                }

                Debug.Log(currentAttack.name);
                currentAttack.StartTime = Time.time;

            }
            else {
                Debug.Log("No valid attack");
                currentAttack = null;
            }
        }
        // Base attack on tree
        else{
            currentAttack = baseAttacks[attackInputType];
        }

        return currentAttack;
    }
}
