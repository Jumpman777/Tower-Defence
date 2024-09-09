using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public HealthBar healthBar;

    private void Start()
    {
        healthBar = GetComponent<HealthBar>();
    }

    public void TakeDamage(float damage)
    {
        healthBar.TakeDamage(damage);
        if (healthBar.health <= 0)
        {
            // Handle tower destruction, game over, etc.
            Destroy(gameObject);
            Destroy(healthBar);
        }
    }
}

