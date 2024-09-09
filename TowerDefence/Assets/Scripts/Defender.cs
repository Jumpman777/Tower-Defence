using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    public float damage = 5f; // Damage the defender deals to enemies
    public float attackRate = 1f; // Time between attacks
    public float moveSpeed = 2f; // Speed at which the defender moves towards enemies
    private float nextAttackTime = 0f; // Time of the next attack
    private Transform target; // The current target (enemy)
    private HealthBar targetHealthBar; // Health bar of the target enemy

    private HealthBar healthBar; // Reference to this defender's health bar

    void Start()
    {
        healthBar = GetComponent<HealthBar>(); // Get the HealthBar component attached to this defender
    }

    void Update()
    {
        // If there is a target and it's time to attack
        if (target != null && Time.time >= nextAttackTime && IsInRange(target))
        {
            nextAttackTime = Time.time + 1f / attackRate; // Calculate the next attack time
            Attack(targetHealthBar); // Attack the target
        }
        // If there is a target but it's not in range, move towards it
        else if (target != null && !IsInRange(target))
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized; // Calculate the direction to the target
            transform.position += direction * moveSpeed * Time.deltaTime; // Move the defender towards the target
        }
    }

    private bool IsInRange(Transform target)
    {
        // Check if the target is within the attack range (1.5 units in this case)
        return Vector3.Distance(transform.position, target.position) <= 1.5f;
    }

    private void Attack(HealthBar targetHealthBar)
    {
        if (targetHealthBar != null)
        {
            targetHealthBar.TakeDamage(damage); // Deal damage to the target's health bar
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // When the defender collides with an enemy, set it as the target
        if (other.CompareTag("Enemy"))
        {
            target = other.transform;
            targetHealthBar = other.GetComponent<HealthBar>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the enemy leaves the defender's range, clear the target
        if (other.CompareTag("Enemy") && other.transform == target)
        {
            target = null;
            targetHealthBar = null;
        }
    }

    public void TakeDamage(float amount)
    {
        if (healthBar != null)
        {
            healthBar.TakeDamage(amount); // Reduce the defender's health

            // If the defender's health drops to zero or below, destroy it
            if (healthBar.health <= 0)
            {
                FindObjectOfType<DefenderSpawner>().OnDefenderDestroyed(); // Notify DefenderSpawner of the destruction
                Destroy(gameObject);
            }
        }
    }
}

