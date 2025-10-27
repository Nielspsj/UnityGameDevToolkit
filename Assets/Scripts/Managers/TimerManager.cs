using UnityEngine;
using UnityEngine.Events;

[InfoHeaderClass("Put me into the scene. I handle timers with some events")]
public class TimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public bool autoStart = true;
    public float duration = 0f; // 0 = infinite timer (just counts up)
    public bool useCountDown = false; // If true, counts down instead of up

    [Header("Events")]
    public UnityEvent OnTimerStart;
    public UnityEvent<float> OnTimerTick; // sends current time each frame
    public UnityEvent OnTimerEnd;

    private float currentTime = 0f;
    private bool isRunning = false;

    void Start()
    {
        if (autoStart)
            StartTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime += (useCountDown ? -1 : 1) * Time.deltaTime;
        OnTimerTick?.Invoke(currentTime);

        
        if (useCountDown == true && currentTime <= 0)
        {
            StopTimer();
            OnTimerEnd?.Invoke();
        }
        else if (!useCountDown == true && duration > 0 && currentTime >= duration)
        {
            Debug.Log("timer stopped");
            StopTimer();
            OnTimerEnd?.Invoke();
        }
        
    }

    public void StartTimer(float startValue = 0f)
    {
        currentTime = useCountDown && duration > 0 ? duration : startValue;
        isRunning = true;
        OnTimerStart?.Invoke();
    }
    
    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = useCountDown && duration > 0 ? duration : 0f;
    }
    
    public float GetCurrentTime()
    {
        return currentTime;
    }
    
}
