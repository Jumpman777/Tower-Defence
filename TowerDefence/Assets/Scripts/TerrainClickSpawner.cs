using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TerrainClickSpawner : MonoBehaviour
{
    public GameObject defenderPrefab; // Reference to the defender prefab
    public float spawnHeight = 1f; // Height at which defenders are placed

    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the clicked object has the "Terrain" tag
                if (hit.collider.CompareTag("Terrain"))
                {
                    // Get the spawn position and adjust height
                    Vector3 spawnPosition = new Vector3(hit.point.x, spawnHeight, hit.point.z);

                    // Debug log to verify position
                    Debug.Log("Spawning defender at: " + spawnPosition);

                    PlaceDefender(spawnPosition);
                }
            }
        }
    }

    void PlaceDefender(Vector3 position)
    {
        // Instantiate defender prefab at the specified position
        GameObject defender = Instantiate(defenderPrefab, position, Quaternion.identity);
        Debug.Log("Defender instantiated at: " + position);

        // Optionally, set up additional properties or components here
    }
}

