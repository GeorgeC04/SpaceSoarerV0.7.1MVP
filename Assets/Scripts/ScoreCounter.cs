using System.Collections;
using System.Collections.Generic;
using Codice.CM.WorkspaceServer.Tree;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreCounter : MonoBehaviour
{
    public TMP_Text scoreText;     // Reference to the Text component
    public TMP_Text multiplierText;
    public float score = 0f;       // Track the score
    public float scoreSpeed = 1f;  // Speed at which the score increases

    private string playerName;     // Store the player's name
    [Header("Auto-Puzzle Settings")]
    public float puzzleInterval;          // fire every 100 points
    private int puzzlesTriggered;         // how many intervals we've already passed
    public PuzzleLauncher puzzleLauncher;

    [Header("Level Up Settings")]
    public TMP_Text levelUpText;
    public CanvasGroup levelUpCanvas;
    public AudioSource audioSource;
    public AudioClip levelUpSound;

    public int levelUpEvery = 50; // points per level
    public int level = 1;         // current level

    // NEW: next score threshold to level up
    private int nextLevelUpScore;

    private float multiplier;

    public Difficulty currentDifficulty = Difficulty.Easy;
    public RandomRockSpawner randRockSpawn;

    void Start()
    {
        currentDifficulty = GameSettings.SelectedDifficulty;
        Time.timeScale = 1f;
        playerName = PlayerPrefs.GetString("PlayerName", "Unknown");

        if (PlayerPrefs.HasKey("SavedScore"))
        {
            score = PlayerPrefs.GetFloat("SavedScore");
            PlayerPrefs.DeleteKey("SavedScore");
        }
        else score = 0f;

        multiplier = PlayerPrefs.GetFloat("SavedMultiplier");

        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            level = PlayerPrefs.GetInt("SavedLevel");
            PlayerPrefs.DeleteKey("SavedLevel");
        }
        else
        {
            level = 1;
        }

        // INIT next threshold
        nextLevelUpScore = level * levelUpEvery;

        puzzlesTriggered = Mathf.FloorToInt(score / puzzleInterval);
    }

    void Update()
    {
        // Increase the score over time
        score += Time.deltaTime * scoreSpeed;

        // Update UI
        int intScore = Mathf.FloorToInt(score);
        scoreText.text = "SCORE: " + Mathf.Round(intScore * multiplier).ToString();
        multiplierText.text = "MULTIPLIER: " + $"x{PlayerPrefs.GetFloat("SavedMultiplier"):0.0}";

        // Auto-puzzle
        int intervals = Mathf.FloorToInt(score / puzzleInterval);
        if (intervals > puzzlesTriggered)
        {
            puzzlesTriggered = intervals;
            if (puzzleLauncher != null)
            {
                puzzlesTriggered = intervals;

                if (puzzleLauncher != null)
                {
                    puzzleLauncher.LaunchPuzzle();
                    if (currentDifficulty == Difficulty.Easy)
                    {
                        randRockSpawn.rockSpeed += 0.001f;
                    }
                    else
                    {
                        randRockSpawn.rockSpeed += 0.40f;
                        randRockSpawn.spawnInterval -= 10.0f;
                    }

                }


                else
                {
                    randRockSpawn.rockSpeed += 0.40f;
                    randRockSpawn.spawnInterval += 0.0005f;
                }
            }
            else Debug.LogWarning("PuzzleLauncher not assigned on ScoreCounter!");
        }

        // —— REPLACED LEVEL-UP LOGIC ——  
        // bump level one step at a time if score crosses multiple thresholds
        while (intScore >= nextLevelUpScore)
        {
            level++;
            nextLevelUpScore += levelUpEvery;

            // persist new level
            PlayerPrefs.SetInt("SavedLevel", level);
            PlayerPrefs.Save();

            StartCoroutine(ShowLevelUp());
            if (levelUpSound != null && audioSource != null)
                audioSource.PlayOneShot(levelUpSound);
        }
    }

    public void EndGame()
    {
        float finalScore = score * PlayerPrefs.GetFloat("SavedMultiplier");
        SaveHighScore(finalScore, playerName);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void EndGamePuzzle(float score, float multiplier, string playerName)
    {
        float finalScore = score * multiplier;
        SaveHighScore(finalScore, playerName);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveHighScore(float newScore, string newPlayerName)
    {
        List<(float score, string player)> highScores = new List<(float, string)>();
        for (int i = 0; i < 5; i++)
            highScores.Add((
                PlayerPrefs.GetFloat("HighScore" + i, 0f),
                PlayerPrefs.GetString("HighScorePlayer" + i, "Unknown")
            ));

        highScores.Add((newScore, newPlayerName));
        highScores.Sort((x, y) => y.score.CompareTo(x.score));

        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetFloat("HighScore" + i, highScores[i].score);
            PlayerPrefs.SetString("HighScorePlayer" + i, highScores[i].player);
        }
        PlayerPrefs.Save();
    }

    private IEnumerator ShowLevelUp()
    {
        levelUpText.text = "LEVEL " + level;
        levelUpText.gameObject.SetActive(true);
        levelUpCanvas.alpha = 0f;

        float duration = 0.5f, pauseTime = 0.3f, fadeOut = 0.5f;
        Vector3 start = levelUpText.transform.localPosition + Vector3.down * 30f;
        Vector3 mid   = levelUpText.transform.localPosition;
        Vector3 end   = mid + Vector3.down * 30f;
        levelUpText.transform.localPosition = start;

        float flash = 4f, t = 0f;
        // fade in
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            levelUpText.transform.localPosition = Vector3.Lerp(start, mid, p);
            levelUpCanvas.alpha = Mathf.Abs(Mathf.Sin(t * Mathf.PI * flash));
            yield return null;
        }
        // pause
        float pt = 0f;
        while (pt < pauseTime)
        {
            pt += Time.deltaTime;
            levelUpCanvas.alpha = Mathf.Abs(Mathf.Sin(pt * Mathf.PI * flash));
            yield return null;
        }
        // fade out
        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            float p = t / fadeOut;
            levelUpText.transform.localPosition = Vector3.Lerp(mid, end, p);
            float baseA = Mathf.Lerp(1f, 0f, p);
            levelUpCanvas.alpha = baseA * Mathf.Abs(Mathf.Sin(t * Mathf.PI * flash));
            yield return null;
        }

        levelUpText.gameObject.SetActive(false);
    }
}
