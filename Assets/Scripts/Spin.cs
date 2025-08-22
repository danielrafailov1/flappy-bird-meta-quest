using UnityEngine;

public class Spin : MonoBehaviour
{
    public float rotationSpeed = 180f; // degrees per second

    void Update()
    {
        // Rotate around Y axis (like a coin spinning)
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
