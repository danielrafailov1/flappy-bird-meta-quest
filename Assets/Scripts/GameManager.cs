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
    public TextMeshProUGUI timerText;
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
    public float gameTime = 15f;

    private int currentScore = 0;
    private int coinCount = 0;
    private bool gameActive = true; 
    private int collisionCount = 0;
    private float currentTime;

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

        currentTime = gameTime;

        // Initialize UI
        UpdateScoreUI();
        UpdateCoinUI();
        UpdateCollisionUI();
        UpdateTimerUI();

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

    void Update()
    {
        if (!gameActive) return;

        // Countdown timer
        currentTime -= Time.deltaTime;
        UpdateTimerUI();

        // Check if time is up
        if (currentTime <= 0)
        {
            currentTime = 0;
            GameOver();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // Display time in seconds with 1 decimal place
            timerText.text = "Time: " + currentTime.ToString("F1") + "s";
        }
        else
        {
            Debug.LogWarning("Timer Text not assigned!");
        }
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

    public float GetCurrentTime()
    {
        return currentTime;
    }
}