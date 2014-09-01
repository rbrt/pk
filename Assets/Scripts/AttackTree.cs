using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AttackTree : MonoBehaviour {

    protected PlayerAttack currentAttack;

    [SerializeField] protected PlayerAttack attack1,
                                            attack2;

    [SerializeField] protected HandlePlayerInputFlash handleInputFlash;

    SafeCoroutine behaviourCoroutine;

    public enum AttackInputType {Attack1, Attack2};

    Dictionary<AttackInputType, PlayerAttack> baseAttacks;

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

        if (currentAttack != null){
            currentAttack.StartTime = Time.time;
        }

        if ((oldAttack == attack1 || oldAttack == attack2) || (oldAttack != currentAttack && currentAttack != null)){
            if (behaviourCoroutine.IsPaused || behaviourCoroutine.IsRunning){
                behaviourCoroutine.Stop();
            }

            behaviourCoroutine = this.StartSafeCoroutine(HighlightTimings(currentAttack));
        }

        return currentAttack;
    }

    IEnumerator HighlightTimings(PlayerAttack playerAttack){
        var sortedMoves = playerAttack.AllMoves.OrderBy(x => x.CutoffTime).ToList();
        Dictionary<NextMove, bool> alertedMoves = new Dictionary<NextMove, bool>();

        sortedMoves.ForEach(x => alertedMoves[x] = false);

        float alertTime = .3f;

        int count = 0;
        float beginningTime = playerAttack.StartTime;

        bool logged = false,
             logged2 = false;

        Debug.Log("crotuon!");

        while (count < sortedMoves.Count){

            float elapsedTime = Time.time - beginningTime;

            if (elapsedTime > playerAttack.BaseAttackTime){
                if (sortedMoves[count].CutoffTime > elapsedTime){

                    if (Mathf.Abs(sortedMoves[count].CutoffTime - elapsedTime) <= alertTime){
                        if (!alertedMoves[sortedMoves[count]]){
                            alertedMoves[sortedMoves[count]] = true;
                            handleInputFlash.TriggerFlash();
                            count++;
                        }
                    }
                }
                else{
                    if (!logged2){
                        Debug.Log(elapsedTime + " " + sortedMoves[count].CutoffTime);
                        logged2 = true;
                    }
                    count++;
                }
            }
            else{
                if (!logged){
                    Debug.Log(elapsedTime + " " + playerAttack.BaseAttackTime);
                    logged = true;
                }
            }

            yield return null;
        }
    }
}
