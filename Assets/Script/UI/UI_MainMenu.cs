using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    public void UpdateGameState()
    {
        GameStateManager.UpdateGameState(GameState.InGame);
    }
}
