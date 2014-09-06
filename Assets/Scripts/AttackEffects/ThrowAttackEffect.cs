using UnityEngine;
using System.Collections;

public class ThrowAttackEffect : AttackEffect {

	protected override IEnumerator Effect(GameObject target){
        var basePos = target.transform.position;
        var currentPos = basePos;
        var trans = target.transform;

        var animator = target.GetComponent<AnimateEnemy>();

        float count = 0,
              countIncrement = .08f,
              xOffset = .025f,
              timeOnGroundBetweenBounce = .1f,
              timeOnGround = 1;

        int bounces = 0,
            bounceCount = 3;

        bool goRight = target.transform.rotation.y != 0;

        while (bounces < bounceCount){
            count = 0;
            bool thrown = false;

            while (count < Mathf.PI){
                if (!thrown){
                    if (bounces > 0){
                        thrown = true;
                        animator.Thrown();
                    }
                    else if (count > Mathf.PI / 4){
                        thrown = true;
                        animator.Thrown();
                    }
                }

                if (goRight){
                    currentPos.x += xOffset - (.02f * (bounces/(float)bounceCount));
                }
                else{
                    currentPos.x -= xOffset - (.02f * (bounces/(float)bounceCount));
                }

                currentPos.y = (basePos.y + Mathf.Sin(count)) * (bounceCount - bounces)/(bounceCount);

                count += countIncrement;
                trans.position = currentPos;

                yield return null;
            }
            bounces++;
            animator.HitGround();
            yield return new WaitForSeconds(timeOnGroundBetweenBounce);
        }

        yield return new WaitForSeconds(timeOnGround);
    }

}
