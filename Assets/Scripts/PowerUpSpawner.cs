using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Orb Prefab ")]
    public GameObject orbPrefab;     // ← drag the prefab asset, NOT a scene instance

    [Header("Spawn Settings")]
    public float spawnInterval;
    public float minX , maxX ;  // world-space X range
    public float spawnY ;            // world-space Y above top of screen

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            SpawnOrb();
        }
    }

    private void SpawnOrb()
    {
        if (orbPrefab == null)
        {
            Debug.LogWarning("PowerUpSpawner: orbPrefab is missing!");
            return;
        }

        float x = Random.Range(minX, maxX);
        Vector3 pos = new Vector3(x, spawnY, 0f);
        // IMPORTANT: we never overwrite orbPrefab itself
        GameObject orb = Instantiate(orbPrefab, pos, Quaternion.identity);
        orb.SetActive(true);
    }
}
