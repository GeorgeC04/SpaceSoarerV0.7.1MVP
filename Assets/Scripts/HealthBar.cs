using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class healthBar : MonoBehaviour
{
    [Header("Ship Icons (one per life)")]
    public GameObject[] spaceShips;

    // Internal health count
    private int currentHealth;

    // Expose so PuzzleLauncher can read it
    public int CurrentHealth => currentHealth;

    public TMP_Text gameOverTxt;

void Awake()
{
    // Don’t carry over a paused state
    Time.timeScale = 1f;

    // 1) If there’s a saved health value, restore and clear it
    if (PlayerPrefs.HasKey("SavedHealth"))
    {
        currentHealth = PlayerPrefs.GetInt("SavedHealth");
        PlayerPrefs.DeleteKey("SavedHealth");
    }
    else
    {
        // 2) Otherwise this is a brand-new run: start at full health
        currentHealth = spaceShips.Length;
    }

    RefreshDisplay();

    // 3) If already at 0, trigger game over immediately
    if (currentHealth <= 0)
    {
        Debug.Log("Health is already zero on scene load — triggering Game Over.");

        gameOverTxt.text = "GAME OVER!!";        

        var scoreCounter = FindObjectOfType<ScoreCounter>();
        if (scoreCounter != null)
        {
            float savedScore = PlayerPrefs.GetFloat("SavedScore", 0f);
            float multiplier = PlayerPrefs.GetFloat("SavedMultiplier", 1f);
            string playerName = PlayerPrefs.GetString("PlayerName", "Unknown");
            scoreCounter.EndGamePuzzle(savedScore,multiplier,playerName);
        }
        else
        {
            Debug.LogError("No ScoreCounter found in scene to trigger Game Over!");
        }
    }
}

    /// <summary>
    /// Call to lose one health—and if zero, ends the game.
    /// </summary>
    public void loseHealth(TMP_Text gameOverTxt)
    {
        currentHealth = Mathf.Max(0, currentHealth - 1);
        RefreshDisplay();

        if (currentHealth <= 0)
        {
            // Show game over text
            gameOverTxt.text = "GAME OVER!!";

            // Let ScoreCounter handle final save and scene load
            var scoreCounter = FindObjectOfType<ScoreCounter>();
            if (scoreCounter != null)
            {
                scoreCounter.EndGame();
            }
            else
            {
                Debug.LogError("No ScoreCounter found in scene!");
            }
        }
        else
        {
            // Save for when we jump into a puzzle and back
            PlayerPrefs.SetInt("SavedHealth", currentHealth);
            PlayerPrefs.Save();
        }
    }

    private void RefreshDisplay()
    {
        for (int i = 0; i < spaceShips.Length; i++)
            spaceShips[i].SetActive(i < currentHealth);
    }
}
