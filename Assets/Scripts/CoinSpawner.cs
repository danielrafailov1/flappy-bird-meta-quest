using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public int pipesPerCoin = 3;          // Spawn a coin every N pipes
    public float moveSpeed = 2f;
    public float spawnX = 10f;

    private int pipeCounter = 0;

    // Call this method from PipeSpawner when a pipe is spawned
    public void OnPipeSpawned(bool wasBottomPipe, float randomY, Vector3 pipePosition)
    {
        pipeCounter++;

        // Spawn a coin every N pipes
        if (pipeCounter % pipesPerCoin == 0 && coinPrefab != null)
        {
            Debug.Log("SPAWNING COIN - pipeCounter: " + pipeCounter + ", pipesPerCoin: " + pipesPerCoin);

            Vector3 coinPos;
            if (wasBottomPipe)
            {
                float coinY = (randomY + pipePosition.y + 4) / 2; // place it in the middle
                coinPos = new Vector3(spawnX, coinY, 0);
                Debug.Log("Creating bottom coin at position: " + coinPos);
            }
            else
            {
                float coinY = (randomY + pipePosition.y - 4) / 2; // place it in the middle
                coinPos = new Vector3(spawnX, coinY, 0);
                Debug.Log("Creating top coin at position: " + coinPos);
            }

            SpawnCoin(coinPos);
        }
    }

    private void SpawnCoin(Vector3 position)
    {
        GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity);
        coin.AddComponent<MoveLeft>().speed = moveSpeed;
        coin.tag = "Coin";
        
        Debug.Log("Spawned coin at: " + position + " with tag: " + coin.tag);
        
        // Note: Collider should be manually added to the coinPrefab in the inspector
        // This allows you to adjust the collider size and position precisely
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