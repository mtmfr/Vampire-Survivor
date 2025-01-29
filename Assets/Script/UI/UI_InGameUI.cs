using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private Slider playerXp;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private GameObject maxLevel;

    private float timer;

    private bool canCountTime;


    private void Update()
    {
        if (canCountTime)
            CountTime();
    }

    private void OnEnable()
    {
        GameStateManager.OnGameStateChange += ControlTimer;
        GameStateManager.OnGameStateChange += ResetTimer;

        PlayerEvent.OnSetXpToLevelUp += SetPlayerXpBar;
        PlayerEvent.OnXpGain += UpdatePlayerXpBar;
        PlayerEvent.OnLevelUp += UpdateLevel;

        PlayerEvent.OnMaxlevelAttained += MaxLevelXpBar;

        GameStateManager.UpdateGameState(GameState.InGame);
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= ControlTimer;
        GameStateManager.OnGameStateChange -= ResetTimer;

        PlayerEvent.OnSetXpToLevelUp -= SetPlayerXpBar;
        PlayerEvent.OnXpGain -= UpdatePlayerXpBar;
        PlayerEvent.OnLevelUp -= UpdateLevel;

        PlayerEvent.OnMaxlevelAttained -= MaxLevelXpBar;
    }

    #region Xp
    private void SetPlayerXpBar(int maxXp)
    {
        playerXp.maxValue = maxXp;
    }

    private void UpdatePlayerXpBar(int current)
    {
        playerXp.value = current;
    }

    private void MaxLevelXpBar()
    {
        maxLevel.SetActive(true);
        playerXp.maxValue = 1;
        playerXp.value = 1;
    }

    private void UpdateLevel(int level)
    {
        currentLevel.text = level.ToString();
    }
    #endregion

    #region Timer
    private void ControlTimer(GameState State)
    {
        canCountTime = State switch
        {
            GameState.InGame => true,
            _ => false
        };
    }

    private void CountTime()
    {
        timer += Time.deltaTime;

        int seconds, minutes;
        seconds = Mathf.FloorToInt(timer % 60);
        minutes = Mathf.FloorToInt(timer / 60);

        if (seconds < 10)
            timerText.text = $"{minutes}:0{seconds}";
        else timerText.text = $"{minutes}:{seconds}";
    }

    private void ResetTimer(GameState gameState)
    {
        if (gameState != GameState.Menu)
            return;

        timer = 0;
    }
    #endregion
}
