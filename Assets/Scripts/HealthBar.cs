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
            gameOverTxt.text = "GAME OVER!!";
            var scoreCounter = FindObjectOfType<ScoreCounter>();
            if (scoreCounter != null)
                scoreCounter.SaveHighScore(scoreCounter.score,
                    PlayerPrefs.GetString("PlayerName", "Unknown"));

            SceneManager.LoadScene("MainMenu");
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
