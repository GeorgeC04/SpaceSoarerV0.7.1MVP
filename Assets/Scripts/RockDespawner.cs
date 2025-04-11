using UnityEngine;

public class RockDespawner : MonoBehaviour
{

    private float screenLeft;
    private MathsPuzzleManager MathsPuzzleManager;

    public void Initialize(MathsPuzzleManager manager)
    {
        MathsPuzzleManager = manager;
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
            Debug.Log("Yep the rock despawner is being callled");
            Destroy(gameObject); // Destroy the rock
        }
    }
}
