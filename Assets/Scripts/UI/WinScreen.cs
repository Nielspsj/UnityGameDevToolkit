using UnityEngine;

public class WinScreen : MonoBehaviour
{
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
