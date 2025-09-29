using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[InfoHeaderClass("Place this object to where you want the end goal to be. " +
        "The player wins by entering the green trigger box")]
public class Goal : MonoBehaviour
{
    /*
    [ReadOnly]
    [SerializeField] private string helpinfo = "Place this object to where you want the end goal to be. " +
        "The player wins by entering the green trigger box.";
    */
    /*
    [ReadOnly]
    [InfoText("Place this object to where you want the end goal to be. " +
        "The player wins by entering the green trigger box.")]
    */
    /*
    [TextArea(1, 10)]
    [SerializeField]
    private string helpInfo = "Place this object to where you want the end goal to be. " +
        "The player wins by entering the green trigger box.";
    */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameStateManager.Instance.WinGame();  
        }
    }
}
