using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreCounter : MonoBehaviour
{
    public TMP_Text scoreText;     // Reference to the Text component
    public float score = 0f;       // Track the score
    public float scoreSpeed = 1f;  // Speed at which the score increases

    private string playerName;     // Store the player's name
    [Header("Auto-Puzzle Settings")]
    public float puzzleInterval = 100f;          // fire every 100 points
    private int puzzlesTriggered = 0;            // how many intervals we've already passed
    public PuzzleLauncher puzzleLauncher; 

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
    }

    void Update()
    {
        // Increase the score over time
        score += Time.deltaTime * scoreSpeed;

        // Update the score text
        scoreText.text = "Score: " + Mathf.FloorToInt(score);

        // int intervals = Mathf.FloorToInt(score / puzzleInterval);

        // // 3) if we've crossed a new one, trigger once per interval
        // if (intervals > puzzlesTriggered)
        // {
        //     puzzlesTriggered = intervals;

        //     if (puzzleLauncher != null)
        //         puzzleLauncher.LaunchPuzzle();
        //     else
        //         Debug.LogWarning("PuzzleLauncher not assigned on ScoreCounter!");
        // }
    }

    public void EndGame()
    {
        // Check if the current score is a high score and update the top 5 list
        SaveHighScore(score, playerName);

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
}
