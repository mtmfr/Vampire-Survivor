using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private Slider playerXp;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private GameObject maxLevel;


    private void OnEnable()
    {
        PlayerEvent.OnSetXpToLevelUp += SetPlayerXpBar;
        PlayerEvent.OnXpGain += UpdatePlayerXpBar;
        PlayerEvent.OnLevelUp += UpdateLevel;

        PlayerEvent.OnMaxlevelAttained += MaxLevelXpBar;

        Inventory.OnGoldValueChanged += UpdateGold;

        TimerEvent.OnTimeChange += DisplayTime;

        GameStateManager.UpdateGameState(GameState.InGame);
    }

    private void OnDisable()
    {
        PlayerEvent.OnSetXpToLevelUp -= SetPlayerXpBar;
        PlayerEvent.OnXpGain -= UpdatePlayerXpBar;
        PlayerEvent.OnLevelUp -= UpdateLevel;

        PlayerEvent.OnMaxlevelAttained -= MaxLevelXpBar;

        Inventory.OnGoldValueChanged -= UpdateGold;

        TimerEvent.OnTimeChange -= DisplayTime;
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

    private void DisplayTime(int minutes, int seconds)
    {
        string secondsDisplay;
        string minuteDisplay;

        if (seconds < 10)
            secondsDisplay = $"0{seconds}";
        else secondsDisplay = seconds.ToString();

        if (minutes < 10)
            minuteDisplay = $"0{minutes}";
        else minuteDisplay = minutes.ToString();

        timerText.text = $"{minuteDisplay}:{secondsDisplay}";
    }
    #endregion

    private void UpdateGold(int gold)
    {
        goldText.text = gold.ToString();
    }
}
