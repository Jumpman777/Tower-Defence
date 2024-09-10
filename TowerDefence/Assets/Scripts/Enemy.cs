using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float damage = 10f;   // Damage dealt per attack
    public float attackRate = 1f; // Time between attacks
    public float detectionRange = 10f; // Detection range for defenders
    public float attackRange = 1.5f;   // Range within which enemies can attack
    public Transform tower;  // Reference to the tower object

    private float nextAttackTime = 0f;
    private Transform target;
    private HealthBar targetHealthBar;
    private Tower towerScript;  // Reference to the Tower script

    void Start()
    {
        // Get the Tower script from the tower GameObject
        if (tower != null)
        {
            towerScript = tower.GetComponent<Tower>();
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Check if enemy is within attack range
            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                if (Time.time >= nextAttackTime)
                {
                    nextAttackTime = Time.time + 1f / attackRate;
                    AttackDefender();
                }
            }
            else
            {
                SetDestination(target.position);
            }
        }
        else
        {
            // If no defender is nearby, move towards the tower
            if (tower != null && Vector3.Distance(transform.position, tower.position) <= attackRange)
            {
                if (Time.time >= nextAttackTime)
                {
                    nextAttackTime = Time.time + 1f / attackRate;
                    AttackTower();
                }
            }
            else
            {
                SetDestination(tower.position);
            }
        }
    }

    private void AttackDefender()
    {
        if (targetHealthBar != null)
        {
            targetHealthBar.TakeDamage(damage);
        }
    }

    private void AttackTower()
    {
        if (towerScript != null)
        {
            towerScript.TakeDamage(damage);
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
                    targetHealthBar = target.GetComponent<HealthBar>();
                }
            }
        }
    }

    private void SetDestination(Vector3 destination)
    {
        // Moves the enemy towards the target position
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * 3f);
    }
}

