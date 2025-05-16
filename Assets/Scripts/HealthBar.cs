using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class healthBar : MonoBehaviour
{


    public GameObject[] spaceShips;
    private int spaceShipLength;

    private float timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        spaceShipLength = spaceShips.Length;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loseHealth(TMP_Text gameOverTxt)
    {
        if (spaceShipLength - 1 <= 0)
        {
            spaceShips[spaceShipLength - 1].SetActive(false);
            gameOverTxt.text = "GAME OVER!!";

            // Find the ScoreCounter component
            ScoreCounter scoreCounter = FindObjectOfType<ScoreCounter>();
            if (scoreCounter != null)
            {
                // Retrieve the current score and player name
                float currentScore = scoreCounter.score;
                string playerName = PlayerPrefs.GetString("PlayerName", "Unknown");

                // Save the high score
                scoreCounter.SaveHighScore(currentScore, playerName);
            }

            // Load the main menu scene
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            spaceShips[spaceShipLength - 1].SetActive(false);
            spaceShipLength--;
        }
    }
        private IEnumerator WaitAndLoadMainMenu()
    {
        yield return new WaitForSeconds(0.1f); // Wait for 2 seconds
        SceneManager.LoadScene(0); // Load the main menu scene
    }


}
