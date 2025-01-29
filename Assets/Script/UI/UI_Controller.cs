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
        MainMenu.SetActive(gameState switch
        {
            GameState.Menu => true,
            _ => false
        });
    }

    private void ActivateGameUI(GameState gameState)
    {
        GameUI.SetActive(gameState switch
        {
            GameState.InGame => true,
            GameState.LevelUp => true,
            _ => false
        });
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