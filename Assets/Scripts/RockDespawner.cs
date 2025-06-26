using UnityEngine;

public class RockDespawner : MonoBehaviour
{

    private float screenLeft;
    private MathsPuzzleManager MathsPuzzleManager;

    private ColourPattern ColourPattern;


    public void Initialize(MathsPuzzleManager manager)
    {
        MathsPuzzleManager = manager;
        Debug.Log("Maths puzzle manager is... " + MathsPuzzleManager);
    }

    public void InitializePattern(ColourPattern patternManager)
    {
        ColourPattern = patternManager;
        Debug.Log("Colour pattern is... " + ColourPattern);
    }

    void Start()
    {
        // Calculate the screen's left boundary in world space
        screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
    }

    void Update()
    {
        
        // Check if the rock has moved past the left boundary
        if (transform.position.x < screenLeft)
        {
            if (MathsPuzzleManager != null)
            {
                Debug.Log("Yep the rock despawner is being callled");
                MathsPuzzleManager.OnRockDestroyed(gameObject);
            }
            Debug.Log("Yep the rock despawner is being called here");
            Destroy(gameObject); // Destroy the rock
        }
        

        // Check if the rock has moved past the left boundary
        if (transform.position.x < screenLeft)
        {
           // Debug.Log("Colour Pattern is... " + ColourPattern);
            if (ColourPattern != null)
            {
                Debug.Log("Yep the rock despawner is being callled");
                ColourPattern.OnRockDestroyed(gameObject);
            }
            Debug.Log("Yep the rock despawner is being called here");
            Destroy(gameObject); // Destroy the rock
        }
    }
}
