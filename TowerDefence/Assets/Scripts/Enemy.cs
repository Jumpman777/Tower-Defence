using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float damage = 5f; // Damage the enemy deals
    public float attackRate = 1f; // Time between attacks
    public float moveSpeed = 2f; // Speed at which the enemy moves
    private float nextAttackTime = 0f; // Time of the next attack
    private Transform target; // Current target (either defender or tower)
    private HealthBar targetHealthBar; // Health bar of the current target

    private HealthBar healthBar; // Reference to this enemy's health bar

    void Start()
    {
        healthBar = GetComponent<HealthBar>(); // Get the HealthBar component attached to this enemy
        target = GameObject.FindGameObjectWithTag("Tower").transform; // Default target is the tower
        targetHealthBar = target.GetComponent<HealthBar>();
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
            transform.position += direction * moveSpeed * Time.deltaTime; // Move the enemy towards the target
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
        // If an enemy collides with a defender, set it as the target
        if (other.CompareTag("Defender"))
        {
            target = other.transform;
            targetHealthBar = other.GetComponent<HealthBar>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the defender leaves the enemy's range, target the tower again
        if (other.CompareTag("Defender") && other.transform == target)
        {
            target = GameObject.FindGameObjectWithTag("Tower").transform;
            targetHealthBar = target.GetComponent<HealthBar>();
        }
    }

    public void TakeDamage(float amount)
    {
        if (healthBar != null)
        {
            healthBar.TakeDamage(amount); // Reduce the enemy's health

            // If the enemy's health drops to zero or below, destroy it
            if (healthBar.health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}


