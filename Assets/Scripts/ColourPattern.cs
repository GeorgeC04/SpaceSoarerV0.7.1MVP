using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.PackageManager.UI;

/*
public enum Difficulty
{
    Easy,
    Hard
}
*/
public class ColourPattern : MonoBehaviour
{
    public GameObject random;
    [Range(-1f, 1f)]
    public float scrollSpeed = 0.5f;
    public float rockSpeed = 1.0f;   // Speed at which rocks move from right to left
    private float offset;
    private Material mat;
    private float distanceMoved = 0f;
    public float spawnDistance = 10f;
    public TextMeshProUGUI puzzleText;
    public TextMeshProUGUI rockText;

    public GameObject rockPrefab;
    public int numberOfRocks = 10; // Number of rocks to spawn
    public float spacing = 2f;    // Space between each rock
    public Transform spawnPoint;

    public TextMeshProUGUI healthBarTxt;
    public GameObject healthBarObject;

    private int correctAnswer;
    private string correctWordAnswer;

    //private bool questionDisplayed = false;
    //private bool rocksSpawned = false;
    private List<GameObject> spawnedRocks = new List<GameObject>();

    public Transform spawnStart;

    //public RandomRockSpawner rockSpawner;

    private int frameCounter = 0;
    public int framesPerSpawn = 600; // The number of frames between each spawn cycle


    public ScoreCounter currentScore; //Gets the current score
    /*
        void Start()
        {
            mat = GetComponent<Renderer>().material;
            if (puzzleText != null)
            {
                puzzleText.gameObject.SetActive(false); // text is initially hidden
            }
            else
            {
                Debug.LogError("PuzzleText is not assigned in the Inspector.");
            }
        }
    */
    private bool isMathPuzzle = true; // Starts with math puzzle

    public Difficulty currentDifficulty = Difficulty.Easy;


    public GameObject[] prefabs;

    public TextMeshProUGUI InitialPuzzleText;

    public GameObject plsJustWork;

    public void beginPuzzle()
    {

        Debug.Log("Yep this is being reached");


        /*
                healthBarObject.SetActive(false);
                currentScore.GetComponent<ScoreCounter>().scoreText.SetText("");

                healthBarTxt.text = "";
        */
        currentDifficulty = GameSettings.SelectedDifficulty;
        Debug.Log($"Starting puzzle with difficulty: {currentDifficulty}");



        /*
        mat = GetComponent<Renderer>().material;
        if (puzzleText != null)
        {
            puzzleText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("PuzzleText is not assigned in the Inspector.");
        }

        // Randomly decide between math and word puzzles
        if (Random.value > 0.5f) // 50% chance
        {
            GeneratePuzzle(); // Generate a math puzzle
        }
        else
        {
            GenerateWordPuzzle(); // Generate a word puzzle
        }

        SpawnRocks();
        frameCounter = 0;
        */

        /*
                foreach (GameObject pref in prefabs)
                {
                    pref.GetComponent<Renderer>().enabled = false;
                }
        */

        InitialPuzzleText.text = "Recreate the pattern below!!";


        int[] randomNumbers = new int[4];

        for (int i = 0; i < randomNumbers.Length; i++)
        {
            randomNumbers[i] = Random.Range(0, 4); // 0 to 3 inclusive
        }

        Debug.Log("Size is... " + randomNumbers.Length);

        int xDist = 500;

        foreach (int number in randomNumbers)
        {
            int l = 0;
            xDist += 150;

            prefabs[number].GetComponent<Renderer>().enabled = true;

            Vector3 spawnPosition = new Vector3(xDist, 0, 0);
            GameObject spawnedRock = Instantiate(prefabs[number], spawnPosition, Quaternion.identity, spawnStart);
            spawnedRock.SetActive(true);
            spawnedRocks.Add(spawnedRock);

            RockDespawner rockDespawner = spawnedRock.AddComponent<RockDespawner>();
            //rockDespawner.Initialize(this);
        }

        //Spawn the four rows of of rocks
        //And then set the tag for the right one to be "CorrectAnswer"
        //Then if they get it wrong they'll lose a life


        int xPos = 1600;
        int correctNumber;

        for (int n = 0; n < 4; n++)
        {

            int yPos = 150;
            xPos += 200;
            correctNumber = randomNumbers[n];

            for (int j = 0; j < 4; j++)
            {
                Vector3 spawnPosition = new Vector3(xPos, yPos, 0);
                GameObject spawnedRock = Instantiate(prefabs[j], spawnPosition, Quaternion.identity, spawnStart);
                spawnedRock.SetActive(true);
                spawnedRocks.Add(spawnedRock);

                RockDespawner rockDespawner = spawnedRock.AddComponent<RockDespawner>();
                rockDespawner.InitializePattern(this);

                yPos -= 100;

                if (j == correctNumber)
                {
                    spawnedRock.tag = "CorrectAnswer";
                }
            }
        }



            



    }



