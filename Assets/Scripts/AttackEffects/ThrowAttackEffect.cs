using UnityEngine;
using System.Collections;

public class ThrowAttackEffect : AttackEffect {

	protected override IEnumerator Effect(GameObject target){
        var basePos = target.transform.position;
        var currentPos = basePos;
        var trans = target.transform;

        float count = 0,
              countIncrement = .01f,
              xOffset = .1f;

        while (count < Mathf.PI * 1.5f){
            currentPos.x = basePos.x + xOffset;
            currentPos.y = basePos.y + Mathf.Sin(count);

            count += countIncrement;
            trans.position = currentPos;

            yield return null;
        }
    }

}
