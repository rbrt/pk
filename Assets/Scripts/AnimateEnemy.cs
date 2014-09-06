using UnityEngine;
using System.Collections;

public class AnimateEnemy : MonoBehaviour {

    [SerializeField] protected Sprite[] idleSprites,
                                        punchSprites,
                                        damageSprites,
                                        deadSprites,
                                        movementSprites,
                                        thrownSprites,
                                        hitGroundSprites;

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

    IEnumerator Animate(Sprite[] frames, float animationSpeed, bool repeat = false){
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
        animationCoroutine = this.StartSafeCoroutine(Animate(punchSprites, animationSpeed));
    }

    public void Idle(){
        float animationSpeed = .1f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(idleSprites, animationSpeed, true));
    }

    // Should not be move sprites in the future
    public void Inactive(){
        float animationSpeed = .025f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(movementSprites, animationSpeed));
    }

    public void Damage(){
        float animationSpeed = .025f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(damageSprites, animationSpeed));
    }

    public void Dead(){
        float animationSpeed = .025f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(deadSprites, animationSpeed));
    }

    public void Move(){
        float animationSpeed = .025f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(movementSprites, animationSpeed, true));
    }

    public void Thrown(){
        float animationSpeed = .25f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(thrownSprites, animationSpeed));
    }

    public void HitGround(){
        float animationSpeed = .025f;
        CancelCurrentAnimation();
        animationCoroutine = this.StartSafeCoroutine(Animate(hitGroundSprites, animationSpeed));
    }
}
