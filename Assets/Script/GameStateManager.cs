using System;
using UnityEngine;

public static class GameStateManager
{
    public static event Action<GameState> OnGameStateChange;

    private static int gameGold;

    /// <summary>
    /// Do actions based off the current state of the game
    /// </summary>
    /// <param name="gameState">the current state of the game</param>
    public static void UpdateGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Menu:
                Inventory.ClearWeaponList();
                break;
            case GameState.InGame:
                Inventory.GoldValueChanged(Inventory.gold);
                break;
            case GameState.Pause:
                break;
            case GameState.GameOver:
                gameGold += Inventory.gold;
                Inventory.gold = 0;
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
}

public enum GameState : byte
{
    Menu,
    InGame,
    LevelUp,
    GameOver,
    Pause
}
