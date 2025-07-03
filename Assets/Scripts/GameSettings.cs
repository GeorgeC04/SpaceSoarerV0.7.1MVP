using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public static Difficulty SelectedDifficulty = Difficulty.Easy; // Default difficulty


    static void ChangeSetting(Difficulty newDifficulty ) {
        SelectedDifficulty = newDifficulty;
    }
}




