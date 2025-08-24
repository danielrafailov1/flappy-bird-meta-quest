using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    private AudioSource musicSource;

    void Start()
    {
        musicSource = GetComponent<AudioSource>();

        if (musicSource == null)
        {
            Debug.LogError("No AudioSource found on BackgroundMusic GameObject!");
        }
    }

    void Update()
    {
        // Check if game is over and stop music
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive())
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop();
                Debug.Log("Background music stopped - game over");
            }
        }
    }
}