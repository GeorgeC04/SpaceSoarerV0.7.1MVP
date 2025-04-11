using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    public int numberOfRocks = 5; // Number of rocks to spawn
    public float spacing = 2f;    // Space between each rock
    public Transform spawnPoint;  // Assign the starting spawn point in the Inspector

    public void SpawnRocks()
    {
        if (rockPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Rock Prefab or Spawn Point not assigned.");
            return;
        }

        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 spawnPosition = spawnPoint.position + new Vector3(i * spacing, 0, 0);
            Instantiate(rockPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
