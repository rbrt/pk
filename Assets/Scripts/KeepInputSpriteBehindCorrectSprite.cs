using UnityEngine;
using System.Collections;

public class KeepInputSpriteBehindCorrectSprite : MonoBehaviour {

    [SerializeField] protected SpriteRenderer thisSpriteRenderer,
                                              targetSpriteRenderer;

	void Update () {
        thisSpriteRenderer.sortingOrder = targetSpriteRenderer.sortingOrder + 1;
	}
}
