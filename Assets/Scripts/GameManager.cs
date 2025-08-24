using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI collisionText; 
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalCoinText;
    public TextMeshProUGUI finalCollisionText; 
    public TextMeshProUGUI totalScoreText;
    public Button restartButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dieSound;

    [Header("Game Settings")]
    public int scorePerPipe = 1;
    public int coinValue = 1;

    private int currentScore = 0;
    private int coinCount = 0;
    private bool gameActive = true; 
    private int collisionCount = 0; 

    public static GameManager Instance; // Singleton pattern

    public void AddCollision()
    {
        collisionCount++;
        UpdateCollisionUI();
        Debug.Log("Collision count: " + collisionCount);
    }

    private void UpdateCollisionUI()
    {
        if (collisionText != null)
        {
            collisionText.text = "Collisions: " + collisionCount;
        }
        else
        {
            Debug.LogWarning("Collision Text not assigned!");
        }
    }

    public int GetCollisionCount()
    {
        return collisionCount;
    }

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("GameManager started!");

        // Initialize UI
        UpdateScoreUI();
        UpdateCoinUI();
        UpdateCollisionUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Debug.Log("Game over panel hidden");
        }
        else
        {
            Debug.LogWarning("Game Over Panel not assigned!");
        }

        // Setup restart button
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        // Setup audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        Debug.Log("GameManager setup complete");
    }

    public void AddScore(int points = -1)
    {
        if (!gameActive) return;

        if (points == -1) points = scorePerPipe;

        Debug.Log("Adding " + points + " points to score");

        currentScore += points;
        UpdateScoreUI();
    }

    public void CollectCoin()
    {
        if (!gameActive) return;

        coinCount += coinValue;
        UpdateCoinUI();
    }

    public void GameOver()
    {
        if (!gameActive) return;

        gameActive = false;

        // Play death sound
        if (dieSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dieSound);
        }

        // Calculate final score (current score + coins)
        int finalScore = currentScore + coinCount;

        // Update game over UI
        if (finalScoreText != null) finalScoreText.text = "Score: " + currentScore;
        if (finalCoinText != null) finalCoinText.text = "Coins: " + coinCount;
        if (totalScoreText != null) totalScoreText.text = "Total: " + finalScore;
        if (finalCollisionText != null) finalCollisionText.text = "Collisions: " + collisionCount;

        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Stop time (optional - pauses the game)
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        // Resume time
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
            Debug.Log("Score UI updated: " + currentScore);
        }
        else
        {
            Debug.LogWarning("Score Text not assigned!");
        }
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coinCount;
            Debug.Log("Coin UI updated: " + coinCount);
        }
        else
        {
            Debug.LogWarning("Coin Text not assigned!");
        }
    }

    // Getters for other scripts
    public bool IsGameActive()
    {
        return gameActive;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetCoinCount()
    {
        return coinCount;
    }
}