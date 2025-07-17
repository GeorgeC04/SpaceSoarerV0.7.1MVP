using UnityEngine;

public class SpaceshipJiggle : MonoBehaviour
{
    public float jiggleAmount = 0.05f; // The intensity of the jiggle (how far the spaceship moves)
    public float jiggleSpeed = 10f;    // The speed/frequency of the jiggle (higher = faster)
    
    private Vector3 initialPosition;   // The starting position of the spaceship

    void Start()
    {
        // Store the spaceship's initial position so we can return to it
        initialPosition = transform.position;
    }

    void Update()
    {
        // Apply a high-frequency, low-displacement jiggle using sine wave and perlin noise
        float jiggleX = Mathf.Sin(Time.time * jiggleSpeed) * jiggleAmount;
        float jiggleY = Mathf.PerlinNoise(Time.time * jiggleSpeed, 0) * jiggleAmount;
        float jiggleZ = Mathf.Cos(Time.time * jiggleSpeed) * jiggleAmount;

        // Add the jiggle to the spaceship's position
        transform.position = initialPosition + new Vector3(jiggleX, jiggleY, jiggleZ);
    }
}
