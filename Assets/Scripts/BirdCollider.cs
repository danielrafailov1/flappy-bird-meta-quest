using UnityEngine;

public class BirdCollision : MonoBehaviour
{
    [Header("Collision Settings")]
    public string pipeTag = "Pipe";
    public string coinTag = "Coin";
    public string scoreZoneTag = "ScoreZone";

    [Header("Ground Detection")]
    public float groundY = 0f;  // Y position considered as ground level
    public bool useGroundY = true;  // Enable/disable ground Y position check

    [Header("Audio")]
    public AudioClip coinSound;  // Drag your coin MP3 here in the Inspector
    private AudioSource audioSource;

    [Header("Debug Info")]
    public bool showColliderInfo = true;  // Show collider info in console

    private bool hasCollided = false;
    private bool collisionEnabled = false;
    private bool hasHitGround = false;  // Track if ground collision already registered

    void Start()
    {
        Debug.Log("BirdCollision script started on: " + gameObject.name);

        // Enable collision detection after a short delay
        Invoke("EnableCollision", 0.5f);

        // Check if bird has a collider (don't add one automatically)
        Collider birdCollider = GetComponent<Collider>();
        if (birdCollider == null)
        {
            Debug.LogError("*** BIRD has NO collider! Coin collection will NOT work! ***");
        }
        else
        {
            Debug.Log("=== BIRD COLLIDER INFO ===");
            Debug.Log("Bird collider found: " + birdCollider.GetType().Name);
            Debug.Log("Bird position: " + transform.position);

            if (birdCollider is SphereCollider sphereCol)
            {
                Debug.Log("SphereCollider - Center: " + sphereCol.center + ", Radius: " + sphereCol.radius);
                Debug.Log("SphereCollider - Is Trigger: " + sphereCol.isTrigger);
                Debug.Log("World center: " + (transform.position + sphereCol.center));
                Debug.Log("World bounds: radius " + sphereCol.radius + " units from " + (transform.position + sphereCol.center));
            }
            else if (birdCollider is BoxCollider boxCol)
            {
                Debug.Log("BoxCollider - Center: " + boxCol.center + ", Size: " + boxCol.size);
                Debug.Log("BoxCollider - Is Trigger: " + boxCol.isTrigger);
            }
            else if (birdCollider is CapsuleCollider capCol)
            {
                Debug.Log("CapsuleCollider - Center: " + capCol.center + ", Radius: " + capCol.radius + ", Height: " + capCol.height);
                Debug.Log("CapsuleCollider - Is Trigger: " + capCol.isTrigger);
            }
            Debug.Log("=== END BIRD COLLIDER INFO ===");
        }

        // Check if bird has a Rigidbody (don't add one automatically)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("Bird has NO Rigidbody! Please add a Rigidbody manually if needed for physics collisions.");
        }
        else
        {
            if (showColliderInfo)
            {
                Debug.Log("Bird Rigidbody found - UseGravity: " + rb.useGravity + ", IsKinematic: " + rb.isKinematic);
            }
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

    void Update()
    {
        // Check if bird has hit the ground using Y position
        if (useGroundY && collisionEnabled && !hasCollided)
        {
            // Check if bird is currently at/below ground level
            bool isAtGround = transform.position.y <= groundY;

            // Only register collision if bird just hit the ground (transition from above to at/below)
            if (isAtGround && !hasHitGround)
            {
                hasHitGround = true;  // Mark that we've registered this ground hit
                Debug.Log("Bird hit ground at Y position: " + transform.position.y);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddCollision();
                }
            }
            // Reset the flag if bird moves above ground level
            else if (!isAtGround && hasHitGround)
            {
                hasHitGround = false;  // Allow for future ground collisions
                Debug.Log("Bird moved above ground level, ready for next ground collision");
            }
        }
    }

    void EnableCollision()
    {
        collisionEnabled = true;
        Debug.Log("Collision detection enabled for bird");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("=== TRIGGER DETECTED ===");
        Debug.Log("Collision enabled: " + collisionEnabled);
        Debug.Log("Other object: " + other.gameObject.name);
        Debug.Log("Other tag: '" + other.tag + "'");
        Debug.Log("Other position: " + other.transform.position);
        Debug.Log("Bird position: " + transform.position);
        Debug.Log("Distance: " + Vector3.Distance(transform.position, other.transform.position));

        if (!collisionEnabled)
        {
            Debug.Log("Collision ignored (not enabled yet): " + other.gameObject.name);
            return;
        }

        if (showColliderInfo)
        {
            Debug.Log("Other collider info: " + other.GetType().Name + " - IsTrigger: " + other.isTrigger);

            // Show detailed collider info
            if (other is SphereCollider sphereCol)
            {
                Debug.Log("Other SphereCollider - Center: " + sphereCol.center + ", Radius: " + sphereCol.radius);
                Debug.Log("World center: " + (other.transform.position + sphereCol.center));
            }
        }

        // Check what we collided with
        if (other.CompareTag(coinTag))
        {
            Debug.Log("*** COIN COLLISION DETECTED! ***");
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

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCollision();
            }
        }
        else
        {
            Debug.Log("Hit object with tag: '" + other.tag + "' (expected: '" + coinTag + "')");
        }
        Debug.Log("=== END TRIGGER ===");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bird collided with: " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");

        // Handle solid collisions (pipes, ground, etc.)
        if (collision.gameObject.CompareTag(pipeTag) ||
            collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("Obstacle"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCollision();
            }
        }
        else
        {
            // Hit something else with solid collision
            Debug.Log("Solid collision with untagged object: " + collision.gameObject.name);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCollision();
            }
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

    private float lastScoreTime = 0f;
    private float scoreCooldown = 1f;

    private void AddScore()
    {
        // Prevent multiple scores in quick succession
        if (Time.time - lastScoreTime < scoreCooldown) return;

        lastScoreTime = Time.time;

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
            GameManager.Instance.AddCollision();
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