using UnityEngine;
using UnityEngine.InputSystem;

[InfoHeaderClass("Put me on the player. I store last checkpoint for respawning from GameStateManager and can also respawn instantly with R")]
public class PlayerRespawn : MonoBehaviour
{
    public static Vector3 lastCheckpointPosition;
    public KeyCode respawnKey = KeyCode.R;

    void Start()
    {
        // Set initial spawn point
        lastCheckpointPosition = transform.position;
    }

    void Update()
    {
        RPressed();
    }

    private void RPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = UnityEngine.InputSystem.Keyboard.current;

        if (k != null && (k.rKey.wasPressedThisFrame))
        {
            transform.position = lastCheckpointPosition;
        }
#else
        if (Input.GetKeyDown(respawnKey))
        {
            transform.position = lastCheckpointPosition;
        }
#endif   
    }

    public void RespawnAtCheckpoint()
    {
        Debug.Log("Respawn at checkpoint");
        transform.position = lastCheckpointPosition;
    }
}