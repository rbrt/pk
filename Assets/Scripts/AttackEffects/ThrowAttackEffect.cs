using UnityEngine;
using System.Collections;

public class ThrowAttackEffect : AttackEffect {

	protected override IEnumerator Effect(GameObject target){
        var basePos = target.transform.position;
        var currentPos = basePos;
        var trans = target.transform;

        float count = 0,
              countIncrement = .08f,
              xOffset = .025f,
              timeOnGroundBetweenBounce = .1f,
              timeOnGround = .5f;

        int bounces = 0,
            bounceCount = 3;

        while (bounces < bounceCount){
            count = 0;
            while (count < Mathf.PI){
                currentPos.x += xOffset - (.02f * (bounces/(float)bounceCount));
                currentPos.y = (basePos.y + Mathf.Sin(count)) * (bounceCount - bounces)/(bounceCount);

                count += countIncrement;
                trans.position = currentPos;

                yield return null;
            }
            bounces++;
            yield return new WaitForSeconds(timeOnGroundBetweenBounce);
        }

        yield return new WaitForSeconds(timeOnGround);
    }

}
