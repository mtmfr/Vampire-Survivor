using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private SO_CurrentLevel currentLevel;
    private List<GameObject> bgPlacement = new(9);
    private List<Transform> availableEnemySpawnPos;

    private Transform playerTransform;

    private Sprite bgSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int tiles = 0; tiles < bgPlacement.Capacity; tiles++)
        {
            bgPlacement.Add(Instantiate(currentLevel.LevelBg));
        }

        playerTransform = GameObject.FindWithTag("Player").transform;

        SpawnLevelBox();
    }

    private void OnEnable()
    {
        LevelEvent.OnLevelSelected += UpdateCurrentLevel;
    }

    private void OnDisable()
    {
        LevelEvent.OnLevelSelected -= UpdateCurrentLevel;
    }

    private void UpdateCurrentLevel(SO_CurrentLevel currentLevel)
    {
        this.currentLevel = currentLevel;
    }

    private void SpawnLevelBox()
    {
        if (bgPlacement.Count == 0)
            Debug.LogException(new System.ArgumentNullException("bgPlacement", "No background have been found"), this);

        Vector3 spriteSize = currentLevel.BgSprite.bounds.size;

        bgPlacement[0].transform.position = playerTransform.position - spriteSize;
        bgPlacement[1].transform.position = playerTransform.position + Vector3.down * spriteSize.y;
        Vector3 pos = Vector3.down * spriteSize.y + Vector3.right * spriteSize.x;
        bgPlacement[2].transform.position = playerTransform.position + pos;

        bgPlacement[3].transform.position = playerTransform.position + Vector3.left * spriteSize.x;
        bgPlacement[4].transform.position = playerTransform.position;
        bgPlacement[5].transform.position = playerTransform.position + Vector3.right * spriteSize.x;

        pos = Vector3.left * spriteSize.x + Vector3.up * spriteSize.y;
        bgPlacement[6].transform.position = playerTransform.position + pos;
        bgPlacement[7].transform.position = playerTransform.position + Vector3.up * spriteSize.x;
        pos = Vector3.up * spriteSize.y + Vector3.right * spriteSize.x;
        bgPlacement[8].transform.position = playerTransform.position + pos;
    }
}

public static class LevelEvent
{
    public static event Action<SO_CurrentLevel> OnLevelSelected;
    public static void LevelSelected(SO_CurrentLevel selectedLevel)
    {
        OnLevelSelected?.Invoke(selectedLevel);
    }
}
