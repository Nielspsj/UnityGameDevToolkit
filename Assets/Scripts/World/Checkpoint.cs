using UnityEngine;

[InfoHeaderClass("Put me onto checkpoint triggers. The PlayerRespawn script will store my position.")]
public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn.lastCheckpointPosition = transform.position;
        }
    }
}