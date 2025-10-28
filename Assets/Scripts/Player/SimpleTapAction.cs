using UnityEngine;
using static SwerveController;

public class SimpleTapAction : MonoBehaviour
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
    [SerializeField, Header("How far to check for ground")]
    private float groundCheckDistance = 1.1f; // How far down to check for ground
    [SerializeField, Header("Put the ground in the groundLayer")]
    private LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded = true;                // Store if player is touching the ground


    private void Awake()
    {        
        rb = GetComponent<Rigidbody>();
    }    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleTap();
        }
    }

    private void HandleTap()
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
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void JumpSingle()
    {
        // --- Ground Check ---
        // Shoot a ray downward from the player's position
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
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
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
