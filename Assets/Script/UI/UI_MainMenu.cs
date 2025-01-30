using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    #region main menu screen
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject characterSelectScreen;
    [SerializeField] private GameObject stageSelect;
    #endregion

    private SO_Character characterToUse;
    private SO_CurrentLevel levelToLoad;

    public void UpdateGameState()
    {
        
    }

    #region navigation
    private void GoToMainMenu()
    {
        mainMenuScreen.SetActive(true);
        characterSelectScreen.SetActive(false);
        characterSelectScreen.SetActive(false);
    }

    private void GoToCharacterSelect()
    {
        mainMenuScreen.SetActive(false);
        characterSelectScreen.SetActive(true);
        characterSelectScreen.SetActive(false);
    }

    private void GoToLevelSelet()
    {
        mainMenuScreen.SetActive(false);
        characterSelectScreen.SetActive(false);
        characterSelectScreen.SetActive(true);
    }
    #endregion

    public void SetupGame()
    {
        GoToCharacterSelect();
    }

    public void ChoseCharacter(SO_Character chosenCharacter)
    {
        characterToUse = chosenCharacter;
        GoToLevelSelet();
    }

    public void ChoseMap(SO_CurrentLevel chosenLevel)
    {
        levelToLoad = chosenLevel;
        StartGame();
    }

    private void StartGame()
    {
        PlayerEvent.CharacterChosen(characterToUse);
        LevelEvent.LevelSelected(levelToLoad);
        GameStateManager.UpdateGameState(GameState.InGame);
    }

    public void ReturnToMainMenu()
    {
        if (characterToUse != null)
            characterToUse = null;

        GoToMainMenu();
    }

    public void ReturnToLevelSelect()
    {
        if (levelToLoad != null)
            levelToLoad = null;

        GoToCharacterSelect();
    }
}
