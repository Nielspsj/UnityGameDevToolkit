using UnityEngine;
using static SwerveController;

public class TapAction : MonoBehaviour
{
    public enum JumpMode
    {
        Endless,   // Jump or flap wings forever
        Single   // Once and then require ground check for more
    }
    [SerializeField, Header("Switch between jump modes")]
    private JumpMode jumpMode = JumpMode.Endless; // Switch mode in Inspector

    [SerializeField, Header("Force applied upward when tapping")]
    private float jumpForce = 5f;
    [SerializeField, Header("Fall Settings")]
    private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [SerializeField, Header("How far to check for ground")]
    private float groundCheckDistance = 1.1f; // How far down to check for ground
    [SerializeField, Header("Put the ground in the groundLayer. Set the Layer of the ground to Ground in the Inspector.")]
    private LayerMask groundLayer;

    private Rigidbody rigidBody;
    private bool isGrounded = true;                // Store if player is touching the ground


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }    

    private void Start()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnTap.AddListener(HandleTap);
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnTap.RemoveListener(HandleTap);
        }
    }

    public void HandleTap()
    {
        if (jumpMode == JumpMode.Endless)
        {
            JumpEndless();
        }
        else if (jumpMode == JumpMode.Single)
        {
            JumpSingle();
        }
    }

    private void JumpEndless()
    {
        rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void JumpSingle()
    {
        // --- Ground Check ---
        // Shoot a ray downward from the player's position
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = Vector3.down;

        Debug.DrawRay(rayOrigin, rayDirection * groundCheckDistance, Color.red);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // --- Jump Input ---
        if (isGrounded == true)
        {
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (rigidBody.linearVelocity.y < 0)
        {
            // Fall faster when moving downward
            rigidBody.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rigidBody.linearVelocity.y > 0)
        {
            // Make upward movement snappier
            rigidBody.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
}
