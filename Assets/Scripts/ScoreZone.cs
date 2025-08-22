using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    private bool hasScored = false;
    
    void Start()
    {
        // Make sure this has a trigger collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            // Add a box collider as trigger
            BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
            boxCol.isTrigger = true;
            boxCol.size = new Vector3(0.5f, 10f, 1f); // Adjust size as needed
        }
        else
        {
            col.isTrigger = true;
        }
        
        // Set tag
        gameObject.tag = "ScoreZone";
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Only score once per zone
        if (hasScored) return;
        
        // Check if it's the bird
        if (other.GetComponent<BirdCollision>() != null)
        {
            hasScored = true;
            
            // Add score through GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore();
            }
            
            Debug.Log("Bird passed through score zone!");
        }
    }
}