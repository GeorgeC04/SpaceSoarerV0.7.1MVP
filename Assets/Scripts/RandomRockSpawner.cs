using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;


public class RandomRockSpawner : MonoBehaviour
{
    public GameObject[] rockPrefab;
    public GameObject plane;
    public ScoreCounter currentScore; //Gets the current score
    public float spawnInterval = 0f; // Time interval between spawns
    public float rockSpeed = 0.09f; // Speed at which rocks move from right to left
    public int maxRocks = 50; // Maximum number of rocks on screen at one time
    public Transform spawnPoint;
    public int startFrameCount = 0;//300; // Number of frames to wait before starting to spawn rocks
    public float questionSpawnInterval = 5.0f;


    private float previousRockPosition;
    //private float previousRockPosition = 10;


    //public MathsPuzzleManager PuzzleManage = Plane.FindObjectOfType(typeof(MathsPuzzleManager)) as MathsPuzzleManager;

    private float topViewPort;

    private float bottomViewPort;

    private float validPositionTop;

    private float validPositionBottom;

    private float questionSpawn = 0f;

    private List<GameObject> spawnedRocks = new List<GameObject>();
    private float timer = 0f;

    private bool rockSpawnAllow = true;

    public Difficulty currentDifficulty = Difficulty.Easy;
    private Dictionary<GameObject, float> rockSpeeds = new Dictionary<GameObject, float>();




    void Start()
    {
        //previousRockPosition = (Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.transform.position.z)).y - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z)).y) / 2;
        topViewPort = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.transform.position.z)).y;
        bottomViewPort = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z)).y;
        //Centre of the Y view port
        previousRockPosition = 0;
        // SpawnRock();~
        currentDifficulty = GameSettings.SelectedDifficulty;
        if (currentDifficulty == Difficulty.Easy)
        {

            rockSpeed += 0.01f;
        }
        else
        {

            rockSpeed += 0.05f;
        }

        Debug.Log("Rock speed is... " + rockSpeed);

    }

    public void begin()
    {
        plane.GetComponent<MathsPuzzleManager>().enabled = false;
        plane.GetComponent<ColourPattern>().enabled = false;
        rockSpawnAllow = true;
        //previousRockPosition = (Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.transform.position.z)).y - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z)).y) / 2;
        topViewPort = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.transform.position.z)).y;
        bottomViewPort = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z)).y;
        //Centre of the Y view port
        previousRockPosition = 0;
        // SpawnRock();

    }

    void Update()
    {
        if (startFrameCount > 0)
        {
            startFrameCount--;
            return;
            //frame count down to start spawning rocks
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

        // Timer to control spawn interval
        timer += Time.deltaTime;
        if (timer >= spawnInterval && spawnedRocks.Count < maxRocks && rockSpawnAllow)
        {

            if (questionSpawn >= questionSpawnInterval)
            {
                rockSpawnAllow = false;
                int whichOne = Random.Range(0, 2);
                //int whichOne = 1;
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
                //this.enabled = false;
                //thisPlane.MathsPuzzleManager.start();
                //puzzle.start();
                //Plane.FindObjectOfType(typeof(MathsPuzzleManager)).start();
            }
            else
            {
                SpawnRock();
                timer = 0f;
                questionSpawn++;
            }


        }

        // Move the spawned rocks from right to left
        foreach (GameObject rock in spawnedRocks)
{
    if (rock != null)
    {
        float speed;
        if (rockSpeeds.TryGetValue(rock, out speed))
        {
            rock.transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
        }
        else
        {
            // fallback speed if not found
            rock.transform.Translate(Vector3.left * rockSpeed * Time.deltaTime, Space.World);
        }
    }
}

        // Check if rocks should be destroyed when off-screen
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

    public int spawnYValue()
    {
        return 0;
    }

    void SpawnRock()
    {
        float spawnY = Random.Range(previousRockPosition - 50, previousRockPosition + 50);
        while (spawnY < (bottomViewPort - (bottomViewPort / 100) * 80) || spawnY > (topViewPort - (topViewPort / 100) * 5))
        {
            spawnY = Random.Range(previousRockPosition - 50, previousRockPosition + 50);
        }
        previousRockPosition = spawnY;


        float offsetX = 100f; // Adjust this value to control how far to the right 


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

        

        // Pick random rock prefab
        GameObject randomRockPrefab1 = rockPrefab[Random.Range(0, rockPrefab.Length)];
        GameObject randomRockPrefab2 = rockPrefab[Random.Range(0, rockPrefab.Length)];

        GameObject spawnedRock = SpawnRandomizedAsteroid(spawnPosition);



        spawnedRock.tag = "Asteroid";
        spawnedRock.SetActive(true);

        GameObject underneathRock = SpawnRandomizedAsteroid(secondSpawnPosition);


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
    int prefabIndex = Random.Range(0, rockPrefab.Length);
    Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    GameObject asteroid = Instantiate(rockPrefab[prefabIndex], position, randomRotation);

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

    // ✅ Ensure all spawned rocks rotate
    asteroid.AddComponent<RockRotation>();

    return asteroid;
}









    //SpriteRenderer spriteRenderer = spawnedRock.GetComponent<SpriteRenderer>();
    //if (spriteRenderer != null)
    //{
    //  spriteRenderer.sortingLayerName = "Default"; // Ensure this matches a visible sorting layer
    //spriteRenderer.sortingOrder = 0; // Adjust the sorting order if needed
    //}
}
