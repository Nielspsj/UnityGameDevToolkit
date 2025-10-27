using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    public TimerManager timer;
    public TextMeshProUGUI timerText;

    public void UpdateDisplay(float currentTime)
    {
        Debug.Log("UpdateDisplay: " + currentTime);
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
