using System.Collections;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private float offset;
    private Material mat;
    public Texture[] backgrounds;   // Array to store background textures
    public float changeInterval = 5f;  // Time interval between background changes
    public float fadeDuration = 1f;    // Duration of the fade effect
    private int currentBackgroundIndex = 0;
    private float timeSinceLastChange = 0f;
    private Coroutine fadeCoroutine;

    public RockSpawner rockSpawner;  // Reference to the RockSpawner script
    public RandomRockSpawner randomRockSpawner;  // Reference to the RandomRockSpawner script

    void Start()
    {
        mat = GetComponent<Renderer>().material;

        // Ensure the shader supports transparency (if using Unity Standard Shader)
        mat.SetFloat("_Mode", 2);
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        mat.renderQueue = 1000;

        mat.mainTexture = backgrounds[currentBackgroundIndex];
    }

    void Update()
    {
        offset += (Time.deltaTime * scrollSpeed) / 10f;
        mat.SetTextureOffset("_MainTex", new Vector2(offset, 0));

        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= changeInterval)
        {
            timeSinceLastChange = 0f;
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeBackground());
        }
    }

    IEnumerator FadeBackground()
    {
        int nextBackgroundIndex = (currentBackgroundIndex + 1) % backgrounds.Length;

        Color color = mat.color;
        color.a = 1f;
        mat.color = color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            color.a = 1f - normalizedTime;
            mat.color = color;
            yield return null;
        }

        mat.mainTexture = backgrounds[nextBackgroundIndex];
        currentBackgroundIndex = nextBackgroundIndex;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            color.a = normalizedTime;
            mat.color = color;
            yield return null;
        }

        color.a = 1f;
        mat.color = color;

        // Change rock themes when the background changes
        //if (rockSpawner != null)
        //{
            //rockSpawner.ChangeRockTheme(currentBackgroundIndex);
        //}

        //if (randomRockSpawner != null)
        //{
            //randomRockSpawner.ChangeRockTheme(currentBackgroundIndex);
        //}
    }
}
