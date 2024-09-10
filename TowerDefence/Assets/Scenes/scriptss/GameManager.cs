using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GameObject pathTerrainPrefab;
    public GameObject mountainTerrainPrefab;
    public Transform pathTerrainPosition;
    public Transform mountainTerrainPosition;

    private void Start()
    {
        // Instantiate and set up path terrain
        GameObject pathTerrain = Instantiate(pathTerrainPrefab, pathTerrainPosition.position, Quaternion.identity);
        pathTerrain.GetComponent<TerrainGenerator>().enabled = true;

        // Instantiate and set up mountain terrain
        GameObject mountainTerrain = Instantiate(mountainTerrainPrefab, mountainTerrainPosition.position, Quaternion.identity);
        mountainTerrain.GetComponent<MountainGenerator>().enabled = true;
    }
}
