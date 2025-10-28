using UnityEngine;

[InfoHeaderClass("Drag the object into the scene.")]
public class PauseMenuManager : MonoBehaviour
{
    private bool isPaused = false;

    private void Awake()
    {
        gameObject.SetActive(true); // Show parent object at start
        GetComponent<Canvas>().enabled = false; // Hide Canvas/UI visuals at start
    }

    private void Start()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnPause.AddListener(TogglePause);
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnPause.RemoveListener(TogglePause);
        }
    }

    private void TogglePause()
    {
        if (isPaused == true)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;
        GetComponent<Canvas>().enabled = true;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;
        GetComponent<Canvas>().enabled = false;
    }
}