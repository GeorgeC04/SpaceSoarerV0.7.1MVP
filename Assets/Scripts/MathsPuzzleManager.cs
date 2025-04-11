using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum Difficulty
{
    Easy,
    Hard
}

public class MathsPuzzleManager : MonoBehaviour
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

    private int correctAnswer;
    private string correctWordAnswer;
 
    //private bool questionDisplayed = false;
    //private bool rocksSpawned = false;
    private List<GameObject> spawnedRocks = new List<GameObject>();

    public Transform spawnStart;

    //public RandomRockSpawner rockSpawner;

    private int frameCounter = 0;
    public int framesPerSpawn = 600; // The number of frames between each spawn cycle
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



    


    public void beginPuzzle()
    {
        currentDifficulty = GameSettings.SelectedDifficulty;
        Debug.Log($"Starting puzzle with difficulty: {currentDifficulty}");
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

        // Move the spawned rocks from right to left
        foreach (GameObject rock in spawnedRocks)
        {
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
                int num1 = Random.Range(1, 10);
                int num2 = Random.Range(1, 10);
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
                int num1 = Random.Range(1, 20);
                int num2 = Random.Range(1, 20);

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
                        puzzleText.text = $"{num1} × {num2} = ?";
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
                string[] words = { "SMAL_", "DO_", "B_OK" }; // Words with one blank
                string[] correctLetters = { "L", "G", "O" }; // Correct letters to fill the blanks

                int index = Random.Range(0, words.Length); // Pick a random word
                puzzleText.text = $"Complete the word: {words[index]}"; // Display the word with the blank
                correctAnswer = correctLetters[index][0]; // Store the correct answer as a single character
            }
            else if (currentDifficulty == Difficulty.Hard)
            {
                // Hard difficulty: Words with two blanks
                string[] words = { "CO__T", "AS__ROID", "GAL__Y", "SP__E" }; // Words with two blanks
                string[] correctLetterPairs = { "ME", "TE", "AX", "AC" }; // Correct two-letter combinations

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
            rockDespawner.Initialize(this);

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
        if (spawnedRocks.Count == 1)
        {
            Debug.Log("YES THIS IS HAPPENING");
            HidePuzzleText();
            random.GetComponent<RandomRockSpawner>().begin();
            Debug.Log("Yep getting here too");
            this.enabled = false;
        }
    }
}
