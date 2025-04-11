using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CharacterCustomization : MonoBehaviour
{
    public Image characterImage;   // Reference to the Image UI component
    public Sprite[] characterSkins; // Array to hold different character sprites
    private int currentSkinIndex = 0;

    void Start()
    {
        UpdateCharacterImage(); // Initialize with the first skin
    }

    public void NextSkin()
    {
        currentSkinIndex = (currentSkinIndex + 1) % characterSkins.Length;
        UpdateCharacterImage();
    }

    public void PreviousSkin()
    {
        currentSkinIndex = (currentSkinIndex - 1 + characterSkins.Length) % characterSkins.Length;
        UpdateCharacterImage();
    }

    private void UpdateCharacterImage()
    {
        characterImage.sprite = characterSkins[currentSkinIndex];
    }
    public void SaveSkinChoice()
    {
        PlayerPrefs.SetInt("SelectedSkin", currentSkinIndex); // Save the selected skin index
        PlayerPrefs.Save(); // Optional but recommended to ensure data is written
        SceneManager.LoadScene("MainMenu"); // Load the game scene
    }
}
