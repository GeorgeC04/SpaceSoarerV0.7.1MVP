using UnityEngine;

public class AsteroidSpin : MonoBehaviour
{
    private float spinSpeed;

    void Start()
    {
        // Random spin speed between -100 and 100 degrees per second (negative for opposite direction)
        spinSpeed = Random.Range(-100f, 100f);
    }

    void Update()
    {
        // Spin around Z-axis locally
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }
}
