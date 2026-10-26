using UnityEngine;

public class ForwardRunner : MonoBehaviour
{    
    [Header("Forward Movement Settings")]
    public float forwardSpeed = 5f;
    public bool isRunning = true;

    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isRunning)
        {
            Vector3 forwardMovement = transform.forward * forwardSpeed * Time.fixedDeltaTime;
            rigidBody.MovePosition(rigidBody.position + forwardMovement);
        }
    }
    /*
    void Update()
    {
        if (isRunning)
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }
    }
    */
    // Start/stop running via GameStateManager
    public void StartRunning()
    {
        isRunning = true;
        Time.timeScale = 1;
    }
    public void StopRunning()
    {
        isRunning = false;
        Time.timeScale = 0;
    }
}
