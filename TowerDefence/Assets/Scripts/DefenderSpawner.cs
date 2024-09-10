using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DefenderSpawner : MonoBehaviour
{
    public GameObject defenderPrefab; // Reference to the defender prefab
    public Transform terrain; // The terrain where defenders can be placed
    public int maxDefenders = 5; // Maximum number of defenders allowed
    private int currentDefenderCount = 0; // Current number of defenders

    public UIManager uiManager; // Reference to the UIManager

    private void Start()
    {
        // Initialize the UIManager reference if it's not set
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
    }

    public void SpawnDefender(Vector3 position)
    {
        if (currentDefenderCount < maxDefenders)
        {
            GameObject defender = Instantiate(defenderPrefab, position, Quaternion.identity);
            currentDefenderCount++;
            uiManager.UpdateDefenderCountText(currentDefenderCount); // Update UI

            // Optional: Add any additional setup for the defender here
        }
        else
        {
            Debug.Log("Maximum number of defenders reached!");
        }
    }

    public void OnDefenderDestroyed()
    {
        if (currentDefenderCount > 0)
        {
            currentDefenderCount--;
            uiManager.UpdateDefenderCountText(currentDefenderCount); // Update UI
        }
    }
}












