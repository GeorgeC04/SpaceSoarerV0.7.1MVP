using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRockSpawner : MonoBehaviour
{
    public GameObject[] easyRockPrefabs; // Asteroid1–4
    public GameObject[] hardRockPrefabs; // ColouredAsteroid1–4
    private GameObject[] rockPrefabs;    // Active prefab set based on difficulty

    public GameObject plane;
    public ScoreCounter currentScore;
    public float spawnInterval = 0f;
    public float rockSpeed = 0.09f;
    public int maxRocks = 50;
    public Transform spawnPoint;
    public int startFrameCount = 0;
    public float questionSpawnInterval = 5.0f;

    private float previousRockPosition;
    private float topViewPort;
    private float bottomViewPort;
    private float questionSpawn = 0f;
    private float timer = 0f;

    private bool rockSpawnAllow = true;
    public Difficulty currentDifficulty = Difficulty.Easy;

    private List<GameObject> spawnedRocks = new List<GameObject>();
    private Dictionary<GameObject, float> rockSpeeds = new Dictionary<GameObject, float>();

    void Start()
    {
        topViewPort = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.transform.position.z)).y;
        bottomViewPort = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z)).y;
        previousRockPosition = 0;

        currentDifficulty = GameSettings.SelectedDifficulty;

        // Assign rock prefabs based on difficulty
        if (currentDifficulty == Difficulty.Easy)
        {
            rockPrefabs = easyRockPrefabs;
            rockSpeed += 0.01f;
        }
        else
        {
            rockPrefabs = hardRockPrefabs;
            rockSpeed += 0.05f;
        }

        Debug.Log("Rock speed is... " + rockSpeed);
    }

    public void begin()
    {
        plane.GetComponent<MathsPuzzleManager>().enabled = false;
        plane.GetComponent<ColourPattern>().enabled = false;
        rockSpawnAllow = true;

        topViewPort = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.transform.position.z)).y;
        bottomViewPort = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z)).y;
        previousRockPosition = 0;
    }

    void Update()
    {
        if (startFrameCount > 0)
        {
            startFrameCount--;
            return;
        }

        int thisScore = Mathf.FloorToInt(currentScore.GetComponent<ScoreCounter>().score);

        if (thisScore % 7 == 0)
        {
            if (currentDifficulty == Difficulty.Easy)
            {
                rockSpeed += 0.0005f;
            }
            else
            {
                rockSpeed += 0.10f;
                spawnInterval -= 0.0001f;
            }
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval && spawnedRocks.Count < maxRocks && rockSpawnAllow)
        {
            if (questionSpawn >= questionSpawnInterval)
            {
                rockSpawnAllow = false;
                int whichOne = Random.Range(0, 2);

                if (whichOne == 0)
                {
                    plane.GetComponent<ColourPattern>().enabled = false;
                    plane.GetComponent<MathsPuzzleManager>().enabled = true;
                    plane.GetComponent<MathsPuzzleManager>().beginPuzzle();
                }
                else
                {
                    plane.GetComponent<MathsPuzzleManager>().enabled = false;
                    plane.GetComponent<ColourPattern>().enabled = true;
                    plane.GetComponent<ColourPattern>().beginPuzzle();
                }

                questionSpawn = 0f;
            }
            else
            {
                SpawnRock();
                timer = 0f;
                questionSpawn++;
            }
        }

        foreach (GameObject rock in spawnedRocks)
        {
            if (rock != null)
            {
                if (rockSpeeds.TryGetValue(rock, out float speed))
                {
                    rock.transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
                }
                else
                {
                    rock.transform.Translate(Vector3.left * rockSpeed * Time.deltaTime, Space.World);
                }
            }
        }

        float screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z)).x;
        for (int i = spawnedRocks.Count - 1; i >= 0; i--)
        {
            if (spawnedRocks[i].transform.position.x < screenLeft)
            {
                Destroy(spawnedRocks[i]);
                spawnedRocks.RemoveAt(i);
            }
        }
    }

    void SpawnRock()
    {
        float spawnY = Random.Range(previousRockPosition - 50, previousRockPosition + 50);
        while (spawnY < (bottomViewPort - (bottomViewPort / 100) * 80) || spawnY > (topViewPort - (topViewPort / 100) * 5))
        {
            spawnY = Random.Range(previousRockPosition - 50, previousRockPosition + 50);
        }
        previousRockPosition = spawnY;

        float offsetX = 100f;

        Vector3 spawnPosition = spawnPoint.position;
        Vector3 secondSpawnPosition = spawnPoint.position;
        Vector3 blockOffPositionAbove = spawnPoint.position;
        Vector3 blockOffPositionBelow = spawnPoint.position;

        spawnPosition.y = spawnY;
        secondSpawnPosition.y = spawnY - 150;
        spawnPosition.x += offsetX;
        secondSpawnPosition.x += offsetX;
        blockOffPositionAbove.x += offsetX;
        blockOffPositionBelow.x += offsetX;

        GameObject spawnedRock = SpawnRandomizedAsteroid(spawnPosition);
        spawnedRock.tag = "Asteroid";
        spawnedRock.SetActive(true);

        GameObject underneathRock = SpawnRandomizedAsteroid(secondSpawnPosition);
        underneathRock.tag = "Asteroid";
        underneathRock.SetActive(true);

        spawnedRocks.Add(spawnedRock);
        spawnedRocks.Add(underneathRock);

        blockOffPositionAbove.y = spawnPosition.y;
        blockOffPositionBelow.y = secondSpawnPosition.y;

        while (blockOffPositionAbove.y < topViewPort)
        {
            blockOffPositionAbove.y += 70;
            GameObject blockerRock = SpawnRandomizedAsteroid(blockOffPositionAbove);
            blockerRock.tag = "Asteroid";
            blockerRock.AddComponent<RockRotation>();
            blockerRock.SetActive(true);
            spawnedRocks.Add(blockerRock);
        }

        while (blockOffPositionBelow.y > bottomViewPort)
        {
            blockOffPositionBelow.y -= 70;
            GameObject blockerRock = SpawnRandomizedAsteroid(blockOffPositionBelow);
            blockerRock.tag = "Asteroid";
            blockerRock.AddComponent<RockRotation>();
            blockerRock.SetActive(true);
            spawnedRocks.Add(blockerRock);
        }
    }

    GameObject SpawnRandomizedAsteroid(Vector3 position)
    {
        int prefabIndex = Random.Range(0, rockPrefabs.Length);
        Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        GameObject asteroid = Instantiate(rockPrefabs[prefabIndex], position, randomRotation);

        float randomScale = Random.Range(10f, 15f);
        asteroid.transform.localScale = Vector3.one * randomScale;

        float speedForThisRock = Random.Range(rockSpeed * 0.95f, rockSpeed * 1.05f);
        rockSpeeds[asteroid] = speedForThisRock;

        SpriteRenderer sr = asteroid.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = Random.Range(-3, 3);
        }

        asteroid.tag = "Asteroid";
        asteroid.SetActive(true);
        asteroid.AddComponent<RockRotation>();

        return asteroid;
    }
}
