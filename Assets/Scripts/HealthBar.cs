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

    public void loseHealth(TMP_Text gameOverTxt){
        if(spaceShipLength - 1 <= 0){
            spaceShips[spaceShipLength - 1].SetActive(false);
            gameOverTxt.text = "GAME OVER!!";

            while(timer < 100.0){
                Debug.Log("Timer is... " + timer);
                timer += Time.deltaTime;
            }
            
            timer = 0;
            gameOverTxt.text = "";
            SceneManager.LoadScene(0);
        }
        else{
            spaceShips[spaceShipLength - 1].SetActive(false);
            spaceShipLength --;
        }
        
    }


}
