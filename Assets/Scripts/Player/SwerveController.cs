using UnityEngine;

public class SwerveController : MonoBehaviour
{
    public enum SwerveMode
    {
        Pan,   // Snappy side-to-side movement
        FreePan, // Smooth and free side-to-side movement
        Turn   // Snap 90° turns
    }

    [SerializeField, Header("Switch between swerve modes")]
    private SwerveMode swerveMode = SwerveMode.Pan; // Switch mode in Inspector

    [Header("Pan Settings:")]
    [SerializeField, Header("How far the player moves sideways")]
    private float panDistance = 1f;
    [SerializeField, Header("Maximum X distance in either direction")]
    private float maxPanX = 3f;
    [SerializeField, Header("How fast to pan/move in FreePan")]
    private float moveSpeed = 10f;

    [Header("Turn Settings:")]
    [SerializeField, Header("How fast the player rotates")]
    private float turnSpeed = 10f; // Rotation speed when turning
    private Quaternion targetRotation;
    private Rigidbody rigidBody;

    // Stores Pan movement until the next physics update.
    private float pendingPan = 0f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        targetRotation = rigidBody.rotation;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnSwipe.RemoveListener(HandleSwipe);
        }
    }

    private void Start()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnSwipe.AddListener(HandleSwipe);
        }
        targetRotation = transform.rotation;
    }

    public void HandleSwipe(float direction)
    {
        if (swerveMode == SwerveMode.Pan)
        {
            // Save the requested movement for the physics update.
            pendingPan += direction * panDistance;
            //HandlePan(direction);
        }
        else if (swerveMode == SwerveMode.Turn)
        {
            HandleTurn(direction);
        }
    }
    /*
    private void HandlePan(float direction)
    {
        Vector3 newPosition = transform.position + new Vector3(direction * panDistance, 0, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, -maxPanX, maxPanX);
        transform.position = newPosition;
    }
    */
    
    private void HandleTurn(float direction)
    {
        if (direction > 0) // swipe right
        {
            targetRotation *= Quaternion.Euler(0, 90, 0);
        }
        else if (direction < 0) // swipe left
        {
            targetRotation *= Quaternion.Euler(0, -90, 0);
        }
    }

    private void Update()
    {
        /*
        if (swerveMode == SwerveMode.Turn)
        {
            // Smoothly rotate to target
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
        */
    }

    private void FixedUpdate()
    {
        // Choose which movement method to run.
        if (swerveMode == SwerveMode.Pan)
        {
            HandlePan();
        }
        else if (swerveMode == SwerveMode.FreePan)
        {
            HandleFreePan();
        }
        else if (swerveMode == SwerveMode.Turn)
        {
            HandleTurnMovement();
        }     
    }
    private void HandlePan()
    {
        /*
        Vector3 newPosition = rigidBody.position;
        newPosition.x += 1f;

        rigidBody.position = newPosition;
        */
        
        if (pendingPan == 0f)
            return;

        // Start with the player's current position.
        Vector3 newPosition = rigidBody.position;

        // Apply the distance requested by the last key press.
        newPosition.x += pendingPan;        

        // Keep the player within the horizontal boundaries.
        newPosition.x = Mathf.Clamp(newPosition.x, -maxPanX, maxPanX);

        // Move the player using the Rigidbody.
        //rigidBody.MovePosition(newPosition);
        rigidBody.position = newPosition;

        // Reset the value so the movement only happens once.
        pendingPan = 0f;
        
        /*
        Vector3 newPosition = rigidBody.position + new Vector3(direction * panDistance, 0f, 0f);

        newPosition.x = Mathf.Clamp(newPosition.x, -maxPanX, maxPanX);

        rigidBody.MovePosition(newPosition);
        */
    }

    private void HandleFreePan()
    {
        Vector3 newPosition = rigidBody.position;

        // Ask the InputManager which direction is being held.
        float direction = 0f;

        if (InputManager.Instance != null)
        {
            direction = InputManager.Instance.horizontalInput;
        }

        // Calculate how far to move this physics step.
        float distanceToMove = direction * moveSpeed * Time.fixedDeltaTime;

        // Change the player's horizontal position only.
        newPosition.x = newPosition.x + distanceToMove;

        // Keep the player within the horizontal maxPan boundaries.
        newPosition.x = Mathf.Clamp(newPosition.x, -maxPanX, maxPanX);

        // Move the Rigidbody without changing its vertical position directly.
        rigidBody.MovePosition(newPosition);
    }

    private void HandleTurnMovement()
    {
        // Smoothly rotate towards the target rotation.
        Quaternion newRotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);

        rigidBody.MoveRotation(newRotation);
    }
}

