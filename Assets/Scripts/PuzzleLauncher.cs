using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleLauncher : MonoBehaviour
{
    public ScoreCounter scoreCounter;
    public healthBar healthBar;    // <-- assign in Inspector

    public void LaunchPuzzle()
    {
        // Save the score
        if (scoreCounter != null)
            PlayerPrefs.SetFloat("SavedScore", scoreCounter.score);

        // Save the health
        if (healthBar != null)
            PlayerPrefs.SetInt("SavedHealth", healthBar.CurrentHealth);

        PlayerPrefs.Save();

        SceneManager.LoadScene("JigsawPuzzle");
    }
}