    void Update()
    {
        // Reset the frame counter

        // Check if it's time to spawn the question and rocks
        /*if (frameCounter >= framesPerSpawn)
        {
            GeneratePuzzle();
            SpawnRocks();
            frameCounter = 0; // Reset the frame counter
        }*/

        if (currentScore == null || random == null)
        {
            return;
        }



        int thisScore = Mathf.FloorToInt(currentScore.GetComponent<ScoreCounter>().score);

        if (thisScore % 20 == 0)
        {

            rockSpeed += 0.05f;
        }

        // Move the spawned rocks from right to left
        foreach (GameObject rock in spawnedRocks)
        {
            Debug.Log("We hopefull get it moving at some point?");
            if (rock != null)
            {
                rock.transform.Translate(Vector3.left * rockSpeed * Time.deltaTime);
            }
        }
    }

    void GeneratePuzzle()
    {
        if (currentDifficulty == Difficulty.Easy)
        {
            // Easy math puzzle: only addition
            if (puzzleText != null)
            {
                int num1 = Random.Range(1, 20);
                int num2 = Random.Range(1, 20);
                correctAnswer = num1 + num2;
                puzzleText.text = $"{num1} + {num2} = ?";
                puzzleText.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("PuzzleText is not assigned in the Inspector.");
            }
        }
        else if (currentDifficulty == Difficulty.Hard)
        {
            // Call hard math puzzle method
            if (puzzleText != null)
            {
                int num1 = Random.Range(20, 50);
                int num2 = Random.Range(20, 50);

                // Randomly choose an operation: 0 = addition, 1 = subtraction, 2 = multiplication
                int operation = Random.Range(0, 2);

                switch (operation)
                {
                    case 0:
                        correctAnswer = num1 - num2;
                        puzzleText.text = $"{num1} - {num2} = ?";
                        break;

                    case 1:
                        correctAnswer = num1 * num2;
                        puzzleText.text = $"{num1} � {num2} = ?";
                        break;

                    case 2:
                        correctAnswer = num1 + num2;
                        puzzleText.text = $"{num1} + {num2} = ?";
                        break;


                }

                puzzleText.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("PuzzleText is not assigned in the Inspector.");
            }
        }
    }


    void GenerateWordPuzzle()
    {
        if (puzzleText != null)
        {
            if (currentDifficulty == Difficulty.Easy)
            {
                // Easy difficulty: Words with one blank
            string[] words = {
                "SU_", "SK_", "STA_", "MOO_", "MAR_", "NOV_", "VOI_", "DUS_", "RIN_", "OR_",
                "AUR_", "LEN_", "POL_", "FLU_", "SPI_", "LUN_", "URS_", "TAI_", "AXI_", "SO_",
                "BEL_", "HAL_", "DAW_", "DUS_", "HAZ_", "BEA_", "COR_", "SPO_", "ZON_", "PLA_",
                "LE_", "IO_", "RA_", "GA_", "COM_", "EO_", "RIF_", "ZOO_", "WAR_", "NAS_"
            };

            string[] correctLetters = {
                "N","Y","R","N","S","A","D","T","G","B",
                "A","S","E","X","N","A","A","L","S","L",
                "T","O","N","K","E","M","E","T","E","N",
                "O","O","Y","S","A","N","T","M","P","A"
            }; // Correct letters to fill the blanks

                int index = Random.Range(0, words.Length); // Pick a random word
                puzzleText.text = $"Complete the word: {words[index]}"; // Display the word with the blank
                correctAnswer = correctLetters[index][0]; // Store the correct answer as a single character
            }
            else if (currentDifficulty == Difficulty.Hard)
            {
                // Hard difficulty: Words with two blanks
                string[] words = {
                    "PL__ET",  // PLANET
                    "GA__AXY", // GALAXY
                    "CO__MET", // COMET
                    "ME__OR",  // METEOR
                    "AS__ROID",// ASTEROID
                    "NE__ULA", // NEBULA
                    "RO__KET", // ROCKET
                    "CR__TER", // CRATER
                    "OR__BIT", // ORBIT
                    "GR__VITY",// GRAVITY
                    "AU__ORA", // AURORA
                    "SA__URN", // SATURN
                    "JU__ITER",// JUPITER
                    "NE__TUNE",// NEPTUNE
                    "EA__RTH", // EARTH
                    "VE__US",  // VENUS
                    "UR__NUS", // URANUS
                    "AL__IEN", // ALIEN
                    "SP__ACE", // SPACE
                    "EA__GLE"  // EAGLE (as in lunar module “Eagle”)
                };

                string[] correctLetterPairs = {
                    "AN", // PL AN ET
                    "LA", // GA LA XY
                    "OM", // CO OM ET
                    "TE", // ME TE OR
                    "TE", // AS TE ROID
                    "BE", // NE BE ULA
                    "OC", // RO OC KET
                    "AT", // CR AT ER
                    "BI", // OR BI T
                    "RA", // GR RA VITY
                    "UR", // AU UR ORA
                    "AT", // SA AT URN
                    "UP", // JU UP ITER
                    "PT", // NE PT UNE
                    "AR", // EA AR TH
                    "NU", // VE NU S
                    "AN", // UR AN US
                    "IE", // AL IE N
                    "AC", // SP AC E
                    "AG"  // EA AG LE
                };// Correct two-letter combinations

                int index = Random.Range(0, words.Length); // Pick a random word
                puzzleText.text = $"Complete the word: {words[index]}"; // Display the word with blanks
                correctAnswer = correctLetterPairs[index][0]; // Store the correct answer (only for correct tagging in rocks)
                correctWordAnswer = correctLetterPairs[index]; // Store the two-letter answer
            }

            puzzleText.gameObject.SetActive(true); // Show the puzzle text
        }
        else
        {
            Debug.LogError("PuzzleText is not assigned in the Inspector.");
        }
    }



