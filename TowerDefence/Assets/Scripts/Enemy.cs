using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float damage = 10f; // Damage dealt to defenders or tower
    public float attackRate = 1f; // Rate at which enemies attack
    public float detectionRange = 10f; // Range in which enemies detect defenders
    public float attackRange = 1.5f; // Range in which enemies attack
    public Transform tower;  // Reference to the tower

    private float nextAttackTime = 0f;
    private Transform target;
    private HealthBar targetHealthBar; // Health component of defenders or tower
    private Tower towerScript; // Tower script reference for health

    void Start()
    {
        towerScript = tower.GetComponent<Tower>(); // Assign the Tower script
    }

    void Update()
    {
        if (target != null)
        {
            // Check if the enemy is within attack range of the target (defender or tower)
            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                if (Time.time >= nextAttackTime)
                {
                    nextAttackTime = Time.time + 1f / attackRate;
                    Attack(targetHealthBar); // Attack defenders or the tower
                }
            }
            else
            {
                // Move towards the target if not in attack range
                SetDestination(target.position);
            }
        }
        else
        {
            // No defender detected, move towards the tower
            FindNearestDefender();
            if (target == null)
            {
                // If no defenders found, head towards the tower
                SetDestination(tower.position);
            }
        }
    }

    private void FindNearestDefender()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        float closestDistance = Mathf.Infinity;

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Defender"))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = collider.transform;
                    targetHealthBar = target.GetComponent<HealthBar>(); // Get the defender's health component
                }
            }
        }
    }

    private void SetDestination(Vector3 destination)
    {
        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * 3f);
    }

    private void Attack(HealthBar targetHealthBar)
    {
        // Check if attacking a defender
        if (targetHealthBar != null)
        {
            targetHealthBar.TakeDamage(damage);
        }
        else if (target == tower) // Check if attacking the tower
        {
            towerScript.TakeDamage(damage); // Damage the tower directly
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tower"))
        {
            // Start attacking the tower when in range
            target = tower;
            targetHealthBar = null; // No health bar for the tower
        }
    }
}
