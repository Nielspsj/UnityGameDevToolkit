using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // persists between scenes
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        Debug.Log("reload");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Only works for executables.
    public void QuitGame()
    {
        Application.Quit();
    }
}
