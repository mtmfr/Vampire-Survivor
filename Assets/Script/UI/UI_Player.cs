using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    [SerializeField] private GameObject PlayerUI;

    [SerializeField] private Slider playerHealth;

    [SerializeField] private Slider playerXp;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private GameObject maxLevel;

    private void OnEnable()
    {
        GameStateManager.OnGameStateChange += UpdateUI;

        PlayerEvent.OnSetHealth += SetPlayerHealthUI;
        PlayerEvent.OnUpdateHealth += UpdatePlayerHealthUI;

        PlayerEvent.OnSetXpToLevelUp += SetPlayerXpBar;
        PlayerEvent.OnXpGain += UpdatePlayerXpBar;
        PlayerEvent.OnLevelUp += UpdateLevel;

        PlayerEvent.OnMaxlevelAttained += MaxLevelXpBar;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= UpdateUI;

        PlayerEvent.OnSetHealth -= SetPlayerHealthUI;
        PlayerEvent.OnUpdateHealth -= UpdatePlayerHealthUI;

        PlayerEvent.OnSetXpToLevelUp -= SetPlayerXpBar;
        PlayerEvent.OnXpGain -= UpdatePlayerXpBar;
        PlayerEvent.OnLevelUp -= UpdateLevel;

        PlayerEvent.OnMaxlevelAttained -= MaxLevelXpBar;
    }

    #region Health
    private void SetPlayerHealthUI(int health)
    {
        playerHealth.maxValue = health;
        playerHealth.value = health;
        maxLevel.SetActive(false);
    }

    private void UpdatePlayerHealthUI(int health)
    {
        playerHealth.value = health;
    }
    #endregion

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

    private void UpdateUI(GameState gameState)
    {
        if (gameState != GameState.InGame)
        {
            PlayerUI.SetActive(false);
        }
        else PlayerUI.SetActive(true);
    }
}
