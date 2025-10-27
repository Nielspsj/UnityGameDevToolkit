using UnityEngine;
using TMPro;

[InfoHeaderClass("I show the timers on the screen")]
public class TimerDisplay : MonoBehaviour
{
    [Header("References")]
    public TimerManager timer;              // Assign in Inspector
    public TextMeshProUGUI timerText;       // Assign in Inspector

    [Header("Display Options")]
    public bool showMilliseconds = false;   // Optional toggle for showing 00:00:000

    private bool usingCountdown = false;

    void Start()
    {
        if (timer == null)
        {
            Debug.LogWarning("TimerDisplay: No TimerManager assigned!");
            return;
        }

        // Always sync with TimerManager’s mode
        usingCountdown = timer.useCountDown;

        // Optional: Initialize display at start
        float startTime;
        if (usingCountdown)
        {
            startTime = timer.GetCurrentTime();
        }
        else
        {
            startTime = 0f;
        }
        UpdateDisplay(startTime);
    }

    public void UpdateDisplay(float currentTime)
    {
        if (timer == null || timerText == null)
            return;

        // Always re-sync with TimerManager in case its mode changes mid-game
        usingCountdown = timer.useCountDown;

        // Safety: never show negatives
        currentTime = Mathf.Max(0, currentTime);

        // Convert total seconds to minutes + seconds
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        if (showMilliseconds)
        {
            int milliseconds = Mathf.FloorToInt((currentTime % 1) * 1000);
            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
        }
        else
        {
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        // Optional visual cue if countdown hits zero
        if (usingCountdown && currentTime <= 0)
        {
            timerText.color = Color.red; // highlight when time’s up
        }
    }
}
