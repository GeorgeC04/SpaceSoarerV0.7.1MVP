using UnityEngine;

public class ResolutionBootstrap : MonoBehaviour
{
    const string ResolutionPrefKey = "ResolutionIndex";

    void Awake()
    {
        // assume your MenuController built the same "resolutions" array
        // You could duplicate its code here, or just hardcode the common ones.
        Resolution[] all = Screen.resolutions;
        int idx = PlayerPrefs.GetInt(ResolutionPrefKey, -1);
        if (idx >= 0 && idx < all.Length)
        {
            var r = all[idx];
            Screen.SetResolution(r.width, r.height, Screen.fullScreen);
        }
    }
}
