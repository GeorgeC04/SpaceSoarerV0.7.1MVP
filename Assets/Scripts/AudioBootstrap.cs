using UnityEngine;
using UnityEngine.Audio;

public class AudioBootstrap : MonoBehaviour
{
    public AudioMixer audioMixer;      // assign your masterMixer
    public string exposedParameter = "MasterVolume";
    const string VolumePrefKey = "MasterVolumePref";

    void Awake()
    {
        // read the saved linear value (0–1)
        float saved = PlayerPrefs.GetFloat(VolumePrefKey, 0.75f);
        // convert to dB
        float dB = Mathf.Log10(Mathf.Clamp(saved, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(exposedParameter, dB);
    }
}
