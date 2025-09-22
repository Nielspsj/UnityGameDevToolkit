using UnityEngine;

public class LoseScreen : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(true); // Show parent object at start
        GetComponent<Canvas>().enabled = false; // Hide Canvas/UI visuals at start
    }

    public void Show()
    {
        Debug.Log("show me lose");
        GetComponent<Canvas>().enabled = true;
    }
    
    public void Hide()
    {
        GetComponent<Canvas>().enabled = false;
    }
}
