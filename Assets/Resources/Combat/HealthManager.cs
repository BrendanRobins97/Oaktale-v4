using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {

    public float currentHealth;
    public float maxHealth;
	
	void Start () {
        FullHealth();
	}
    private void Update()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    public float HealthPercent()
    {
        return currentHealth / maxHealth;
    }
    public void Damage(float value)
    {
        currentHealth -= value;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
    }

    public void Heal(float value)
    {
        currentHealth += value;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void Kill()
    {
        currentHealth = 0;
    }

    public void FullHealth()
    {
        currentHealth = maxHealth;
    }
}
