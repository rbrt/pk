using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AttackHandler : MonoBehaviour {

    protected PlayerAttack currentAttack;

    [SerializeField] protected PlayerAttack attack1,
                                            attack2,
                                            downToForwardA1,
                                            downToForwardA2,
                                            downToBackA1,
                                            downToBackA2,
                                            forwardForwardA1,
                                            forwardForwardA2,
                                            backBackA1,
                                            backBackA2;

    [SerializeField] protected HandlePlayerInputFlash handleInputFlash;
    [SerializeField] protected bool logMessages;

    SafeCoroutine behaviourCoroutine;

    public enum AttackInputType {None,
                                 Attack1,
                                 Attack2,
                                 DownToForwardA1,
                                 DownToForwardA2,
                                 DownToBackA1,
                                 DownToBackA2,
                                 ForwardForwardA1,
                                 ForwardForwardA2,
                                 BackBackA1,
                                 BackBackA2
                                 };

    Dictionary<AttackInputType, PlayerAttack> baseAttacks,
                                              attackDictionary;

    IEnumerator Primer(){
        yield break;
    }

    void Awake(){
        baseAttacks = new Dictionary<AttackInputType, PlayerAttack>
                            {
                                { AttackInputType.Attack1, attack1 },
                                { AttackInputType.Attack2, attack2 }
                            };

        behaviourCoroutine = this.StartSafeCoroutine(Primer());

        if (handleInputFlash == null){
            handleInputFlash = GameObject.Find("PlayerCharacter").GetComponent<HandlePlayerInputFlash>();
        }

        InitializeAttackDictionary();
    }

    void InitializeAttackDictionary(){
        attackDictionary = new Dictionary<AttackInputType, PlayerAttack>();

        attackDictionary[AttackInputType.DownToForwardA1] = downToForwardA1;
        attackDictionary[AttackInputType.DownToForwardA2] = downToForwardA2;
        attackDictionary[AttackInputType.DownToBackA1] = downToBackA1;
        attackDictionary[AttackInputType.DownToBackA2] = downToBackA2;
        attackDictionary[AttackInputType.ForwardForwardA1] = forwardForwardA1;
        attackDictionary[AttackInputType.ForwardForwardA2] = forwardForwardA2;
        attackDictionary[AttackInputType.BackBackA1] = backBackA1;
        attackDictionary[AttackInputType.BackBackA2] = backBackA2;
    }

	public PlayerAttack Attack1(){
        return GetAttack(AttackInputType.Attack1);
    }

    public PlayerAttack Attack2(){
        return GetAttack(AttackInputType.Attack2);
    }

    public PlayerAttack GetAttack(AttackInputType attackInputType){
        PlayerAttack oldAttack = currentAttack;

        // Attack in progress already
        if (currentAttack != null){
            //  Has it been long enough for another attack
            if(currentAttack.PassedBaseAttackTime()){
                var attack = GetAttackFromDictionary(attackInputType);

                // No move next on the tree, start from base corresponding to button pressed
                if (attack == null){
                    if (baseAttacks.Keys.Contains(attackInputType)){
                        currentAttack = baseAttacks[attackInputType];
                    }
                }
                // Advance along the tree
                else{
                    currentAttack = attack;
                }

                if (logMessages){
                    Debug.Log(currentAttack.name);
                }
            }
            else {
                if (logMessages){
                    Debug.Log("No valid attack");
            }
                currentAttack = null;
            }
        }
        // Base attack on tree
        else{
            if (baseAttacks.Keys.Contains(attackInputType)){
                currentAttack = baseAttacks[attackInputType];
            }
        }

        if (currentAttack != null){
            currentAttack.StartTime = Time.time;
        }

        if ((oldAttack == attack1 || oldAttack == attack2) || (oldAttack != currentAttack && currentAttack != null)){
            if (behaviourCoroutine.IsPaused || behaviourCoroutine.IsRunning){
                behaviourCoroutine.Stop();
            }
        }

        return currentAttack;
    }

    PlayerAttack GetAttackFromDictionary(AttackInputType attackType){
        if (attackDictionary.Keys.Contains(attackType)){
            return  attackDictionary[attackType];
        }
        return null;
    }
}
