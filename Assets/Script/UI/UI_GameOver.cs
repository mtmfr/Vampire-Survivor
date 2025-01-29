using TMPro;
using UnityEngine;

public class UI_GameOver : MonoBehaviour
{
    public void GoToMainMenu()
    {
        GameStateManager.UpdateGameState(GameState.Menu);
    }
}
