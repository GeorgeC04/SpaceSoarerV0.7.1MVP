using UnityEngine;

public class RockRotation : MonoBehaviour
{
    private float rotationSpeed;

    void Start()
    {
        // Random rotation speed between -90 and 90 degrees per second
        rotationSpeed = Random.Range(-90f, 90f);
    }

    void Update()
    {
        // Rotate around the Y axis (you can choose any axis)
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.World);
    }
}
