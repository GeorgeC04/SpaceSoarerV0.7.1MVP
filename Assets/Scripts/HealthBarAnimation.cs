using UnityEngine;
using System.Collections;

public class HeartPulse : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float scaleAmount = 1.1f;

    void Start()
    {
        foreach (Transform heart in transform)
        {
            StartCoroutine(PulseHeart(heart));
        }
    }

    IEnumerator PulseHeart(Transform heart)
    {
        Vector3 originalScale = heart.localScale;
        Vector3 targetScale = originalScale * scaleAmount;

        while (true)
        {
            // Scale up
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * pulseSpeed;
                heart.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            // Scale down
            t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * pulseSpeed;
                heart.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }
        }
    }
}
