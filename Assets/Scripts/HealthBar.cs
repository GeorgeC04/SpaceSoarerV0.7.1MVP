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
        // 1) Restore from PlayerPrefs if present
        if (PlayerPrefs.HasKey("SavedHealth"))
        {
            currentHealth = PlayerPrefs.GetInt("SavedHealth");
            PlayerPrefs.DeleteKey("SavedHealth");
        }
        else
        {
            // First time play: full health
            currentHealth = spaceShips.Length;
        }

        RefreshDisplay();
    }

    /// <summary>
    /// Call to lose one health—and if zero, ends the game.
    /// </summary>
    public void loseHealth(TMP_Text gameOverTxt)
    {
        // Decrement
        currentHealth = Mathf.Max(0, currentHealth - 1);
        RefreshDisplay();

        if (currentHealth <= 0)
        {
            gameOverTxt.text = "GAME OVER!!";

            // Save high score
            var scoreCounter = FindObjectOfType<ScoreCounter>();
            if (scoreCounter != null)
            {
                scoreCounter.SaveHighScore(scoreCounter.score,
                    PlayerPrefs.GetString("PlayerName", "Unknown"));
            }

            // Load main menu
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            // Optional: save intermediate health in case of puzzle launch
            PlayerPrefs.SetInt("SavedHealth", currentHealth);
            PlayerPrefs.Save();
        }
    }

    // Refreshes which ship icons are active
    private void RefreshDisplay()
    {
        for (int i = 0; i < spaceShips.Length; i++)
            spaceShips[i].SetActive(i < currentHealth);
    }
}
