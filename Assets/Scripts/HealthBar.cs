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

    [Header("Game Over UI")]
    public TMP_Text gameOverTxt;
    public float    gameOverDelay = 2f;  // seconds to show text

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

        // 3) If already at 0, trigger Game Over sequence immediately
        if (currentHealth <= 0)
        {
            StartCoroutine(GameOverSequence());
        }
    }

    /// <summary>
    /// Call to lose one health—and if zero, runs Game Over.
    /// </summary>
    public void loseHealth(TMP_Text unused)
    {
        currentHealth = Mathf.Max(0, currentHealth - 1);
        RefreshDisplay();

        if (currentHealth <= 0)
        {
            StartCoroutine(GameOverSequence());
        }
        else
        {
            // Save for when we jump into a puzzle and back
            PlayerPrefs.SetInt("SavedHealth", currentHealth);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator GameOverSequence()
    {
        // 1) Show “GAME OVER!!”
        if (gameOverTxt != null)
            gameOverTxt.text = "GAME OVER!!";

        // 2) Disable player control
        var player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.enabled = false;           // stop Update() logic
            player.movementSpeed = 0f;        // ensure no new movement
            if (player.rb != null)
            {
                player.rb.velocity = Vector2.zero; 
                
            }
             // immediately clear any leftover momentum
        }


        // 3) Disable rock spawner
        var spawner = FindObjectOfType<RandomRockSpawner>();
        if (spawner != null)
        {
            spawner.enabled = false;
        }

        var colourPattern = FindObjectOfType<ColourPattern>();
        if (spawner != null)
        {
            colourPattern.enabled = false;
        }
        var MathsScript = FindObjectOfType<MathsPuzzleManager>();
        if (spawner != null)
        {
            MathsScript.enabled = false;
        }
            

        // 4) Wait real time so Game Over text is visible
        yield return new WaitForSecondsRealtime(gameOverDelay);

        // 5) Let ScoreCounter handle final save and load
        var scoreCounter = FindObjectOfType<ScoreCounter>();
        if (scoreCounter != null)
        {
            scoreCounter.EndGame();
        }
        else
        {
            Debug.LogError("No ScoreCounter found in scene!");
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void RefreshDisplay()
    {
        for (int i = 0; i < spaceShips.Length; i++)
            spaceShips[i].SetActive(i < currentHealth);
    }
}
