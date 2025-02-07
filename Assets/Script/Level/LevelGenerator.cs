using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private SO_Stage currentLevel;

    [SerializeField] private GameObject stageBg;
    [SerializeField] private GameObject lightSource;
    private List<GameObject> Tiles { get; } = new(35);
    //private List<Transform> availableEnemySpawnPos;
    private List<GameObject> MoveTriggerTiles { get; } = new();

    private Transform playerTransform;

    private Camera gameCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        gameCamera = Camera.main;
    }

    private void OnEnable()
    {
        LevelEvent.OnLevelSelected += UpdateCurrentLevel;
        LevelEvent.OnLevelSpawn += SpawnLevelBox;
        LevelEvent.OnLevelSpawn += SetLevelTimer;

        LevelTile.OnPlayerLeft += MoveTiles;
    }

    private void OnDisable()
    {
        LevelEvent.OnLevelSelected -= UpdateCurrentLevel;
        LevelEvent.OnLevelSpawn -= SpawnLevelBox;
        LevelEvent.OnLevelSpawn -= SetLevelTimer;

        LevelTile.OnPlayerLeft -= MoveTiles;
    }

    private void UpdateCurrentLevel(SO_Stage currentLevel)
    {
        this.currentLevel = currentLevel;
    }

    private void SetLevelTimer()
    {
        Timer.maxTime = currentLevel.Duration;
    }

    #region level Tiles
    private void SpawnLevelBox()
    {
        //Set the current stage backGround
        stageBg.GetComponent<SpriteRenderer>().sprite = currentLevel.BgSprite;

        //instantiate the tiles 
        for (int tiles = 0; tiles < Tiles.Capacity; tiles++)
        {
            this.Tiles.Add(Instantiate(stageBg));
        }

        if (Tiles.Count == 0)
            Debug.LogException(new ArgumentNullException("bgPlacement", "No background have been found"), this);

        //Get the sprite size
        Vector3 spriteSize = currentLevel.BgSprite.bounds.size;

        //initialize the offset
        Vector3 offset = new();

        //initialize the offset multiplier on x
        int offsetMultiplier = -3;

        MoveTriggerTiles.Clear();

        //Place the tiles
        for (int Id = 0; Id < Tiles.Count; Id++)
        {
            GameObject bg = Tiles[Id];

            bg.name = $"BackGround nb {Id + 1}";

            if (Id < 7)
            {
                //1st row
                offset.y = playerTransform.position.y + spriteSize.y * 2;
                offset.x = playerTransform.position.x + spriteSize.x * offsetMultiplier;

                bg.transform.position = offset;
            }
            else if (Id < 14)
            {
                //2nd row
                offset.y = playerTransform.position.y + spriteSize.y; ;
                offset.x = playerTransform.position.x + spriteSize.x * offsetMultiplier;

                bg.transform.position = offset;
            }
            else if (Id < 21)
            {
                //3rd row
                offset.y = playerTransform.position.y;
                offset.x = playerTransform.position.x + spriteSize.x * offsetMultiplier;

                bg.transform.position = offset;

                if (currentLevel.Restriction == MovementRestriction.None && offsetMultiplier == 0)
                    MoveTriggerTiles.Add(bg);

                if (currentLevel.Restriction == MovementRestriction.Vertical)
                    MoveTriggerTiles.Add(bg);
            }
            else if (Id < 28)
            {
                //4th row
                offset.y = playerTransform.position.y - spriteSize.y;
                offset.x = playerTransform.position.x + spriteSize.x * offsetMultiplier;

                bg.transform.position = offset;
            }
            else
            {
                //5th row
                offset.y = playerTransform.position.y - spriteSize.y * 2;
                offset.x = playerTransform.position.x + spriteSize.x * offsetMultiplier;

                bg.transform.position = offset;
            }

            if (currentLevel.Restriction == MovementRestriction.Horizontal)
            {
                if (offsetMultiplier == 0)
                    MoveTriggerTiles.Add(bg);
            }

            offsetMultiplier++;

            //if the offset multiplier is greater or equal to 3 set it to -2
            if (offsetMultiplier > 3)
                offsetMultiplier = -3;
        }

        SetTriggerTiles();
    }
    private void SetTriggerTiles()
    {
        if (MoveTriggerTiles.Count == 0)
        {
            Debug.LogException(new ArgumentNullException("moveTriggerTiles", "No tiles that are able to trigger the backGroundMovement"), this);
            return;
        }

        foreach(GameObject triggerTile in MoveTriggerTiles)
        {
            Collider2D centerTileCol = triggerTile.AddComponent<BoxCollider2D>();
            centerTileCol.isTrigger = true;

            Rigidbody2D centerTileRb = triggerTile.AddComponent<Rigidbody2D>();
            centerTileRb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void MoveTiles(Vector3 dir)
    {
        switch (currentLevel.Restriction)
        {
            case MovementRestriction.None:
                break;
            case MovementRestriction.Vertical:
                dir.x = 0;
                break;
            case MovementRestriction.Horizontal:
                dir.y = 0;
                break;
        }

        foreach(GameObject tile in Tiles)
        {
            if (tile == null)
                continue;

            tile.transform.position += Vector3.Scale(currentLevel.BgSprite.bounds.size, dir);
        }
    }
    #endregion

    private void SpawnLightSource()
    {
        
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
