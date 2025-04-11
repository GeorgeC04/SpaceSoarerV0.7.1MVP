using UnityEngine;

public class DifficultySelector : MonoBehaviour
{
    public void SetEasyDifficulty()
    {
        GameSettings.SelectedDifficulty = Difficulty.Easy;
        Debug.Log("Difficulty set to Easy");
    }

    public void SetHardDifficulty()
    {
        GameSettings.SelectedDifficulty = Difficulty.Hard;
        Debug.Log("Difficulty set to Hard");
    }
}
