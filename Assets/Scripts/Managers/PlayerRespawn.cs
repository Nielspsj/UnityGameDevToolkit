using UnityEngine;

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
}