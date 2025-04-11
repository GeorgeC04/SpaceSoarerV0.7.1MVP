    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;

public class ScoreCounter : MonoBehaviour
{
    public TMP_Text scoreText; // Reference to the Text component
    public float score = 0f; // Track the score
    public float scoreSpeed = 1f; // Speed at which the score increases

    private string playerName; // Store the player's name

    void Start()
    {
        // Ensure the game starts with the correct time scale
        Time.timeScale = 1f;

        // Load the player's name from PlayerPrefs
        playerName = PlayerPrefs.GetString("PlayerName", "Unknown");

        // Reset the score to 0 at the start of the new game
        score = 0f;
    }

    void Update()
    {
        // Increase the score over time
        score += Time.deltaTime * scoreSpeed;

        // Update the score text
        scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
    }

    // This method will be called when the player presses the End Game button
    public void EndGame()
    {
        // Check if the current score is a high score and update the top 5 list
        SaveHighScore(score, playerName);

        // Reset the time scale to ensure the game isn't paused when loading the main menu
        Time.timeScale = 1f;

        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }

    // Function to save a new high score if it qualifies for the top 5
    void SaveHighScore(float newScore, string newPlayerName)
    {
        // Retrieve the existing top 5 scores
        List<(float score, string player)> highScores = new List<(float, string)>();

        for (int i = 0; i < 5; i++)
        {
            float score = PlayerPrefs.GetFloat("HighScore" + i, 0);
            string player = PlayerPrefs.GetString("HighScorePlayer" + i, "Unknown");
            highScores.Add((score, player));
        }

        // Add the new score to the list
        highScores.Add((newScore, newPlayerName));

        // Sort the high scores by score in descending order
        highScores.Sort((x, y) => y.score.CompareTo(x.score));

        // Save only the top 5 scores back to PlayerPrefs
        for (int i = 0; i < 5; i++)
        {
            if (i < highScores.Count)
            {
                PlayerPrefs.SetFloat("HighScore" + i, highScores[i].score);
                PlayerPrefs.SetString("HighScorePlayer" + i, highScores[i].player);
            }
        }

        PlayerPrefs.Save();
    }
}