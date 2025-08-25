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

    [Header("Game State UI")]
    public GameObject startPanel;           // Panel with start button
    public Button startButton;             // Start game button
    public GameObject pausePanel;           // Panel with resume/quit options
    public Button pauseButton;              // Pause button (top-right corner)
    public Button resumeButton;             // Resume from pause
    public Button quitButton;               // Quit to start screen

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dieSound;

    [Header("Game Settings")]
    public int scorePerPipe = 1;
    public int coinValue = 1;
    public float gameTime = 30f;

    private int currentScore = 0;
    private int coinCount = 0;
    private bool gameActive = false;        // Changed to false initially
    private bool gameStarted = false;       // Track if game has been started
    private bool gamePaused = false;        // Track pause state
    private int collisionCount = 0;
    private float currentTime;

    public static GameManager Instance; // Singleton pattern

    public void AddCollision()
    {
        if (!gameActive || gamePaused) return;

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

        // Show start screen initially
        ShowStartScreen();

        // Hide other panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // Setup buttons
        SetupButtons();

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

    private void SetupButtons()
    {
        // Start button
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogWarning("Start Button not assigned!");
        }

        // Pause button
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }
        else
        {
            Debug.LogWarning("Pause Button not assigned!");
        }

        // Resume button
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        else
        {
            Debug.LogWarning("Resume Button not assigned!");
        }

        // Quit button
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitToStart);
        }
        else
        {
            Debug.LogWarning("Quit Button not assigned!");
        }

        // Restart button (existing)
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    private void ShowStartScreen()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Start Panel not assigned!");
        }

        // Hide pause button during start screen
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }

        // Stop background music
        StopBackgroundMusic();
    }

    public void StartGame()
    {
        Debug.Log("Starting game!");

        gameActive = true;
        gameStarted = true;
        gamePaused = false;

        // Hide start panel
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        // Show pause button
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(true);
        }

        // Reset game state
        currentScore = 0;
        coinCount = 0;
        collisionCount = 0;
        currentTime = gameTime;

        // Update UI
        UpdateScoreUI();
        UpdateCoinUI();
        UpdateCollisionUI();
        UpdateTimerUI();

        // Start background music
        StartBackgroundMusic();

        // Resume time
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        if (!gameActive || !gameStarted) return;

        Debug.Log("Pausing game!");

        gamePaused = true;

        // Show pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        // Pause background music
        PauseBackgroundMusic();

        // Pause time
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!gameStarted) return;

        Debug.Log("Resuming game!");

        gamePaused = false;

        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Resume background music
        ResumeBackgroundMusic();

        // Resume time
        Time.timeScale = 1f;
    }

    public void QuitToStart()
    {
        Debug.Log("Quitting to start screen!");

        // Reset game state
        gameActive = false;
        gameStarted = false;
        gamePaused = false;

        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Show start screen
        ShowStartScreen();

        // Resume time for UI interactions
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        // Only update timer if game is active and not paused
        if (!gameActive || !gameStarted || gamePaused) return;

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
        if (!gameActive || gamePaused) return;

        if (points == -1) points = scorePerPipe;

        Debug.Log("Adding " + points + " points to score");

        currentScore += points;
        UpdateScoreUI();
    }

    public void CollectCoin()
    {
        if (!gameActive || gamePaused) return;

        coinCount += coinValue;
        UpdateCoinUI();
    }

    public void GameOver()
    {
        if (!gameActive) return;

        gameActive = false;
        gamePaused = false;

        // Stop background music
        StopBackgroundMusic();

        // Play death sound
        if (dieSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dieSound);
        }

        // Hide pause button
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
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

    // Background Music Control Methods
    private void StartBackgroundMusic()
    {
        GameObject musicObject = GameObject.Find("BackgroundMusic");
        if (musicObject != null)
        {
            AudioSource musicSource = musicObject.GetComponent<AudioSource>();
            if (musicSource != null && !musicSource.isPlaying)
            {
                musicSource.Play();
                Debug.Log("Background music started");
            }
        }
    }

    private void PauseBackgroundMusic()
    {
        GameObject musicObject = GameObject.Find("BackgroundMusic");
        if (musicObject != null)
        {
            AudioSource musicSource = musicObject.GetComponent<AudioSource>();
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Pause();
                Debug.Log("Background music paused");
            }
        }
    }

    private void ResumeBackgroundMusic()
    {
        GameObject musicObject = GameObject.Find("BackgroundMusic");
        if (musicObject != null)
        {
            AudioSource musicSource = musicObject.GetComponent<AudioSource>();
            if (musicSource != null && !musicSource.isPlaying)
            {
                musicSource.UnPause();
                Debug.Log("Background music resumed");
            }
        }
    }

    private void StopBackgroundMusic()
    {
        GameObject musicObject = GameObject.Find("BackgroundMusic");
        if (musicObject != null)
        {
            AudioSource musicSource = musicObject.GetComponent<AudioSource>();
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop();
                Debug.Log("Background music stopped");
            }
        }
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

    // Updated getters for other scripts
    public bool IsGameActive()
    {
        return gameActive && !gamePaused;
    }

    public bool IsGameStarted()
    {
        return gameStarted;
    }

    public bool IsGamePaused()
    {
        return gamePaused;
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