using UnityEngine;
using UnityEngine.Events;

[InfoHeaderClass("Put me onto checkpoint triggers. The PlayerRespawn script will store my position.")]
public class Checkpoint : MonoBehaviour
{
    public UnityEvent OnCheckPointSave;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn.lastCheckpointPosition = transform.position;
            OnCheckPointSave?.Invoke();
        }
    }
}