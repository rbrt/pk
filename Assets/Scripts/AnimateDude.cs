using UnityEngine;
using System.Collections;

public class AnimateDude : MonoBehaviour {

    [SerializeField] protected Sprite idleSprite,
                                      punchSprite,
                                      damageSprite,
                                      deadSprite;

    protected SpriteRenderer spriteRenderer;

	void Start () {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

    public void Punch(){
        spriteRenderer.sprite = punchSprite;
    }

    public void Idle(){
        spriteRenderer.sprite = idleSprite;
    }

    public void Damage(){
        spriteRenderer.sprite = damageSprite;
    }

    public void Dead(){
        spriteRenderer.sprite = deadSprite;
    }
}
