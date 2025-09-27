using UnityEngine;

public class LoseScreen : MonoBehaviour
{
    [TextArea(1, 10)]
    [SerializeField] private string helpInfo = "Drag the object into the scene. Add to OnGameLose event in GameStateManager inspector.";

    void Awake()
    {
        gameObject.SetActive(true); // Show parent object at start
        GetComponent<Canvas>().enabled = false; // Hide Canvas/UI visuals at start
    }

    public void Show()
    {
        //Debug.Log("show me lose");
        GetComponent<Canvas>().enabled = true;
    }
    
    public void Hide()
    {
        GetComponent<Canvas>().enabled = false;
    }
}
