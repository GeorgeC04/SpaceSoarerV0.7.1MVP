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
    public float puzzleInterval ;          // fire every 100 points
    private int puzzlesTriggered;            // how many intervals we've already passed
    public PuzzleLauncher puzzleLauncher;

    [Header("Level Up Settings")]
    public TMP_Text levelUpText;
    public CanvasGroup levelUpCanvas;
    public AudioSource audioSource;
    public AudioClip levelUpSound;

    public int levelUpEvery = 50; // Every 100 points
    private int nextLevelUpScore = 50;
    public int level = 1; 

    private bool isShowingLevelUp = false;

    private int lastLevelUpScore = 0;
    private float multiplier;


    void Start()
    {
        // 1) Ensure the game isn't paused
        Time.timeScale = 1f;

        // 2) Load the player's name
        playerName = PlayerPrefs.GetString("PlayerName", "Unknown");

        // 3) Restore a saved score if we just returned from the puzzle…
        if (PlayerPrefs.HasKey("SavedScore"))
        {
            score = PlayerPrefs.GetFloat("SavedScore");
            PlayerPrefs.DeleteKey("SavedScore");  // so that a brand‐new run won’t reuse it
        }
        else
        {
            // …otherwise start at zero for a fresh game
            score = 0f;
        }
        multiplier = PlayerPrefs.GetFloat("SavedMultiplier");
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            level = PlayerPrefs.GetInt("SavedLevel");
            PlayerPrefs.DeleteKey("SavedLevel");
        }
        else
        {
            level = 2;
        }
        puzzlesTriggered = Mathf.FloorToInt(score / puzzleInterval);
        
    }

    void Update()
    {
        // Increase the score over time
        score += Time.deltaTime * scoreSpeed;

        // Update the score text
        int intScore = Mathf.FloorToInt(score);
       scoreText.text = "SCORE: " + Mathf.Round(intScore * multiplier).ToString();


        float mult = PlayerPrefs.GetFloat("SavedMultiplier");

        multiplierText.text = "MULTIPLIER: " +  $"x{mult:0.0}";
        
        int intervals = Mathf.FloorToInt(score / puzzleInterval);

            // 3) if we've crossed a new one, trigger once per interval
            if (intervals > puzzlesTriggered)
            {
                puzzlesTriggered = intervals;

                if (puzzleLauncher != null)
                    puzzleLauncher.LaunchPuzzle();
                else
                    Debug.LogWarning("PuzzleLauncher not assigned on ScoreCounter!");
            }

        // Trigger Level Up every X points
            intScore = Mathf.FloorToInt(score);

        if (intScore >= lastLevelUpScore + levelUpEvery)
        {   
            //levelUpEvery = (levelUpEvery * 2) - 19; If we want to increase the amount of score it takes to level up.
            lastLevelUpScore += levelUpEvery;
            StartCoroutine(ShowLevelUp());
            if (levelUpSound != null && audioSource != null)
                audioSource.PlayOneShot(levelUpSound);
        }


    }


    public void EndGame()
    {
        // Check if the current score is a high score and update the top 5 list
        float finalScore = score * PlayerPrefs.GetFloat("SavedMultiplier");
        SaveHighScore(finalScore, playerName);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");

    }
    public void EndGamePuzzle(float score, float multiplier, string playerName)
    {
        // Check if the current score is a high score and update the top 5 list
        float finalScore = score * multiplier;
        SaveHighScore(finalScore, playerName);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");

    }

    public void SaveHighScore(float newScore, string newPlayerName)
    {
        List<(float score, string player)> highScores = new List<(float, string)>();

        for (int i = 0; i < 5; i++)
        {
            highScores.Add((
                PlayerPrefs.GetFloat("HighScore" + i, 0f),
                PlayerPrefs.GetString("HighScorePlayer" + i, "Unknown")
            ));
        }

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
        isShowingLevelUp = true;
        levelUpText.gameObject.SetActive(true);

        float duration = 0.5f;
        float pauseTime = 0.3f;
        float fadeOutDuration = 0.5f;

        Vector3 startPos = levelUpText.transform.localPosition + new Vector3(0, -30f, 0);
        Vector3 midPos = levelUpText.transform.localPosition;
        Vector3 endPos = midPos + new Vector3(0, -30f, 0);

        levelUpText.transform.localPosition = startPos;
        levelUpCanvas.alpha = 0f;

        float flashSpeed = 4f;  // Number of flashes per second

        // Fade in and move up with flashing
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            levelUpText.transform.localPosition = Vector3.Lerp(startPos, midPos, progress);

            // Flash alpha: oscillate between 0 and 1 quickly
            float flashAlpha = Mathf.Abs(Mathf.Sin(t * Mathf.PI * flashSpeed));
            levelUpCanvas.alpha = flashAlpha;

            yield return null;
        }

        // Pause with flashing
        float pauseTimer = 0f;
        while (pauseTimer < pauseTime)
        {
            pauseTimer += Time.deltaTime;

            float flashAlpha = Mathf.Abs(Mathf.Sin(pauseTimer * Mathf.PI * flashSpeed));
            levelUpCanvas.alpha = flashAlpha;

            yield return null;
        }

        // Fade out and move down with flashing
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float progress = t / fadeOutDuration;
            levelUpText.transform.localPosition = Vector3.Lerp(midPos, endPos, progress);

            // Flash alpha combined with fade out
            float baseAlpha = Mathf.Lerp(1f, 0f, progress);
            float flashAlpha = Mathf.Abs(Mathf.Sin(t * Mathf.PI * flashSpeed));
            levelUpCanvas.alpha = baseAlpha * flashAlpha;

            yield return null;
        }

        levelUpText.gameObject.SetActive(false);
        isShowingLevelUp = false;
        level++;
        
        PlayerPrefs.SetInt("SavedLevel", level);
        PlayerPrefs.Save();
    }



}
