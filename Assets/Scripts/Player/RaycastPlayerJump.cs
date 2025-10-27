using UnityEngine;

public class RaycastPlayerJump : MonoBehaviour
{
    public float jumpForce = 5f;            // How strong the jump is
    public float groundCheckDistance = 1.1f; // How far down to check for ground
    public LayerMask groundLayer;           // Which layer counts as "ground"

    private Rigidbody rb;
    private bool isGrounded;                // Store if player is touching the ground

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Jump();        
    }

    private void Jump()
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // Just to visualize the ground check ray in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
