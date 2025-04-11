using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{


    

    public static bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void pauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void resumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}
