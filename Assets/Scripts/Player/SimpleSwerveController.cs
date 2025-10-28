using UnityEngine;

public class SimpleSwerveController : MonoBehaviour
{
    public enum SwerveMode
    {
        Pan,   // Snappy side-to-side movement
        Turn   // Snap 90° turns
    }

    [SerializeField, Header("Switch between swerve modes")]
    private SwerveMode swerveMode = SwerveMode.Pan; // Switch mode in Inspector

    [Header("Pan Settings:")]
    [SerializeField, Header("How far the player moves sideways")]
    private float panDistance = 1f;
    [SerializeField, Header("Maximum X distance in either direction")]
    private float maxPanX = 3f;

    [Header("Turn Settings:")]
    [SerializeField, Header("How fast the player rotates")]
    private float turnSpeed = 10f; // Rotation speed when turning
    private Quaternion targetRotation;


    private void Start()
    {
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            HandleSwipe(-1);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            HandleSwipe(1);
        }

        if (swerveMode == SwerveMode.Turn)
        {
            // Smoothly rotate to target
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    private void HandleSwipe(float direction)
    {
        if (swerveMode == SwerveMode.Pan)
        {
            HandlePan(direction);
        }
        else if (swerveMode == SwerveMode.Turn)
        {
            HandleTurn(direction);
        }
    }

    private void HandlePan(float direction)
    {
        //float horizontal = direction.x; // swipe left/right
        //Vector3 targetPosition = transform.position + Vector3.right * horizontal * panSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + new Vector3(direction * panDistance, 0, 0);
        //targetPosition.x = Mathf.Clamp(targetPosition.x, -maxPanX, maxPanX);
        newPosition.x = Mathf.Clamp(newPosition.x, -maxPanX, maxPanX);
        //transform.position = targetPosition;
        transform.position = newPosition;
    }

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
}

