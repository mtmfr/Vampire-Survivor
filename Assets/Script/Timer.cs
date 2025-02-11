using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private bool canCountTime;
    private float timer;
    public static int maxTime;

    private int minutes;
    private int lastMinutes = -1;

    // Update is called once per frame
    void FixedUpdate()
    {
        CountTime();
    }

    private void OnEnable()
    {
        GameStateManager.OnGameStateChange += ControlTimer;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= ControlTimer;
    }

    private void ControlTimer(GameState state)
    {
        Time.timeScale = state == GameState.InGame ? 1 : 0;
    }

    private void CountTime()
    {
        timer += Time.fixedDeltaTime;

        int seconds;
        seconds = Mathf.FloorToInt(timer % 60);
        minutes = Mathf.FloorToInt(timer / 60);
        
        if (minutes > lastMinutes)
        {
            TimerEvent.MinutesChange(minutes);
            lastMinutes++;
        }
        TimerEvent.TimeChange(minutes, seconds);
    }
}

public static class TimerEvent
{
    public static event Action<int, int> OnTimeChange;
    public static void TimeChange(int minutes, int seconds) => OnTimeChange?.Invoke(minutes, seconds);

    public static event Action<int> OnMinutesChange;
    public static void MinutesChange(int minutes) => OnMinutesChange?.Invoke(minutes);

    public static event Action OnTimeRanOut;
    public static void TimeRanOut() => OnTimeRanOut?.Invoke();
}
