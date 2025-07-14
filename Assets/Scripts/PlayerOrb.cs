using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FloatingOrb : MonoBehaviour
{
    [Header("Pickup Settings")]
    public int bonusPoints = 25;

    [Header("Drift Settings")]
    public float driftSpeed = 1f;           // units per second
    public float despawnY = -6f;            // world‐space Y below which orb is destroyed

    private ScoreCounter _scoreCounter;

    void Start()
    {
        _scoreCounter = FindObjectOfType<ScoreCounter>();
    }

    void Update()
    {
        // move straight down
        transform.position += Vector3.down * driftSpeed * Time.deltaTime;

        // if it's fallen below the screen, destroy it
        if (transform.position.y < despawnY)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_scoreCounter != null)
                _scoreCounter.score += bonusPoints;

            Destroy(gameObject);
        }
    }
}
