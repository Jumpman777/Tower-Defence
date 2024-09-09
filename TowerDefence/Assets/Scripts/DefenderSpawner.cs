using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this for the UI elements

public class DefenderSpawner : MonoBehaviour
{
    public GameObject defenderPrefab;
    public int maxDefenders = 5;
    private int currentDefenderCount = 0;

    // Reference to the UI Text element to display remaining defenders
    public Text defenderCountText;

    private void Start()
    {
        UpdateDefenderCountUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentDefenderCount < maxDefenders)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Terrain"))
                {
                    SpawnDefender(hit.point);
                }
            }
        }
    }

    private void SpawnDefender(Vector3 position)
    {
        Instantiate(defenderPrefab, position, Quaternion.identity);
        currentDefenderCount++;
        UpdateDefenderCountUI();
    }

    private void UpdateDefenderCountUI()
    {
        defenderCountText.text = "Defenders Remaining: " + (maxDefenders - currentDefenderCount).ToString();
    }

    public void OnDefenderDestroyed()
    {
        currentDefenderCount--;
        UpdateDefenderCountUI();
    }
}


