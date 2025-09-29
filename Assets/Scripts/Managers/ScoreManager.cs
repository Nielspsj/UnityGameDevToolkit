using UnityEngine;
using TMPro;

[InfoHeaderClass("Drag this object into the scene. Attach the score text object into the inspector below")]
public class ScoreManager : MonoBehaviour
{
    /*
    [TextArea(1, 10)]
    [SerializeField]
    private string helpInfo = "Drag this object into the scene. \n" + "Attach the score text object into the inspector below.";
    */
    public static ScoreManager Instance { get; private set; }

    [SerializeField, Header("Text UI for score + highscore display")]
    private TextMeshProUGUI scoreText;

    private int score = 0;
    private int highscore = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        highscore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();
    }

    public void AddScore(int value)
    {
        score += value;
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("HighScore", highscore);
        }
        UpdateUI();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}\nHigh: {highscore}";
        }
    }
}
