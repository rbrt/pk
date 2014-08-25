using UnityEngine;
using System.Collections;

public class AnimateEnemy : MonoBehaviour {

    [SerializeField] protected Sprite[] idleSprites,
                                        punchSprites,
                                        damageSprites,
                                        deadSprites,
                                        movementSprites;

    protected SpriteRenderer spriteRenderer;

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

    IEnumerator Animate(Sprite[] frames, bool repeat, float animationSpeed){
        int frameCount = 0;

        while (frameCount < frames.Length){
            spriteRenderer.sprite = frames[frameCount];
            frameCount++;

            if (repeat && frameCount > frames.Length - 1){
                frameCount = 0;
            }

            yield return new WaitForSeconds(animationSpeed);
        }
    }

    public void Punch(){
        float animationSpeed = .025f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(punchSprites, false, animationSpeed));
    }

    public void Idle(){
        float animationSpeed = .1f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(idleSprites, true, animationSpeed));
    }

    // Should not be move sprites in the future
    public void Inactive(){
        float animationSpeed = .025f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(movementSprites, false, animationSpeed));
    }

    public void Damage(){
        float animationSpeed = .025f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(damageSprites, false, animationSpeed));
    }

    public void Dead(){
        float animationSpeed = .025f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(deadSprites, false, animationSpeed));
    }

    public void Move(){
        float animationSpeed = .025f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(movementSprites, true, animationSpeed));
    }
}
