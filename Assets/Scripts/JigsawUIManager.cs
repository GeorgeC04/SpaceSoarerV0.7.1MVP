using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class JigsawUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;       
    public TMP_Text puzzleScoreText; 
    public TMP_Text livesText;

    public Difficulty difficulty = Difficulty.Easy;    

    [Header("Puzzle Settings")]
    public float puzzleDuration ;  
    public int rewardPoints ;     // points for success
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
   

    // 2) Lives: lose one life on failure, keep same on success
    int finalLives = baseLives + (success ? 0 : -1);
    finalLives = Mathf.Max(0, finalLives);

    // 3) Persist score + lives

    PlayerPrefs.SetInt  ("SavedHealth", finalLives);

    // 4) --- NEW: bump the puzzle multiplier on success ---
    float m = PlayerPrefs.GetFloat("SavedMultiplier", 1f);
        if (success)
        {
            difficulty = GameSettings.SelectedDifficulty;
            if (difficulty == Difficulty.Easy)
            {
                m += 0.5f;
            }
            else
            {
                m += 1.0f;
            }
        }
        
    PlayerPrefs.SetFloat("SavedMultiplier", m);

    PlayerPrefs.Save();

    // 5) Back to main game
    SceneManager.LoadScene(gameSceneName);
}

}
