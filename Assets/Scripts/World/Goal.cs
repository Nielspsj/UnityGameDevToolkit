using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameStateManager.Instance.WinGame();  
        }
    }
}
