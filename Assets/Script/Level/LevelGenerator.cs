using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private SO_Stage currentLevel;
    [SerializeField] private GameObject stageBg;
    private List<GameObject> bgPlacement = new(15);
    private List<Transform> availableEnemySpawnPos;

    private Transform playerTransform;

    private Sprite bgSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void OnEnable()
    {
        LevelEvent.OnLevelSelected += UpdateCurrentLevel;
        LevelEvent.OnLevelSpawn += SpawnLevelBox;
    }

    private void OnDisable()
    {
        LevelEvent.OnLevelSelected -= UpdateCurrentLevel;
        LevelEvent.OnLevelSpawn -= SpawnLevelBox;
    }

    private void UpdateCurrentLevel(SO_Stage currentLevel)
    {
        this.currentLevel = currentLevel;
    }

    private void SpawnLevelBox()
    {
        stageBg.GetComponent<SpriteRenderer>().sprite = currentLevel.BgSprite;
        for (int tiles = 0; tiles < bgPlacement.Capacity; tiles++)
        {
            bgPlacement.Add(Instantiate(stageBg));
        }

        if (bgPlacement.Count == 0)
            Debug.LogException(new ArgumentNullException("bgPlacement", "No background have been found"), this);

        Vector3 spriteSize = currentLevel.BgSprite.bounds.size;

        Vector3 offset = new();

        int offsetMultiplier = -2;

        for (int Id = 0; Id < bgPlacement.Count; Id++)
        {
            GameObject bg = bgPlacement[Id];

            if (Id < 5)
            {
                //1st row
                offset.y = playerTransform.position.y + spriteSize.y;
                offset.x = playerTransform.position.x + spriteSize.x * offsetMultiplier;

                bg.transform.position = offset;
            }
            else if (Id < 10)
            {
                //2nd row
                offset.y = playerTransform.position.y;
                offset.x = playerTransform.position.x + spriteSize.x * offsetMultiplier;

                bg.transform.position = offset;
            }
            else
            {
                //3rd row
                offset.y = playerTransform.position.y - spriteSize.y;
                offset.x = playerTransform.position.x + spriteSize.x * offsetMultiplier;

                bg.transform.position = offset;
            }

            offsetMultiplier++;

            if (offsetMultiplier >= 3)
                offsetMultiplier = -2;
        }
    }
}

public static class LevelEvent
{
    public static event Action<SO_Stage> OnLevelSelected;
    public static void LevelSelected(SO_Stage selectedLevel)
    {
        OnLevelSelected?.Invoke(selectedLevel);
    }

    public static event Action OnLevelSpawn;
    public static void LevelSpawn()
    {
        OnLevelSpawn?.Invoke();
    }
}
