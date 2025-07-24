using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.PackageManager.UI;
public enum Difficulty
{
    Easy,
    Hard
}

public class MathsPuzzleManager : MonoBehaviour
{
    public RandomRockSpawner randRockSpawn;
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


    public bool isShowingText = false;
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






    public void beginPuzzle()
    {
        //healthBarObject.SetActive(false);
        currentScore.GetComponent<ScoreCounter>().scoreText.SetText("");

        healthBarTxt.text = "";
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

        if (currentScore == null || random == null)
        {
            return;
        }



        int thisScore = Mathf.FloorToInt(currentScore.GetComponent<ScoreCounter>().score);

        if (thisScore % 7 == 0)
        {
            if (currentDifficulty == Difficulty.Easy)
            {
                randRockSpawn.rockSpeed += 0.0005f;
            }
            else
            {
                randRockSpawn.rockSpeed += 0.10f;
                randRockSpawn.spawnInterval -= 0.0003f;
            }


        }

        // Move the spawned rocks from right to left
        foreach (GameObject rock in spawnedRocks)
        {
            if (rock != null)
            {
                rock.transform.Translate(Vector3.left * (randRockSpawn.rockSpeed * 0.8f) * Time.deltaTime);
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
                StartCoroutine(ShowPuzzleTextAnimatedNotColoured());
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
                        puzzleText.text = $"{num1} x {num2} = ?";
                        break;

                    case 2:
                        correctAnswer = num1 + num2;
                        puzzleText.text = $"{num1} + {num2} = ?";
                        break;


                }

                StartCoroutine(ShowPuzzleTextAnimatedNotColoured());
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
                "SU_", "SK_", "STA_", "MOO_", "MAR_", "NOV_", "VOI_", "DUS_", "RIN_",
                "AUR_", "LEN_", "POL_", "FLU_", "SPI_", "LUN_",  "TAI_", "AXI_",
                "BEL_", "HAL_", "DAW_", "DUS_", "HAZ_", "BEA_", "COR_", "SPO_", "ZON_",
                "RA_", "GA_", "COM_", "EO_", "RIF_", "ZOO_", "WAR_", "NAS_",
                "COSM_", "BL_CK", "RO_K","L_GHT", "SP_ED"
            };

                string[] correctLetters = {
                "N","Y","R","N","S","A","D","T","G",
                "A","S","E","X","N","A","L","S",
                "T","O","N","K","E","M","E","T","E",
                "Y","S","A","N","T","M","P","A",
                "O","A","C","I","E"
            };// Correct letters to fill the blanks

                int index = Random.Range(0, words.Length); // Pick a random word
                puzzleText.text = $"Complete the word: {words[index]}"; // Display the word with the blank
                correctAnswer = correctLetters[index][0]; // Store the correct answer as a single character
            }
            else if (currentDifficulty == Difficulty.Hard)
            {
                // Hard difficulty: Words with two blanks
                string[] words = {
                    "PL__ET",  // PLANET
                    "GA__XY", // GALAXY
                    "C__ET", // COMET
                    "ME__OR",  // METEOR
                    "AS__ROID",// ASTEROID
                    "NE__LA", // NEBULA
                    "RO__ET", // ROCKET
                    "CR__ER", // CRATER
                    "OR__T", // ORBIT
                    "GR__ITY",// GRAVITY
                    "AU__RA", // AURORA
                    "SA__RN", // SATURN
                    "JU__TER",// JUPITER
                    "NE__UNE",// NEPTUNE
                    "E__TH", // EARTH
                    "VE__S",  // VENUS
                    "UR__US", // URANUS
                    "AL__N", // ALIEN
                    "SP__E", // SPACE
                    "E__LE",  // EAGLE (as in lunar module “Eagle”)
                    "SP__E",
                    "SO__ER"
                };

                string[] correctLetterPairs = {
                    "AN", // PL AN ET
                    "LA", // GA LA XY
                    "OM", // CO OM ET
                    "TE", // ME TE OR
                    "TE", // AS TE ROID
                    "BU", // NE BE ULA
                    "CK", // RO CK ET
                    "AT", // CR AT ER
                    "BI", // OR BI T
                    "AV", // GR AV ITY
                    "RO", // AU UR ORA
                    "TU", // SA TU RN
                    "PI", // JU PI TER
                    "PT", // NE PT UNE
                    "AR", // E AR TH
                    "NU", // VE NU S
                    "AN", // UR AN US
                    "IE", // AL IE N
                    "AC", // SP AC E
                    "AG",  // E AG LE
                    "AC",
                    "AR"
                }; // Correct two-letter combinations

                int index = Random.Range(0, words.Length); // Pick a random word
                puzzleText.text = $"Complete the word: {words[index]}"; // Display the word with blanks
                correctAnswer = correctLetterPairs[index][0]; // Store the correct answer (only for correct tagging in rocks)
                correctWordAnswer = correctLetterPairs[index]; // Store the two-letter answer
            }

            StartCoroutine(ShowPuzzleTextAnimatedNotColoured()); // Show the puzzle text
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

        //healthBarObject.SetActive(true);
        //healthBarTxt.text = "Health: ";
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

        // Pre-add the correct answer text so it never appears again
        string correctAnswerText;
        if (puzzleText.text.Contains("Complete the word"))
            correctAnswerText = currentDifficulty == Difficulty.Hard
                ? correctWordAnswer
                : ((char)correctAnswer).ToString();
        else
            correctAnswerText = correctAnswer.ToString();
        usedAnswers.Add(correctAnswerText);

        int xVector = 0;

