using UnityEngine;
using System.Collections;

public class AttackEffect : MonoBehaviour {

	public SafeCoroutine InvokeEffect(GameObject target){
        return this.StartSafeCoroutine(Effect(target));
    }

    protected virtual IEnumerator Effect(GameObject target){
        yield break;
    }

}
