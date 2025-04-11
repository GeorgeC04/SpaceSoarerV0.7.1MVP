using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MenuController : MonoBehaviour
{

    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private int defaultVolume = 100;

    private bool _isFullScreen;
    




    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("New Game")]
    [SerializeField] public TMP_InputField playerNameInput;
    public string _newGameLevel;



    [Header("Resolutions Dropdowns")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;





    private void Start()
    {
        //PlayerPrefs.DeleteAll();

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for(int i = 0; i  < resolutions.Length ; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            } 
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }


    

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

    }
    public void OnStartGamePressed()
    {
        string playerName = playerNameInput.text; // Get the player's name

        // Save the player's name if necessary
        PlayerPrefs.SetString("PlayerName", playerName);

        PlayerPrefs.Save();
        // Set the current difficulty from GameSettings
        Difficulty selectedDifficulty = GameSettings.SelectedDifficulty;
        Debug.Log($"Game started with difficulty: {selectedDifficulty}");
        SceneManager.LoadScene("NewGame");
        Debug.Log("Player's Name: " + PlayerPrefs.GetString("PlayerName"));


    }


    //this is the new game level you want to lose into


    public void NewGameDialogueYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }
    public void resetButton(string MenuType)
    {
        if (MenuType == "Sound")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0");
            VolumeApply();
        }
        if (MenuType == "Gameplay")
        {
            GameplayApply();
        }
    }



    public void quitGame()
    {
        Application.Quit();
    }
    public void setVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());

    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }

    public void GameplayApply()
    {
        StartCoroutine(ConfirmationBox());
    }


    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
    }
    public void GraphicsApply()
    {
        

        PlayerPrefs.SetInt("masterFullscreen" , (_isFullScreen ? 1 : 0));
        Screen.fullScreen = _isFullScreen;

        StartCoroutine(ConfirmationBox());


    }
    public void loadLevel()
    {
        SceneManager.LoadScene("NewGame");
    }


}

