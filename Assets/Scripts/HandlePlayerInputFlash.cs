using UnityEngine;
using System.Collections;

public class HandlePlayerInputFlash : MonoBehaviour {

    [SerializeField] protected SpriteRenderer flashRenderer,
                                              playerRenderer;

	void Update () {
        if (flashRenderer.sprite != playerRenderer.sprite){
            flashRenderer.sprite = playerRenderer.sprite;
        }
	}

    void Awake(){
        flashRenderer.material.SetFloat("_StartTime", Time.time - 2);
        flashRenderer.material.SetFloat("_EffectTime", 0);
    }

    public void TriggerFlash(){
        flashRenderer.material.SetFloat("_EffectTime", .1f);
        flashRenderer.material.SetFloat("_StartTime", Time.time);
    }
}
