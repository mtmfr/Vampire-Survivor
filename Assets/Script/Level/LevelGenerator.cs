using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private SO_Stage currentLevel;

    [SerializeField] private GameObject stageBg;
    [SerializeField] private GameObject lightSource;
    private List<GameObject> Tiles { get; } = new(35);
    private List<GameObject> lightSources = new();

    private List<Vector3Int> spawnPositions = new();

    private GameObject triggerTile;
    private Transform playerTransform;

    [Tooltip("The chance of spawning lightSources (in %)")]
    [SerializeField, Range(0, 100)] private int lightsourceSpawnChance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void OnEnable()
    {
        LevelEvent.OnLevelSelected += UpdateCurrentLevel;
        LevelEvent.OnLevelSpawn += SpawnLevelTiles;
        LevelEvent.OnLevelSpawn += SetLevelTimer;

        LevelTile.OnPlayerLeft += MoveTiles;
    }

    private void OnDisable()
    {
        LevelEvent.OnLevelSelected -= UpdateCurrentLevel;
        LevelEvent.OnLevelSpawn -= SpawnLevelTiles;
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
    private void SpawnLevelTiles()
    {
        //Set the current stage backGround
        stageBg.GetComponent<SpriteRenderer>().sprite = currentLevel.BgSprite;

        //instantiate the tiles 
        for (int tile = 0; tile < Tiles.Capacity; tile++)
        {
            Tiles.Add(Instantiate(stageBg));
        }

        if (Tiles.Count == 0)
            Debug.LogException(new ArgumentNullException("bgPlacement", "No background have been found"), this);

        //Get the sprite size
        Vector3 spriteSize = currentLevel.BgSprite.bounds.size;

        //initialize the offset
        Vector3 offset = new();

        //initialize the offset multiplier on x
        int xOffset = -3;
        int yOffset;

        for (int Id = 0; Id < Tiles.Count; Id++)
        {
            GameObject bg = Tiles[Id];

            bg.name = $"BackGround nb {Id + 1}";

            if (Id < 7)
            {
                //1st row
                yOffset = 2;
                offset.Set(playerTransform.position.x + spriteSize.x * xOffset, playerTransform.position.y + spriteSize.y * yOffset, 0);

                bg.transform.position = offset;
            }
            else if (Id < 14)
            {
                //2nd row
                yOffset = 1;
                offset.Set(playerTransform.position.x + spriteSize.x * xOffset, playerTransform.position.y + spriteSize.y, 0);

                bg.transform.position = offset;
            }
            else if (Id < 21)
            {
                //3rd row
                yOffset = 0;
                offset.Set(playerTransform.position.x + spriteSize.x * xOffset, playerTransform.position.y, 0);

                bg.transform.position = offset;

                if (xOffset == 0)
                    triggerTile = bg;
            }
            else if (Id < 28)
            {
                //4th row
                yOffset = -1;
                offset.Set(playerTransform.position.x + spriteSize.x * xOffset, playerTransform.position.y - spriteSize.y, 0);

                bg.transform.position = offset;
            }
            else
            {
                //5th row
                yOffset = -2;
                offset.Set(playerTransform.position.x + spriteSize.x * xOffset, playerTransform.position.y - spriteSize.y * 2, 0);

                bg.transform.position = offset;
            }

            GetSpawnTile(bg, xOffset, yOffset);

            xOffset++;

            //if the offset multiplier is greater than 3 set it to -3
            if (xOffset > 3)
                xOffset = -3;
        }

        SetTriggerTiles();
    }

    /// <summary>
    /// Move the tiles in the desired direction
    /// </summary>
    /// <param name="dir">The direction of the movement</param>
    private void MoveTiles(Vector3 dir)
    {
        foreach(GameObject tile in Tiles)
        {
            if (tile == null)
                continue;

            Vector3 nextPos = Vector3.Scale(currentLevel.BgSprite.bounds.size, dir);

            if (!nextPos.HasNaanValue(nextPos))
                tile.transform.position += nextPos;
        }

        UpdateSpawnPositions(dir);

        if (ShouldSpawnLightSource())
            SpawnLightSource();
    }
    #endregion

    #region setTiles
    private void GetSpawnTile(GameObject bg, int xPos, int yPos)
    {
        var tile = bg.GetComponent<LevelTile>();
        if (MathF.Abs(xPos) == 3 && Mathf.Abs(yPos) != 2)
        {
            SetSpawnPos(tile, 1 * (int)Mathf.Sign(xPos), 0);
        }
        if (MathF.Abs(xPos) != 3 && Mathf.Abs(yPos) == 2)
        {
            SetSpawnPos(tile, 0, 1 * (int)Mathf.Sign(yPos));
        }
        if (MathF.Abs(xPos) == 3 && Mathf.Abs(yPos) == 2)
        {
            SetSpawnPos(tile, 1 * (int)Mathf.Sign(xPos), 1 * (int)Mathf.Sign(yPos));
        }
    }

    private void SetSpawnPos(LevelTile tileToSet, int xValue, int yValue)
    {
        tileToSet.SetIsSpawner(true);

        tileToSet.SetPosId(xValue, yValue);

        spawnPositions.Add(Vector3Int.CeilToInt(tileToSet.gameObject.transform.position));
    }

    /// <summary>
    /// SetThe tiles that will control the Movement of the tiles
    /// </summary>
    private void SetTriggerTiles()
    {
        Collider2D centerTileCol = triggerTile.AddComponent<BoxCollider2D>();
        centerTileCol.isTrigger = true;

        Rigidbody2D centerTileRb = triggerTile.AddComponent<Rigidbody2D>();
        centerTileRb.bodyType = RigidbodyType2D.Static;
    }
    #endregion

    #region LightSource
    private bool ShouldSpawnLightSource()
    {
        float shouldSpawn = Mathf.Round(UnityEngine.Random.value) * 100;

        return shouldSpawn <= lightsourceSpawnChance;
    }

    /// <summary>
    /// spawn a light source
    /// </summary>
    private void SpawnLightSource()
    {
        LightSource lightSourceToSpawn = lightSource.GetComponent<LightSource>();

        Vector3Int spawnPos = GetRandomSpawnPosition();        

        if (lightSources.Count == 0)
        {
            GameObject spawnedLightSource = Instantiate(lightSource, spawnPos, Quaternion.identity);
            lightSources.Add(spawnedLightSource);
            return;
        }

        //check if there already is an active lightHouse at this position
        Vector3Int closestLightSource = Vector3Int.CeilToInt(lightSources.OrderBy(lightsource => Vector3Int.Distance(Vector3Int.CeilToInt(lightSource.transform.position), spawnPos)).FirstOrDefault().transform.position);

        //return if there is
        if (closestLightSource == spawnPos)
            return;

        //Check if there are inactive lightSources
        //take an inactive one if there is, instantiate one otherwise
        if (ObjectPool.IsAnyObjectInactive(lightSourceToSpawn))
        {
            lightSourceToSpawn = ObjectPool.GetInactiveObject(lightSourceToSpawn);
            lightSourceToSpawn.gameObject.transform.position = spawnPos;
            lightSourceToSpawn.gameObject.SetActive(true);
        }
        else
        {
            GameObject spawnedLightSource = Instantiate(lightSource, spawnPos, Quaternion.identity);
            lightSources.Add(spawnedLightSource);
        }
    }
    #endregion

    #region Spawn position
    private void UpdateSpawnPositions(Vector2 dir)
{
    // Clear the existing spawnPositions list to prepare for new values.
    spawnPositions.Clear();

    // Create a new list containing all the spawner tiles from the Tiles collection.
    List<GameObject> spawners = Tiles.Where(tile =>
    {
        // Try to get the LevelTile component from the tile.
        bool isTile = tile.TryGetComponent(out LevelTile levelTile);

        // Check if the tile is a spawner.
        bool isTileSpawner = levelTile.isSpawner;

        // Return true if it's a valid tile and is a spawner.
        return isTile && isTileSpawner;
    }).ToList();

    // Iterate through each spawner tile.
    foreach (GameObject spawner in spawners)
    {
        // Skip the spawner if its position is invalid (HasNaanValue checks for NaN or undefined values).
        if (spawner.transform.position.HasNaanValue(spawner.transform.position))
            continue;

        // Get the LevelTile component from the spawner.
        var tile = spawner.GetComponent<LevelTile>();

        // Skip the spawner if its position ID doesn't match the rounded direction.
        if (tile.posId != Vector2Int.CeilToInt(dir))
            continue;

        // Add the spawner's position (rounded up to the nearest integer) to the spawnPositions list.
        spawnPositions.Add(Vector3Int.CeilToInt(spawner.transform.position));
    }

    // Trigger an event to notify that the spawn positions have changed.
    SpawnerEvent.SpawnPosChanged(spawnPositions);
}

    /// <summary>
    /// Get the position of the tiles that are spawner
    /// </summary>
    /// <returns></returns>
    private Vector3Int GetRandomSpawnPosition()
    {
        if (spawnPositions.Count == 0)
            throw new NullReferenceException("no object in spawnPosition");

        //Take a random position as vector3Int
        int randomId = UnityEngine.Random.Range(0, spawnPositions.Count - 1);

        if (randomId < 0 || randomId > spawnPositions.Count - 1)
            Debug.Log(randomId);

        return spawnPositions[randomId];
    }
    #endregion
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
