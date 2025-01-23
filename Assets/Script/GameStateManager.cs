using System;
using UnityEngine;

public static class GameStateManager
{
    public static event Action<GameState> OnGameStateChange;

    /// <summary>
    /// Do actions based off the current state of the game
    /// </summary>
    /// <param name="gameState">the current state of the game</param>
    public static void UpdateGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Menu:
                break;
            case GameState.InGame:
                UpdateCanStartCountingPoint(true);
                UpdateCanMove(true);
                break;
            case GameState.Pause:
                UpdateCanStartCountingPoint(false);
                UpdateCanMove(false);
                break;
            case GameState.GameOver:
                UpdateCanStartCountingPoint(false);
                UpdateCanMove(false);
                break;
        }
        OnGameStateChange?.Invoke(gameState);
    }

    public static event Action<bool> OnCanStartCountingPoint;
    /// <summary>
    /// Send an event that define if the game can count the point of the player or not
    /// </summary>
    /// <param name="canCountPoint">wether or not the game can count the point of the player</param>
    private static void UpdateCanStartCountingPoint(bool canCountPoint)
    {
        OnCanStartCountingPoint?.Invoke(canCountPoint);
    }

    public static event Action<bool> OnUpdateCanMove;
    /// <summary>
    /// Send an event that define wether or not the player is allowed to move
    /// </summary>
    /// <param name="canMove">wheter or not the player can move</param>
    private static void UpdateCanMove(bool canMove)
    {
        OnUpdateCanMove?.Invoke(canMove);
    }
}

public enum GameState
{
    Menu,
    InGame,
    GameOver,
    Pause
}
