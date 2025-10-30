using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[InfoHeaderClass("Place this object to where you want the end goal to be. " +
        "The player wins by entering the green trigger box")]
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
