using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    [Header("Bird Settings")]
    public GameObject birdPrefab;         // Your bird prefab
    public Vector3 spawnPosition = new Vector3(-5, 2, 0);  // Move bird far left of pipes
    public bool spawnOnStart = true;      // Spawn bird automatically on start

    [Header("Audio Settings")]
    public AudioClip coinSound;           // Drag your coin MP3 here - will auto-assign to spawned birds

    private GameObject currentBird;       // Reference to the spawned bird

    void Start()
    {
        if (spawnOnStart && birdPrefab != null)
        {
            SpawnBird();
        }
    }

    public void SpawnBird()
    {
        Debug.Log("SpawnBird called");
        // Destroy existing bird if there is one
        if (currentBird != null)
        {
            Destroy(currentBird);
        }
        // Spawn new bird
        if (birdPrefab != null)
        {
            Debug.Log("Bird prefab exists: " + birdPrefab.name);
            Debug.Log("Spawning bird at position: " + spawnPosition);
            currentBird = Instantiate(birdPrefab, spawnPosition, birdPrefab.transform.rotation);
            Debug.Log("Bird spawned successfully at: " + currentBird.transform.position);

            // Add the bird controller script if it doesn't exist
            if (currentBird.GetComponent<BirdController>() == null)
            {
                BirdController controller = currentBird.AddComponent<BirdController>();
                Debug.Log("BirdController added to bird");
            }
            else
            {
                Debug.Log("BirdController already exists on bird");
            }

            // Add collision detection script and assign coin sound
            BirdCollision collision = currentBird.GetComponent<BirdCollision>();
            if (collision == null)
            {
                collision = currentBird.AddComponent<BirdCollision>();
                Debug.Log("BirdCollision added to bird");
            }
            else
            {
                Debug.Log("BirdCollision already exists on bird");
            }

            // Automatically assign the coin sound to the BirdCollision script
            if (collision != null && coinSound != null)
            {
                collision.coinSound = coinSound;
                Debug.Log("Coin sound automatically assigned to bird: " + coinSound.name);
            }
            else if (coinSound == null)
            {
                Debug.LogWarning("Coin sound not assigned in BirdSpawner! Drag your MP3 file to the 'Coin Sound' field.");
            }
        }
        else
        {
            Debug.LogWarning("Bird prefab is not assigned!");
        }
    }

    public GameObject GetCurrentBird()
    {
        return currentBird;
    }

    // Optional: Method to respawn bird (useful for testing)
    [ContextMenu("Respawn Bird")]
    public void RespawnBird()
    {
        SpawnBird();
    }
}