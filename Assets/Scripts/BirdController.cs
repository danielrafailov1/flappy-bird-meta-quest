using UnityEngine;

public class BirdController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;          // Speed of vertical movement
    public float minY = -5f;              // Minimum Y position
    public float maxY = 5f;               // Maximum Y position
    public bool clampToLimits = true;     // Whether to clamp bird within Y limits

    [Header("Input Settings")]
    public KeyCode upKey = KeyCode.W;           // Key to move up (changed to W)
    public KeyCode downKey = KeyCode.S;         // Key to move down (changed to S)

    private Vector3 currentPosition;

    void Start()
    {
        currentPosition = transform.position;

        // Debug log to see if the script is running
        Debug.Log("BirdController started at position: " + transform.position);

        // Ensure the bird starts within the Y limits if clamping is enabled
        if (clampToLimits)
        {
            currentPosition.y = Mathf.Clamp(currentPosition.y, minY, maxY);
            transform.position = currentPosition;
            Debug.Log("Bird position after clamping: " + transform.position);
        }
    }

    void Update()
    {
        // Test if ANY input is working
        if (Input.anyKey)
        {
            Debug.Log("Some key is being pressed!");
        }

        HandleInput();
        MoveBird();
    }

    private void HandleInput()
    {
        float verticalInput = 0f;

        // Check for arrow key input
        if (Input.GetKey(upKey))
        {
            verticalInput = 1f;
            Debug.Log("UP key pressed!");
        }
        else if (Input.GetKey(downKey))
        {
            verticalInput = -1f;
            Debug.Log("DOWN key pressed!");
        }

        // Apply movement
        if (verticalInput != 0f)
        {
            Vector3 oldPos = currentPosition;
            currentPosition.y += verticalInput * moveSpeed * Time.deltaTime;

            // Clamp position if enabled
            if (clampToLimits)
            {
                currentPosition.y = Mathf.Clamp(currentPosition.y, minY, maxY);
            }

            Debug.Log("Moving bird from " + oldPos.y + " to " + currentPosition.y);
        }
    }

    private void MoveBird()
    {
        // Update the bird's position
        currentPosition.x = transform.position.x; // Keep X position unchanged
        currentPosition.z = transform.position.z; // Keep Z position unchanged
        transform.position = currentPosition;
    }

    // Public method to move bird programmatically (useful for Meta Quest integration later)
    public void MoveBirdVertical(float inputValue)
    {
        // inputValue should be between -1 (down) and 1 (up)
        inputValue = Mathf.Clamp(inputValue, -1f, 1f);

        currentPosition.y += inputValue * moveSpeed * Time.deltaTime;

        if (clampToLimits)
        {
            currentPosition.y = Mathf.Clamp(currentPosition.y, minY, maxY);
        }

        currentPosition.x = transform.position.x;
        currentPosition.z = transform.position.z;
        transform.position = currentPosition;
    }

    // Public method to set bird position directly (useful for hand tracking)
    public void SetBirdY(float targetY)
    {
        currentPosition = transform.position;
        currentPosition.y = clampToLimits ? Mathf.Clamp(targetY, minY, maxY) : targetY;
        transform.position = currentPosition;
    }

    // Get current Y position
    public float GetCurrentY()
    {
        return transform.position.y;
    }
}