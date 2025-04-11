using UnityEngine;

public class PlayerSkinLoader : MonoBehaviour
{
    public Sprite[] playerSkins; // Array of player skins, similar to customization screen
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Load the saved skin index
        int selectedSkin = PlayerPrefs.GetInt("SelectedSkin", 0); // Default to skin 0 if none is saved
        spriteRenderer.sprite = playerSkins[selectedSkin]; // Set the player sprite to the saved skin
    }
}
