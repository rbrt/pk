using UnityEngine;
using System.Collections;

public class KnockbackAttackEffect : AttackEffect {

    protected override IEnumerator Effect(GameObject target){
        var basePos = target.transform.position;
        var currentPos = basePos;
        var trans = target.transform;

        var animator = target.GetComponent<AnimateEnemy>();
        animator.Damage();

        float count = 0,
              increment = .02f;


        bool goRight = target.transform.rotation.y != 0;

        while (count < 1){
            if (goRight){
                currentPos.x += increment;
            }
            else{
                currentPos.x -= increment;
            }

            count += .3f;
            trans.position = currentPos;

            yield return null;
        }
    }

}
