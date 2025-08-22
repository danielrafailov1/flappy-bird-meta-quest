using UnityEngine;

public class BirdCollision : MonoBehaviour
{
    [Header("Collision Settings")]
    public string pipeTag = "Pipe";
    public string coinTag = "Coin";
    public string scoreZoneTag = "ScoreZone";

    [Header("Audio")]
    public AudioClip coinSound;  // Drag your coin MP3 here in the Inspector
    private AudioSource audioSource;

    private bool hasCollided = false;
    private bool collisionEnabled = false;

    void Start()
    {
        Debug.Log("BirdCollision script started on: " + gameObject.name);

        // Enable collision detection after a short delay
        Invoke("EnableCollision", 0.5f);

        // Make sure the bird has a collider
        if (GetComponent<Collider>() == null)
        {
            // Add a sphere collider if none exists
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true; // Set as trigger for coin collection
            collider.radius = 0.3f; // Make smaller to avoid unwanted collisions
            Debug.Log("Added SphereCollider to bird with radius: " + collider.radius);
        }
        else
        {
            Debug.Log("Bird already has a collider");
        }

        // Add Rigidbody for physics collision detection
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false; // We don't want gravity for this game
            rb.isKinematic = false; // Allow physics interactions
            Debug.Log("Added Rigidbody to bird");
        }
        else
        {
            Debug.Log("Bird already has a Rigidbody");
        }

        // Setup audio source for coin sounds
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // Don't play automatically
            Debug.Log("Added AudioSource to bird for coin sounds");
        }
    }

    void EnableCollision()
    {
        collisionEnabled = true;
        Debug.Log("Collision detection enabled for bird");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!collisionEnabled)
        {
            Debug.Log("Collision ignored (not enabled yet): " + other.gameObject.name);
            return;
        }

        Debug.Log("TRIGGER: Bird triggered with: " + other.gameObject.name + " (Tag: '" + other.tag + "')");

        if (hasCollided) return; // Prevent multiple collisions

        // Check what we collided with
        if (other.CompareTag(coinTag))
        {
            Debug.Log("Hit a coin!");
            CollectCoin(other.gameObject);
        }
        else if (other.CompareTag(scoreZoneTag))
        {
            Debug.Log("Hit score zone!");
            AddScore();
        }
        else if (other.CompareTag(pipeTag))
        {
            Debug.Log("Hit a pipe!");
            GameOver();
        }
        else
        {
            Debug.Log("Hit untagged object: " + other.gameObject.name + " with tag: '" + other.tag + "'");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bird collided with: " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");

        if (hasCollided) return;

        // Handle solid collisions (pipes, ground, etc.)
        if (collision.gameObject.CompareTag(pipeTag) ||
            collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("Obstacle"))
        {
            GameOver();
        }
        else
        {
            // Hit something else with solid collision
            Debug.Log("Solid collision with untagged object: " + collision.gameObject.name);
            GameOver(); // End game anyway for now
        }
    }

    private void CollectCoin(GameObject coin)
    {
        // Play coin sound
        if (coinSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(coinSound);
            Debug.Log("Playing coin collection sound");
        }
        else
        {
            Debug.LogWarning("Coin sound or AudioSource is missing!");
        }

        // Add coin to score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CollectCoin();
        }

        // Destroy the coin
        Destroy(coin);

        Debug.Log("Coin collected!");
    }

    private void AddScore()
    {
        // Add score for passing pipe
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore();
        }

        Debug.Log("Score added!");
    }

    private void GameOver()
    {
        if (hasCollided) return;

        hasCollided = true;

        // Trigger game over
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }

        // Stop bird movement
        BirdController birdController = GetComponent<BirdController>();
        if (birdController != null)
        {
            birdController.enabled = false;
        }

        Debug.Log("Game Over!");
    }
}