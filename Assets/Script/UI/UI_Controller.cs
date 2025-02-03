using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    #region UI drawer
    [Header("UI drawer")]
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject LevelUpUI;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject GameOverMenu;
    #endregion

    private void Start()
    {
        GameStateManager.UpdateGameState(GameState.Menu);
    }

    private void OnEnable()
    {
        GameStateManager.OnGameStateChange += ActivateMainMenu;
        GameStateManager.OnGameStateChange += ActivateGameUI;
        GameStateManager.OnGameStateChange += ActivateLevelUpUI;
        GameStateManager.OnGameStateChange += ActivatePauseMenu;
        GameStateManager.OnGameStateChange += ActivateGameOver;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= ActivateMainMenu;
        GameStateManager.OnGameStateChange -= ActivateGameUI;
        GameStateManager.OnGameStateChange -= ActivateLevelUpUI;
        GameStateManager.OnGameStateChange -= ActivatePauseMenu;
        GameStateManager.OnGameStateChange -= ActivateGameOver;
    }

    private void ActivateMainMenu(GameState gameState)
    {
        if (gameState == GameState.Menu)
            MainMenu.SetActive(true);
        else MainMenu.SetActive(false);
    }

    private void ActivateGameUI(GameState gameState)
    {
        if (gameState == GameState.InGame || gameState == GameState.LevelUp)
            GameUI.SetActive(true);
        else GameUI.SetActive(false);
    }

    private void ActivateLevelUpUI(GameState gameState)
    {
        LevelUpUI.SetActive(gameState switch
        {
            GameState.LevelUp => true,
            _ => false
        });
    }

    private void ActivatePauseMenu(GameState gameState)
    {
        PauseMenu.SetActive(gameState switch
        {
            GameState.Pause => true,
            _ => false
        });
    }

    private void ActivateGameOver(GameState gameState)
    {
        GameOverMenu.SetActive(gameState switch
        {
            GameState.GameOver => true,
            _ => false
        });
    }
}