        if (currentDifficulty == Difficulty.Hard)
        {
            int userScore = Mathf.FloorToInt(currentScore.GetComponent<ScoreCounter>().score);

            if (userScore < 300)
                xVector = 500;
            if (userScore < 600 && userScore > 300)
                xVector = 700;
            if (userScore < 1000 && userScore > 600)
                xVector = 100;
            if (userScore > 1000)
                xVector = userScore + 400;
            if (userScore > 2000)
                xVector = userScore * 2;
        }
        else
        {
            xVector = 150;
        }

        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 spawnPosition = spawnPoint.position + new Vector3(xVector, i * spacing, 0);
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
                string answerText = string.Empty;

                // Word puzzle branch
                if (puzzleText.text.Contains("Complete the word"))
                {
                    if (i == correctAnswerIndex)
                    {
                        answerText = correctAnswerText;
                        spawnedRock.tag = "CorrectAnswer";
                    }
                    else
                    {
                        // generate unique wrong answer
                        do
                        {
                            if (currentDifficulty == Difficulty.Easy)
                                answerText = ((char)Random.Range(65, 91)).ToString();
                            else
                            {
                                char l1 = (char)Random.Range(65, 91);
                                char l2 = (char)Random.Range(65, 91);
                                answerText = $"{l1}{l2}";
                            }
                        }
                        while (usedAnswers.Contains(answerText));
                        spawnedRock.tag = "Rock";
                    }
                }
                // Math puzzle branch
                else
                {
                    if (i == correctAnswerIndex)
                    {
                        answerText = correctAnswerText;
                        spawnedRock.tag = "CorrectAnswer";
                    }
                    else
                    {
                        // generate unique wrong number
                        do
                        {
                            answerText = Random.Range(1, 20).ToString();
                        }
                        while (usedAnswers.Contains(answerText));
                        spawnedRock.tag = "Rock";
                    }
                }

                rockText.text = answerText;
                usedAnswers.Add(answerText); // mark it used
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
    

    private IEnumerator ShowPuzzleTextAnimated()
    {
        isShowingText = true;
        puzzleText.gameObject.SetActive(true);

        RectTransform rect = puzzleText.GetComponent<RectTransform>();

        // Set fixed start position here, so it resets every time
        Vector2 fixedStartPos = new Vector2(-12.7959f, 1f);
        rect.anchoredPosition = fixedStartPos;

        Vector2 midPos = fixedStartPos;
        Vector2 startPos = midPos + new Vector2(0f, -30f);
        Vector2 endPos = midPos + new Vector2(0f, -30f);

        // Immediately move to startPos (30 units below fixed start)
        rect.anchoredPosition = startPos;

        float duration = 0.5f;
        float pauseTime = 1.5f;
        float fadeOutDuration = 0.5f;

        CanvasGroup puzzleCanvas = puzzleText.GetComponent<CanvasGroup>();
        if (puzzleCanvas == null) puzzleCanvas = puzzleText.gameObject.AddComponent<CanvasGroup>();
        puzzleCanvas.alpha = 0f;

        float flashSpeed = 4f;

        // Fade in + move up
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            rect.anchoredPosition = Vector2.Lerp(startPos, midPos, progress);
            puzzleCanvas.alpha = Mathf.Abs(Mathf.Sin(t * Mathf.PI * flashSpeed));
            yield return null;
        }

        // Pause
        float pauseTimer = 0f;
        while (pauseTimer < pauseTime)
        {
            pauseTimer += Time.deltaTime;
            puzzleCanvas.alpha = Mathf.Abs(Mathf.Sin(pauseTimer * Mathf.PI * flashSpeed));
            yield return null;
        }

        // Fade out + move down
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float progress = t / fadeOutDuration;
            rect.anchoredPosition = Vector2.Lerp(midPos, endPos, progress);
            float baseAlpha = Mathf.Lerp(1f, 0f, progress);
            float flashAlpha = Mathf.Abs(Mathf.Sin(t * Mathf.PI * flashSpeed));
            puzzleCanvas.alpha = baseAlpha * flashAlpha;
            yield return null;
        }

        puzzleText.gameObject.SetActive(false);
        isShowingText = false;
    }




     private IEnumerator ShowPuzzleTextAnimatedNotColoured()
    {
        isShowingText = true;
        puzzleText.gameObject.SetActive(true);

        float duration = 0.5f;
        float pauseTime = 3f;
        float fadeOutDuration = 0.5f;

        Vector3 startPos = puzzleText.transform.localPosition + new Vector3(0, -30f, 0);
        Vector3 midPos = puzzleText.transform.localPosition;
        Vector3 endPos = midPos + new Vector3(0, -30f, 0);

        puzzleText.transform.localPosition = startPos;

        CanvasGroup puzzleCanvas = puzzleText.GetComponent<CanvasGroup>();
        puzzleCanvas.alpha = 0f;

        float flashSpeed = 4f;

        // Fade in + move up with flashing
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            puzzleText.transform.localPosition = Vector3.Lerp(startPos, midPos, progress);
            yield return null;
        }

        // Pause + flashing
        float pauseTimer = 0f;
        while (pauseTimer < pauseTime)
        {
            pauseTimer += Time.deltaTime;
            puzzleCanvas.alpha = Mathf.Abs(Mathf.Sin(pauseTimer * Mathf.PI * flashSpeed));
            yield return null;
        }

        // Fade out + move down
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float progress = t / fadeOutDuration;
            puzzleText.transform.localPosition = Vector3.Lerp(midPos, endPos, progress);
            float baseAlpha = Mathf.Lerp(1f, 0f, progress);
            float flashAlpha = Mathf.Abs(Mathf.Sin(t * Mathf.PI * flashSpeed));
            puzzleCanvas.alpha = baseAlpha * flashAlpha;
            yield return null;
        }

        puzzleText.gameObject.SetActive(false);
        isShowingText = false;
    }



}


