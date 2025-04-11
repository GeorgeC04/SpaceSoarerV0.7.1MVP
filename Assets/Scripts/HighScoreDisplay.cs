using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreDisplay : MonoBehaviour
{
    public TMP_Text highScoreText;

    void Start()
    {
        
        LoadHighScores();
    }

    
    void LoadHighScores()
    {
        string highScoreString = "Top 5 High Scores:\n";
        for (int i = 0; i < 5; i++)
        {
            float score = PlayerPrefs.GetFloat("HighScore" + i, 0);
            string player = PlayerPrefs.GetString("HighScorePlayer" + i, "Unknown");

            
            if (score > 0 || player != "Unknown")
            {
                highScoreString += (i + 1) + ". " + player + ": " + score.ToString("F0") + "\n";
            }
        }

        highScoreText.text = highScoreString;
    }

}

