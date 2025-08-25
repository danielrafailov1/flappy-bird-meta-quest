using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [Header("Pipe Settings")]
    public GameObject bottomPipePrefab;   // GreenPipeUp
    public GameObject topPipePrefab;      // GreenPipeDown
    public float spawnInterval = 2f;      // Time between spawns
    public float minY = 2f;               // Minimum Y position
    public float maxY = 5f;               // Maximum Y position
    public float spawnX = 10f;
    public float moveSpeed = 2f;

    [Header("References")]
    public CoinSpawner coinSpawner;       // Reference to the CoinSpawner

    private float timer = 0f;
    private bool spawnBottomNext = true;  // Alternates between bottom and top pipes
    private bool isGameActive = false;    // Only spawn when game is active

    void Update()
    {
        // Only spawn pipes when game is active
        if (!isGameActive) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnPipe();
            timer = 0f;
        }
    }

    public void StartSpawning()
    {
        isGameActive = true;
        timer = 0f; // Reset timer when starting
        Debug.Log("Pipe spawning started!");
    }

    public void StopSpawning()
    {
        isGameActive = false;
        Debug.Log("Pipe spawning stopped!");
    }

    private void SpawnPipe()
    {
        float randomY = Random.Range(minY, maxY);
        float range = maxY - minY;

        Vector3 bottomPos = Vector3.zero;
        Vector3 topPos = Vector3.zero;

        Debug.Log("THIS IS THE ARTIFACT CODE RUNNING!");

        if (spawnBottomNext)
        {
            // Bottom pipe (facing up)
            bottomPos = new Vector3(spawnX, randomY - (range * 0.4f), 0);
            Debug.Log("Spawning bottom pipe at: " + bottomPos);
            GameObject pipe = Instantiate(bottomPipePrefab, bottomPos, bottomPipePrefab.transform.rotation);
            pipe.AddComponent<MoveLeft>().speed = moveSpeed;

            // Add collider and tag for collision detection
            BoxCollider existingCollider = pipe.GetComponent<BoxCollider>();
            if (existingCollider != null)
            {
                Debug.Log("FOUND EXISTING COLLIDER on bottom pipe with size: " + existingCollider.size);
                // Remove existing collider to replace it
                DestroyImmediate(existingCollider);
            }

            BoxCollider collider = pipe.AddComponent<BoxCollider>();
            collider.isTrigger = true; // Make pipe a trigger for easier collision detection

            // Use the exact values from your perfectly adjusted collider
            Vector3 scale = pipe.transform.localScale;

            // Your perfect collider values:
            collider.size = new Vector3(0.02227314f, 0.02106743f, 0.07106968f);
            collider.center = new Vector3(-9.111848e-05f, 0.0001764514f, -0.02396321f);

            Debug.Log("BOTTOM PIPE - Pipe scale: " + scale);
            Debug.Log("BOTTOM PIPE - Added BoxCollider with size: " + collider.size);
            Debug.Log("BOTTOM PIPE - Collider center: " + collider.center);
            Debug.Log("BOTTOM PIPE - Actual world size: " + Vector3.Scale(collider.size, scale));

            // Notify coin spawner
            if (coinSpawner != null)
            {
                coinSpawner.OnPipeSpawned(true, randomY, bottomPos);
            }
        }
        else
        {
            // Top pipe (facing down)
            topPos = new Vector3(spawnX, randomY + (range * 0.4f), 0);
            Debug.Log("Spawning top pipe at: " + topPos);
            GameObject pipe = Instantiate(topPipePrefab, topPos, topPipePrefab.transform.rotation);
            pipe.AddComponent<MoveLeft>().speed = moveSpeed;

            // Add collider and tag for collision detection
            BoxCollider existingCollider = pipe.GetComponent<BoxCollider>();
            if (existingCollider != null)
            {
                Debug.Log("FOUND EXISTING COLLIDER on top pipe with size: " + existingCollider.size);
                // Remove existing collider to replace it
                DestroyImmediate(existingCollider);
            }

            BoxCollider collider = pipe.AddComponent<BoxCollider>();
            collider.isTrigger = true; // Make pipe a trigger for easier collision detection

            // Use the exact values from your perfectly adjusted collider
            Vector3 scale = pipe.transform.localScale;

            // Your perfect collider values:
            collider.size = new Vector3(0.02227314f, 0.02106743f, 0.07106968f);
            collider.center = new Vector3(-9.111848e-05f, 0.0001764514f, -0.02396321f);

            Debug.Log("TOP PIPE - Pipe scale: " + scale);
            Debug.Log("TOP PIPE - Added BoxCollider with size: " + collider.size);
            Debug.Log("TOP PIPE - Collider center: " + collider.center);
            Debug.Log("TOP PIPE - Actual world size: " + Vector3.Scale(collider.size, scale));

            // Notify coin spawner
            if (coinSpawner != null)
            {
                coinSpawner.OnPipeSpawned(false, randomY, topPos);
            }
        }

        // Create score zone
        if (bottomPos != Vector3.zero || topPos != Vector3.zero)
        {
            Vector3 scoreZonePos;
            if (spawnBottomNext) // We just spawned a bottom pipe
            {
                scoreZonePos = new Vector3(spawnX, bottomPos.y + 2f, 0); // Position above bottom pipe
            }
            else // We just spawned a top pipe
            {
                scoreZonePos = new Vector3(spawnX, topPos.y - 2f, 0); // Position below top pipe
            }

            // Create score zone GameObject
            GameObject scoreZone = new GameObject("ScoreZone");
            scoreZone.transform.position = scoreZonePos;
            scoreZone.tag = "ScoreZone";

            // Add collider
            BoxCollider scoreCollider = scoreZone.AddComponent<BoxCollider>();
            scoreCollider.isTrigger = true;
            scoreCollider.size = new Vector3(0.5f, 3f, 1f); // Adjust height as needed for the gap

            // Add ScoreZone script
            scoreZone.AddComponent<ScoreZone>();

            // Add movement
            scoreZone.AddComponent<MoveLeft>().speed = moveSpeed;

            Debug.Log("Created score zone at: " + scoreZonePos);
        }

        // Flip for next spawn
        spawnBottomNext = !spawnBottomNext;
    }

    public class MoveLeft : MonoBehaviour
    {
        public float speed = 2f;
        public float leftLimit = -10f;

        void Update()
        {
            transform.position += Vector3.left * speed * Time.deltaTime;

            if (transform.position.x < leftLimit)
            {
                Destroy(gameObject);
            }
        }
    }
}