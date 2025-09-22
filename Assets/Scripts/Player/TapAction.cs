using UnityEngine;

public class TapAction : MonoBehaviour
{
    [SerializeField, Tooltip("Force applied upward when tapping")]
    private float jumpForce = 5f;

    private Rigidbody rb;

    private void Awake() => rb = GetComponent<Rigidbody>();

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnTap.AddListener(Jump);
            //Debug.Log("adding listener");
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnTap.RemoveListener(Jump);
            //Debug.Log("Removing listener");
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
