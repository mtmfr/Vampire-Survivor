using UnityEngine;

public class Experience : MonoBehaviour
{
    [Header("Level")]
    private int currentLevel;
    [SerializeField] private int maxLevel;
    private int xpToLevelUp;
    private int currentXp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = 1;

        xpToLevelUp = 15;
        currentXp = 0;

        PlayerEvent.SetXpToLevelUp(xpToLevelUp);
        PlayerEvent.XpGain(currentXp);
        PlayerEvent.LevelUp(currentLevel);
    }

    private void OnEnable()
    {
        XpEvent.OnXpGain += GainXp;
    }

    private void OnDisable()
    {
        XpEvent.OnXpGain -= GainXp;
    }

    private void GainXp(int xpGained)
    {
        if (xpGained < 0)
            return;

        if (xpToLevelUp < 0)
            return;

        if (xpGained + currentXp < xpToLevelUp)
        {
            currentXp += xpGained;
            PlayerEvent.XpGain(currentXp);
        }
        else
        {
            currentXp = currentXp + xpGained - xpToLevelUp;
            PlayerEvent.XpGain(currentXp);
            currentLevel++;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        if (currentLevel + 1 < maxLevel)
        {
            currentLevel++;
            PlayerEvent.LevelUp(currentLevel);
            xpToLevelUp *= 2;
            PlayerEvent.SetXpToLevelUp(xpToLevelUp);
        }
        else if (currentLevel + 1 == maxLevel)
        {
            PlayerEvent.LevelUp(currentLevel);
            xpToLevelUp = -1;
            PlayerEvent.MaxlevelAttained();
        }
    }
}
