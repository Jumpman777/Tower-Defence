using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform tower;  // Reference to the tower's transform

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Set movement properties
        agent.speed = 3.5f;  // Movement speed of the enemy
        agent.angularSpeed = 120f;  // Speed of turning
        agent.acceleration = 8f;  // How quickly the agent accelerates
        agent.stoppingDistance = 1f;  // Distance at which the agent stops from the destination
        agent.radius = 0.5f;  // Radius of the agent for pathfinding

        // Set the tower as the destination
        agent.SetDestination(tower.position);
    }

    void Update()
    {
        // Optional: you can add additional logic here for behaviors or updates
    }
}

