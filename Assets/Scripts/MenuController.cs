using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Text.RegularExpressions;
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

    const string ResolutionPrefKey = "ResolutionIndex";





    private void Start()
    {
        // 1) Grab all supported resolutions and build the dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        var options = new List<string>(resolutions.Length);

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var r = resolutions[i];
            options.Add($"{r.width} x {r.height}");

            if (r.width == Screen.width && r.height == Screen.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);

        // 2) Pick up any saved index (or fall back to whatever we detected above)
        int savedIndex = PlayerPrefs.GetInt(ResolutionPrefKey, currentResolutionIndex);

        // 3) Apply it to the dropdown UI…
        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();

        // 4) …and immediately apply the game window to that resolution
        SetResolution(savedIndex);

        // 5) Finally, wire up future changes so we both save + apply
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }




    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // persist it!
        PlayerPrefs.SetInt(ResolutionPrefKey, resolutionIndex);
        PlayerPrefs.Save();
    }


    public void OnStartGamePressed()
    {


        string playerName;

        //Checks the user has input a name, otherwise provides a generic name instead of failure
        try
        {

            playerName = playerNameInput.text; // Get the player's name
            //Checks the name is only a letter or number, to prevent code injection
            if (!IsValidPlayerName(playerName))
            {
                playerName = "User";

            }
            
        }
        catch (Exception)
        {
            playerName = "User";
        }



        // Save the player's name if necessary
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.DeleteKey("SavedScore");
        PlayerPrefs.DeleteKey("SavedHealth");
        PlayerPrefs.SetFloat("SavedMultiplier", 1f);

        PlayerPrefs.Save();
        // Set the current difficulty from GameSettings
        Difficulty selectedDifficulty = GameSettings.SelectedDifficulty;
        Debug.Log($"Game started with difficulty: {selectedDifficulty}");
        SceneManager.LoadScene("NewGame");
        Debug.Log("Player's Name: " + PlayerPrefs.GetString("PlayerName"));


    }

    //Basic function designed to check user input values are only letter or numbers, preventing code injection
    public static bool IsValidPlayerName(string playerName)
    {
        // Returns true only if playerName contains only letters and numbers
        return Regex.IsMatch(playerName, @"^[a-zA-Z0-9]+$");
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


        PlayerPrefs.SetInt("masterFullscreen", (_isFullScreen ? 1 : 0));
        Screen.fullScreen = _isFullScreen;

        StartCoroutine(ConfirmationBox());


    }
    public void loadLevel()
    {
        SceneManager.LoadScene("NewGame");
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MainMenu");
;    }


}

