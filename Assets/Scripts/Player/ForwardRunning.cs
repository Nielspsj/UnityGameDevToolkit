using UnityEngine;

public class ForwardRunner : MonoBehaviour
{
    [TextArea(1, 10)]
    [SerializeField]
    private string helpInfo = "Put me on the player";

    [Header("Forward Movement Settings")]
    public float forwardSpeed = 5f;
    public bool isRunning = true;
    
    void Update()
    {
        if (isRunning)
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }
    }

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
