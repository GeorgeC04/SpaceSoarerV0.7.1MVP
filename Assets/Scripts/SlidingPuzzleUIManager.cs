using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SlidingPuzzleUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;       
    public TMP_Text puzzleScoreText; 
    public TMP_Text livesText;       

    [Header("Puzzle Settings")]
    public float puzzleDuration = 60f;  
    public int rewardPoints    = 10;     // points for success
    public string gameSceneName = "NewGame";

    float timeLeft;
    int baseScore;
    int baseLives;

    void Start()
    {
        timeLeft = puzzleDuration;

        baseScore = Mathf.FloorToInt(PlayerPrefs.GetFloat("SavedScore", 0f));
        puzzleScoreText.text = "Score: " + baseScore;

        baseLives = PlayerPrefs.GetInt("SavedHealth", 
                      FindObjectOfType<healthBar>()?.spaceShips.Length ?? 5);
        livesText.text = "Lives: " + baseLives;
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(timeLeft).ToString();

        if (timeLeft <= 0f)
            EndPuzzle(false);
    }

    /// <summary>
    /// Call this when the puzzle is solved (true) or failed (false).
    /// </summary>
    public void EndPuzzle(bool success)
    {
        enabled = false;

        // 1) Score: add points on success, leave unchanged on failure
        int finalScore = baseScore + (success ? rewardPoints : 0);

        // 2) Lives: lose one life on failure, keep same on success
        int finalLives = baseLives + (success ? 0 : -1);
        finalLives = Mathf.Max(0, finalLives);

        // 3) Persist both
        PlayerPrefs.SetFloat("SavedScore", finalScore);
        PlayerPrefs.SetInt("SavedHealth", finalLives);
        PlayerPrefs.Save();

        // 4) Back to main game
        SceneManager.LoadScene(gameSceneName);
    }
}
