using UnityEngine;

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
        if (Input.GetKeyDown(respawnKey))
        {
            transform.position = lastCheckpointPosition;
        }
    }

    public void RespawnAtCheckpoint()
    {
        Debug.Log("Respawn at checkpoint");
        transform.position = lastCheckpointPosition;
    }
}