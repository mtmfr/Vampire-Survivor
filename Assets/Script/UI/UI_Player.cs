using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    [SerializeField] private Slider playerHealth;

    [SerializeField] private Slider playerXp;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private GameObject maxLevel;

    private void OnEnable()
    {
        PlayerEvent.OnSetHealth += SetPlayerHealthUI;
        PlayerEvent.OnUpdateHealth += UpdatePlayerHealthUI;

        PlayerEvent.OnSetXpToLevelUp += SetPlayerXpBar;
        PlayerEvent.OnXpGain += UpdatePlayerXpBar;
        PlayerEvent.OnLevelUp += UpdateLevel;

        PlayerEvent.OnMaxlevelAttained += MaxLevelXpBar;
    }

    private void OnDisable()
    {
        PlayerEvent.OnSetHealth -= SetPlayerHealthUI;
        PlayerEvent.OnUpdateHealth -= UpdatePlayerHealthUI;

        PlayerEvent.OnSetXpToLevelUp -= SetPlayerXpBar;
        PlayerEvent.OnXpGain -= UpdatePlayerXpBar;
        PlayerEvent.OnLevelUp -= UpdateLevel;

        PlayerEvent.OnMaxlevelAttained -= MaxLevelXpBar;
    }

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
}
