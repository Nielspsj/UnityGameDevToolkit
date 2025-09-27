using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [TextArea(1, 10)]
    [SerializeField] private string helpInfo = "Drag the object into the scene. Add to OnGameWin event in GameStateManager inspector.";
    void Awake()
    {
        gameObject.SetActive(true); // Show parent object at start
        GetComponent<Canvas>().enabled = false; // Hide Canvas/UI visuals at start
    }

    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
    }

    public void Hide()
    {
        GetComponent<Canvas>().enabled = false;
    }
}
