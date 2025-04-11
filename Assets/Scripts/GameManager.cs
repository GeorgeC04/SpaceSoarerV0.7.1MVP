using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private RockSpawner rockSpawner;

    void Start()
    {
        // Call the method to spawn rocks when the game starts
        SpawnRocks();
    }

    public void SpawnRocks()
    {
        if (rockSpawner != null)
        {
            rockSpawner.SpawnRocks();
        }
        else
        {
            Debug.LogError("Rock Spawner not assigned.");
        }
    }
}
