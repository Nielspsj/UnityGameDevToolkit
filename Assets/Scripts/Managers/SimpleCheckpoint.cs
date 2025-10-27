using UnityEngine;

public class SimpleCheckpoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn.lastCheckpointPosition = transform.position;
        }
    }
}