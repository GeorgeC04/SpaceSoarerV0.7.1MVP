using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class JigsawUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;        // Drag your on-screen timer TextMeshPro here
    public TMP_Text puzzleScoreText;  // Drag your on-screen score TextMeshPro here
    public TMP_Text livesText;  

    [Header("Puzzle Settings")]
    public float puzzleDuration = 30f;  // How long the player has to solve
    public int penaltyPoints   = 5; 
    public int rewardPoints = 10;     // How many points to award on success
    public string gameSceneName = "NewGame"; // Name of your main game scene

    float timeLeft;
    int baseScore;
    int baseLives;
    void Start()
    {
        // 1) Initialize the countdown
        timeLeft = puzzleDuration;

        // 2) Read the carried-in score from PlayerPrefs
        baseScore = Mathf.FloorToInt(PlayerPrefs.GetFloat("SavedScore", 0f));
        puzzleScoreText.text = "Score: " + baseScore;
        baseLives = PlayerPrefs.GetInt("SavedHealth", 
        FindObjectOfType<healthBar>()?.spaceShips.Length ?? 3);
        livesText.text = "Lives: " + baseLives;
    }

    void Update()
    {
        // Update the timer display
        timeLeft -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(timeLeft).ToString();

        // Auto-fail if time runs out
        if (timeLeft <= 0f)
            EndPuzzle(false);
    }

    /// <summary>
    /// Call this when the puzzle is solved (pass true) or failed (false).
    /// </summary>
    public void EndPuzzle(bool success)
    {
        enabled = false;

        int finalScore = baseScore;
        if (success)
        {
            finalScore += rewardPoints;
        }
        else
        {
            finalScore -= penaltyPoints;
            // Prevent negative scores
            finalScore = Mathf.Max(0, finalScore);
        }

        PlayerPrefs.SetFloat("SavedScore", finalScore);
        PlayerPrefs.SetInt("SavedHealth", baseLives);
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName);
    }
}
