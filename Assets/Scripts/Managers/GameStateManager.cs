using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Won, Lost }

[InfoHeaderClass("Put me into the scene. I handle Game States with events")]
public class GameStateManager : MonoBehaviour
{
    /*
    [TextArea(1, 10)]
    [SerializeField]
    private string helpInfo = "Put me into the scene";
    */
    public static GameStateManager Instance { get; private set; }

    [Header("Events (Add in the Inspector UI screens, ForwardRunner etc.)")]
    public UnityEvent OnGameWin;
    public UnityEvent OnGameLose;
    public UnityEvent OnGameRestart;
    public UnityEvent OnGameNextLevel;

    private GameState state = GameState.Playing;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void WinGame()
    {
        if (state != GameState.Playing) return;
        state = GameState.Won;
        OnGameWin?.Invoke();
    }

    public void LoseGame()
    {
        if (state != GameState.Playing) return;
        state = GameState.Lost;
        OnGameLose?.Invoke();
    }

    public void RestartGame()
    {
        //Debug.Log("restart gamestate method called");

        state = GameState.Playing;
        OnGameRestart?.Invoke();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        state = GameState.Playing;
        OnGameNextLevel?.Invoke();
    }
}
