using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSliderController : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;                  // Reference to AudioMixer
    public string exposedParameter = "MasterVolume"; // Exposed parameter name in AudioMixer

    [Header("UI")]
    public Slider volumeSlider;                    // Volume Slider UI
    public TextMeshProUGUI volumeLabel;             // Volume text (percentage)

    private const string VolumePrefKey = "MasterVolumePref"; // Key to save the volume value

    void Start()
    {
        // Load the saved volume from PlayerPrefs, defaulting to 0.75 (75%) if not found
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 0.75f);
        Debug.Log("Loaded saved volume: " + savedVolume);

        // Set the slider to the saved value
        volumeSlider.value = savedVolume;

        // Apply the saved volume to the AudioMixer immediately
        SetVolume(savedVolume);

        // Update the volume label with the current value (in percentage)
        UpdateVolumeLabel(savedVolume);

        // Ensure volume is applied when the user changes the slider
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Update the label dynamically as the slider changes
        volumeSlider.onValueChanged.AddListener(UpdateVolumeLabel);
    }

    public void SetVolume(float value)
    {
        // Convert the slider value to decibels (dB) for the AudioMixer
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        Debug.Log("Setting volume to: " + dB + " dB");

        // Apply the volume to the AudioMixer
        audioMixer.SetFloat(exposedParameter, dB);

        // Save the volume to PlayerPrefs
        PlayerPrefs.SetFloat(VolumePrefKey, value);
        PlayerPrefs.Save();

        // Update the label with the new value (as a percentage)
        UpdateVolumeLabel(value);
    }

    // Update the volume label (percentage) next to the slider
    private void UpdateVolumeLabel(float value)
    {
        if (volumeLabel != null)
        {
            int percent = Mathf.RoundToInt(value * 100);  // Convert to percentage
            volumeLabel.text = percent + "%";             // Display percentage as text
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from slider change event to prevent memory leaks
        volumeSlider.onValueChanged.RemoveListener(SetVolume);
        volumeSlider.onValueChanged.RemoveListener(UpdateVolumeLabel);
    }
}
