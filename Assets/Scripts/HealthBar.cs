using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [SerializeField] protected Image healthBar;
    protected float baseWidth,
                    baseX,
                    currentHealth,
                    health = 100;

    void Start () {
        currentHealth = health;
        baseWidth = healthBar.rectTransform.rect.width;
        baseX = healthBar.rectTransform.position.x;
	}

    void Update(){
        if (health > currentHealth){
            health -= Time.deltaTime * 10;

            if (health < currentHealth){
                health = currentHealth;
            }

            healthBar.rectTransform.sizeDelta = new Vector2(baseWidth * (health / 100), healthBar.rectTransform.rect.height);
            var pos = healthBar.rectTransform.position;

            healthBar.rectTransform.position = pos;
        }


    }

    public void DecrementHealth(float amount){
        currentHealth -= amount;
        if (currentHealth <= 0){
            PlayerController.Instance.Die();
        }
    }

}
