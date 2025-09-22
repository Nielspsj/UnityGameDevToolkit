using UnityEngine;

public class RestartScoreButton : MonoBehaviour
{
    public void RestartScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
    }
}
