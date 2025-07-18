using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleLauncher : MonoBehaviour
{
    public ScoreCounter scoreCounter;
    public healthBar    healthBar;    // assign in Inspector

    [Tooltip("List of puzzle scene names you want to randomly choose between")]
    public string[] puzzleSceneNames = new[] { "JigsawPuzzle", "SampleScene" };

    public void LaunchPuzzle()
    {
        // 1) Persist current score + health
        if (scoreCounter != null)
            PlayerPrefs.SetFloat("SavedScore", scoreCounter.score);
        if (healthBar != null)
            PlayerPrefs.SetInt("SavedHealth", healthBar.CurrentHealth);
        PlayerPrefs.Save();

        // 2) Pick a random scene from the list
        if (puzzleSceneNames == null || puzzleSceneNames.Length == 0)
        {
            Debug.LogError("No puzzleSceneNames configured on PuzzleLauncher!");
            return;
        }
        int idx = Random.Range(0, puzzleSceneNames.Length);
        string sceneToLoad = puzzleSceneNames[idx];

        // 3) Fire off the transition
        SceneManager.LoadScene(sceneToLoad);
    }
}
