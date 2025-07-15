using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FloatingOrb : MonoBehaviour
{
    [Header("Drift")]
    public float driftSpeed;    // units/sec
    public float despawnY ;   // world-space Y to self-destroy

    void Update()
    {
        // fall straight down
        transform.position += Vector3.down * driftSpeed * Time.deltaTime;

        // off‐screen?
        if (transform.position.y < despawnY)
            Destroy(gameObject);
    }
}
