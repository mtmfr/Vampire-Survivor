using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainUI : MonoBehaviour
{
    #region UI drawer
    [Header("UI drawer")]
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject GameOverMenu;
    #endregion

    [Header("Game UI")]
    [SerializeField] private TextMeshProUGUI gameScore;

    [Header("GameOver UI")]
    [SerializeField] private TextMeshProUGUI endOverScore;

    private void Start()
    {
        GameStateManager.UpdateGameState(GameState.Menu);
    }

    private void OnEnable()
    {
        GameStateManager.OnGameStateChange += GameStateUI;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= GameStateUI;
    }

    #region ChangeGameState
    public void StartGame()
    {
        GameStateManager.UpdateGameState(GameState.InGame);
    }

    public void PauseGame()
    {
        GameStateManager.UpdateGameState(GameState.Pause);
    }

    public void ResumeGame()
    {
        GameStateManager.UpdateGameState(GameState.InGame);
    }

    public void GameOver()
    {
        GameStateManager.UpdateGameState(GameState.GameOver);
    }

    public void GoToMainMenu()
    {
        GameStateManager.UpdateGameState(GameState.Menu);
    }
    #endregion

    private void GameStateUI(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                MainMenu.SetActive(true);
                PauseMenu.SetActive(false);
                GameOverMenu.SetActive(false);
                break;
            case GameState.InGame:
                MainMenu.SetActive(false);
                GameUI.SetActive(true);
                PauseMenu.SetActive(false);
                break;
            case GameState.Pause:
                GameUI.SetActive(false);
                PauseMenu.SetActive(true);
                break;
            case GameState.GameOver:
                GameUI.SetActive(false);
                GameOverMenu.SetActive(true);
                break;
        }
    }

    private void UpdateGameScoreUI(int score)
    {
        gameScore.text = score.ToString();
        endOverScore.text = score.ToString();
    }
}