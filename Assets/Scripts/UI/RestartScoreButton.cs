using UnityEngine;

public class RestartScoreButton : MonoBehaviour
{
    [TextArea(1, 10)]
    [SerializeField]
    private string helpInfo = "Use me to restart the highscore.";
    public void RestartScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
    }
}
