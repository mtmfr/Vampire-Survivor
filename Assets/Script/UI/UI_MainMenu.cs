using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    private AudioSource music;

    #region main menu screen
    [SerializeField] private GameObject startMenuScreen;
    [SerializeField] private GameObject characterSelectScreen;
    [SerializeField] private GameObject stageSelectScreen;
    #endregion

    #region Character
    [SerializeField] private List<CharacterButton> characterButtons = new();

    private SO_Character characterToUse;

    private bool characterSpriteLoaded = false;
    #endregion

    #region Stage
    [SerializeField] private List<StageButton> stageButtons = new();
    private SO_Stage levelToLoad;

    private bool stageSpriteLoaded = false;
    #endregion

    private void Awake()
    {
        music = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        GoToStartMenu();
        music.Play();
    }
    private void OnDisable()
    {
        music.Stop();
    }

    #region navigation
    private void GoToStartMenu()
    {
        startMenuScreen.SetActive(true);
        characterSelectScreen.SetActive(false);
        stageSelectScreen.SetActive(false);
    }

    private void GoToCharacterSelect()
    {
        characterSelectScreen.SetActive(true);
        startMenuScreen.SetActive(false);
        stageSelectScreen.SetActive(false);
    }

    private void GoToLevelSelect()
    {
        stageSelectScreen.SetActive(true);
        startMenuScreen.SetActive(false);
        characterSelectScreen.SetActive(false);
    }
    #endregion

    /// <summary>
    /// Go to the character select screen. Bound in the editor
    /// </summary>
    public void SetupGame()
    {
        GoToCharacterSelect();
        LoadCharacterSprite();
    }

    /// <summary>
    /// Screen that let the player chose their starting character
    /// </summary>
    /// <param name="chosenCharacter">the character to use</param>
    public void ChoseCharacter(SO_Character chosenCharacter)
    {
        characterToUse = chosenCharacter;
        GoToLevelSelect();
        LoadStageSprite();
    }

    /// <summary>
    /// Let the player chose the map they will be playing on
    /// </summary>
    /// <param name="chosenLevel"></param>
    public void ChoseMap(SO_Stage chosenLevel)
    {
        levelToLoad = chosenLevel;
        StartGame();
    }

    /// <summary>
    /// Start the game
    /// </summary>
    private void StartGame()
    {
        PlayerEvent.CharacterChosen(characterToUse);
        LevelEvent.LevelSelected(levelToLoad);
        LevelEvent.LevelSpawn();
        GameStateManager.UpdateGameState(GameState.InGame);
    }

    /// <summary>
    /// let the player return to main menu
    /// </summary>
    public void ReturnToMainMenu()
    {
        characterToUse = null;

        GoToStartMenu();
    }

    /// <summary>
    /// Let the plauyer go back to the character select screen
    /// </summary>
    public void ReturnToLevelSelect()
    {
        levelToLoad = null;

        GoToCharacterSelect();
    }

    /// <summary>
    /// Load the sprite of the character on the character select screen
    /// </summary>
    private void LoadCharacterSprite()
    {
        if (!characterSpriteLoaded)
        {
            foreach (CharacterButton charImage in characterButtons)
            {
                charImage.buttonImage.sprite = charImage.character.CharacterSprite;
                charImage.characterName.text = charImage.character.name;
            }
            characterSpriteLoaded = true;
        }
    }

    private void LoadStageSprite()
    {
        if (!stageSpriteLoaded)
        {
            foreach(StageButton stageButton in stageButtons)
            {
                stageButton.stageImage.sprite = stageButton.stage.BgSprite;
                stageButton.stageDescription.text = stageButton.stage.StageDescription;
            }
        }
    }
}

[Serializable]
public class CharacterButton
{
    public SO_Character character;
    public Image buttonImage;
    public TextMeshProUGUI characterName;
}

[Serializable]
public class StageButton
{
    public SO_Stage stage;
    public Image stageImage;
    public TextMeshProUGUI stageDescription;
}