    public void HidePuzzleText()
    {
        if (puzzleText != null)
        {
            puzzleText.gameObject.SetActive(false);
        }

        healthBarObject.SetActive(true);
        healthBarTxt.text = "Health: ";
    }

    void SpawnRocks()
    {
        if (rockPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Rock Prefab or Spawn Point not assigned.");
            return;
        }

        // Clear the list of spawned rocks
        spawnedRocks.Clear();

        // Determine the correct answer position
        int correctAnswerIndex = Random.Range(0, numberOfRocks);

        // Create a set to track unique answers
        HashSet<string> usedAnswers = new HashSet<string>();

        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 spawnPosition = spawnPoint.position + new Vector3(0, i * spacing, 0);
            GameObject spawnedRock = Instantiate(rockPrefab, spawnPosition, Quaternion.identity, spawnStart);
            spawnedRock.SetActive(true);
            spawnedRocks.Add(spawnedRock);

            RockDespawner rockDespawner = spawnedRock.AddComponent<RockDespawner>();
            rockDespawner.InitializePattern(this);

            TextMeshProUGUI rockText = spawnedRock.GetComponentInChildren<TextMeshProUGUI>();
            if (rockText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found in the spawned rock.");
            }
            else
            {
                // Initialize the answerText variable
                string answerText = string.Empty;

                // Check if it's a word puzzle
                if (puzzleText.text.Contains("Complete the word"))
                {
                    if (i == correctAnswerIndex)
                    {
                        // Assign the correct word answer (single or two letters)
                        answerText = currentDifficulty == Difficulty.Hard ? correctWordAnswer : ((char)correctAnswer).ToString();
                        spawnedRock.tag = "CorrectAnswer";
                    }
                    else
                    {
                        if (currentDifficulty == Difficulty.Easy)
                        {
                            // Generate a single random letter
                            do
                            {
                                answerText = ((char)Random.Range(65, 91)).ToString(); // Random letter (A-Z)
                            } while (usedAnswers.Contains(answerText));
                        }
                        else if (currentDifficulty == Difficulty.Hard)
                        {
                            // Generate two random letters
                            do
                            {
                                char letter1 = (char)Random.Range(65, 91); // Random A-Z
                                char letter2 = (char)Random.Range(65, 91); // Random A-Z
                                answerText = $"{letter1}{letter2}"; // Combine into a two-letter string
                            } while (usedAnswers.Contains(answerText));
                        }
                    }
                }
                else // Math puzzle
                {
                    if (i == correctAnswerIndex)
                    {
                        // Correct number answer
                        answerText = correctAnswer.ToString();
                        spawnedRock.tag = "CorrectAnswer";
                    }
                    else
                    {
                        // Generate a random number that hasn't been used
                        do
                        {
                            answerText = Random.Range(1, 20).ToString();
                        } while (usedAnswers.Contains(answerText));
                    }
                }

                rockText.text = answerText; // Set the text on the rock
                usedAnswers.Add(answerText); // Add the answer to the set of used answers
            }
        }
    }







    public void OnRockDestroyed(GameObject rock)
    {
        // Remove the rock from the list of spawned rocks
        spawnedRocks.Remove(rock);

        Debug.Log("Spawned rocks count " + spawnedRocks.Count);

        // If all rocks are destroyed, hide the puzzle text
        if (spawnedRocks.Count == 8)
        {
            Debug.Log("YES THIS IS HAPPENING");
            HidePuzzleText();
            random.GetComponent<RandomRockSpawner>().begin();
            Debug.Log("Yep getting here too");
            this.enabled = false;
        }
    }
}