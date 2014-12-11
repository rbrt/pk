using UnityEngine;
using System.Collections;

public class AnimateDude : MonoBehaviour {

    [SerializeField] protected Sprite[] idleSprites,
                                        punchSprites,
                                        damageSprites,
                                        deadSprites,
                                        blockSprites;

    protected SpriteRenderer spriteRenderer;
    protected float animationSpeed = .025f;

    SafeCoroutine animationCoroutine;

    IEnumerator Primer(){
        yield break;
    }

	void Start () {
        animationCoroutine = this.StartSafeCoroutine(Primer());
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

    void CancelCurrentAnimation(){
        if (animationCoroutine.IsRunning || animationCoroutine.IsPaused){
            animationCoroutine.Stop();
        }
    }

    IEnumerator Animate(Sprite[] frames){
        int frameCount = 0;
        while (frameCount < frames.Length){
            spriteRenderer.sprite = frames[frameCount];
            frameCount++;
            yield return new WaitForSeconds(animationSpeed);
        }
    }

    public void Attack(Sprite[] attackSprites){
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(attackSprites));
    }

    public void Idle(){
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(idleSprites));
    }

    public void Damage(){
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(damageSprites));
    }

    public void Dead(){
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(deadSprites));
    }

    public void Block(){
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(blockSprites));
    }
}
