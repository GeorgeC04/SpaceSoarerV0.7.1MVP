using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.PackageManager.UI;
using System.Runtime.CompilerServices;

/*
public enum Difficulty
{
    Easy,
    Hard
}
*/
public class ColourPattern : MonoBehaviour
{

    public RandomRockSpawner randRockSpawn; //Spawn point for rocks
    public GameObject random; //Random spawner
    [Range(-1f, 1f)]
    public float scrollSpeed = 0.5f;
    //public float rockSpeed = 1.0f;   // Speed at which rocks move from right to left
    public float spawnDistance = 10f; // Distance between rock spawn points
    public TextMeshProUGUI puzzleText; //Puzzle text point
    public TextMeshProUGUI rockText; //Rock text point

    public GameObject rockPrefab; //Prefab / image for the rock
    public int numberOfRocks = 10; // Number of rocks to spawn
    public float spacing = 2f;    // Space between each rock
    public Transform spawnPoint; //Point rocks spawn from

    public TextMeshProUGUI healthBarTxt; //Holds the health bar text "Health: "
    public GameObject healthBarObject; //Holds the health bar info and components

    private int correctAnswer; //Holds the correct answer
    private string correctWordAnswer; //Holds the correct answer for the word

    public CanvasGroup recreatePatternCanvas; //Holds the canvas for the pattern recreation puzzle

    public bool isShowingText = false; //Boolean lets code know if it's showing 

    
    private List<GameObject> spawnedRocks = new List<GameObject>(); //holds a dynamic list with all the rocks on the page

    public Transform spawnStart;

    //public RandomRockSpawner rockSpawner;

    private int frameCounter = 0;
    public int framesPerSpawn = 600; // The number of frames between each spawn cycle


    public ScoreCounter currentScore; //Gets the current score
    
    private bool isMathPuzzle = true; // Starts with math puzzle

    public Difficulty currentDifficulty = Difficulty.Easy;


    public GameObject[] prefabs;

    public TextMeshProUGUI InitialPuzzleText;

    public GameObject plsJustWork;

    public void beginPuzzle()
    {

        spawnedRocks.Clear();



        //healthBarObject.SetActive(false);
        currentScore.GetComponent<ScoreCounter>().scoreText.SetText("");

        //healthBarTxt.text = "";

        currentDifficulty = GameSettings.SelectedDifficulty;
        Debug.Log($"Starting puzzle with difficulty: {currentDifficulty}");




        //mat = GetComponent<Renderer>().material;
        if (puzzleText != null)
        {
            puzzleText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("PuzzleText is not assigned in the Inspector.");
        }
        

        puzzleText.text = "Recreate the pattern!!";
        StartCoroutine(ShowPuzzleTextAnimated());
        



        int[] randomNumbers;

        if (currentDifficulty == Difficulty.Easy)
        {
            randomNumbers = new int[3];
        }
        else
        {
            randomNumbers = new int[4];
        }


        for (int i = 0; i < randomNumbers.Length; i++)
        {
            randomNumbers[i] = Random.Range(0, 4); // 0 to 3 inclusive
        }

        

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
            spawnedRock.tag = "PowerUp";

            RockDespawner rockDespawner = spawnedRock.AddComponent<RockDespawner>();
            
        }




        


        int xPos = 1600;
        int correctNumber;
        int xPosIncrease = 0;

        int sizeOfFor;

        if (currentDifficulty == Difficulty.Easy)
        {
            sizeOfFor = 3;

            xPosIncrease = 400;
        }
        else
        {
            sizeOfFor = 4;

            int userScore = Mathf.FloorToInt(currentScore.GetComponent<ScoreCounter>().score);


            if (userScore < 300)
            {
                xPosIncrease = 550;
            }
            if (userScore < 600 && userScore > 300)
            {
                xPosIncrease = 750;
            }
            if (userScore < 1000 && userScore > 600)
            {
                xPosIncrease = 950;
            }
            if (userScore > 1000)
            {
                xPosIncrease = (userScore + 300);
            }
            if (userScore > 2000)
            {
                xPosIncrease = (userScore + 200);
            }
            if (userScore > 3000)
            {
                xPosIncrease = (userScore + 100);
            }

        }
        
        

        for (int n = 0; n < sizeOfFor; n++)
        {

            int yPos = 150;
            xPos += xPosIncrease;
            
                
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
                else
                {
                    spawnedRock.tag = "Rock";
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
                    "GA__XY", // GALAXY
                    "CO__T", // COMET
                    "ME__OR",  // METEOR
                    "AS__ROID",// ASTEROID
                    "NE__LA", // NEBULA
                    "RO__ET", // ROCKET
                    "CR__ER", // CRATER
                    "OR__T", // ORBIT
                    "G__VITY",// GRAVITY
                    "AU__RA", // AURORA
                    "SA__RN", // SATURN
                    "JU__TER",// JUPITER
                    "NE__UNE",// NEPTUNE
                    "E__TH", // EARTH
                    "VE__S",  // VENUS
                    "UR__US", // URANUS
                    "AL__N", // ALIEN
                    "SP__E", // SPACE
                    "E__LE"  // EAGLE (as in lunar module “Eagle”)
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
        if ((currentDifficulty == Difficulty.Easy && spawnedRocks.Count == 6) || (currentDifficulty == Difficulty.Hard && spawnedRocks.Count == 8))
        {
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

    // Adjust positions to move right by adding an offset to the x-axis
    Vector2 startPos = fixedStartPos + new Vector2(50f, -30f); // Moving it 50 units to the right
    Vector2 midPos = startPos; // Mid position is same as start position
    Vector2 endPos = midPos + new Vector2(50f, -30f); // Move it 50 units more to the right at the end

    // Immediately move to startPos (50 units right and 30 units down from fixed start)
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

        Vector3 startPos = puzzleText.transform.localPosition + new Vector3(25, -30f, 0);
        Vector3 midPos = puzzleText.transform.localPosition + new Vector3(25, -30f, 0) ;
        Vector3 endPos = midPos + new Vector3(25, -30f, 0);

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