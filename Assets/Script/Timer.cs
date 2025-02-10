using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private bool canCountTime;
    private float timer;
    public static int maxTime;

    private int lastSeconds, lastMinutes = -1;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canCountTime)
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
        canCountTime = state switch
        {
            GameState.InGame => true,
            _ => false
        };
    }

    private void CountTime()
    {
        timer += Time.fixedDeltaTime;

        int seconds, minutes;
        seconds = Mathf.FloorToInt(timer % 60);
        minutes = Mathf.FloorToInt(timer / 60);

        if (seconds > lastSeconds)
        {
            TimerEvent.TimeChange(minutes, seconds);
            lastSeconds = seconds;
        }

        if (minutes > lastMinutes)
        {
            TimerEvent.MinutesChange(minutes);
            lastMinutes = minutes;
        }
    }
}

public static class TimerEvent
{
    public static event Action<int, int> OnTimeChange;
    public static void TimeChange(int minutes, int seconds) => OnTimeChange?.Invoke(minutes, seconds);

    public static event Action<int> OnMinutesChange;
    public static void MinutesChange(int minutes) => OnMinutesChange?.Invoke(minutes);
}
