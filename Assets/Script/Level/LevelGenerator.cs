using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private bool hasTileBeenSpawned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;

        // Instantiate the background tiles and add them to the Tiles list.
        for (int tile = 0; tile < Tiles.Capacity; tile++)
        {
            Tiles.Add(Instantiate(stageBg));
        }
    }

    private void OnEnable()
    {
        LevelEvent.OnLevelSelected += UpdateCurrentLevel;

        LevelTile.OnPlayerLeft += MoveTiles;

        GameStateManager.OnGameStateChange += DeactivateLightSource;
    }

    private void OnDisable()
    {
        LevelEvent.OnLevelSelected -= UpdateCurrentLevel;

        LevelTile.OnPlayerLeft -= MoveTiles;

        GameStateManager.OnGameStateChange -= DeactivateLightSource;
    }

    private void UpdateCurrentLevel(SO_Stage currentLevel)
    {
        this.currentLevel = currentLevel;
        Timer.maxTime = currentLevel.Duration;

        // If no tiles have been instantiated, log an exception.
        if (Tiles.Count == 0)
            Debug.LogException(new ArgumentNullException("bgPlacement", "No background have been found"), this);

        foreach (GameObject tile in Tiles)
        {
            tile.GetComponent<SpriteRenderer>().sprite =  currentLevel.BgSprite;
        }

        if (hasTileBeenSpawned == false)
        {
            SpawnLevelTiles();
            hasTileBeenSpawned = true;
        }
    }

    #region level Tiles

    /// <summary>
    /// This function is responsible for spawning and positioning the level tiles (background) in the game.
    /// </summary>
    private void SpawnLevelTiles()
    {
        // Get the size of the background sprite to help with tile positioning.
        Vector3 spriteSize = currentLevel.BgSprite.bounds.size;

        // Initialize a new Vector3 for offset calculations.
        Vector3 offset = new();

        // Initialize the offset multiplier for the x-axis (starting at -3).
        int xOffset = -3;
        int yOffset;

        // Loop through all the tiles and position them based on their index.
        for (int Id = 0; Id < Tiles.Count; Id++)
        {
            GameObject tile = Tiles[Id];

            // Set the name of the tile (background) for identification.
            tile.name = $"BackGround nb {Id + 1}";

            if (Id < 7)
            {
                // First row of tiles.
                yOffset = 2;
                offset.Set(playerTransform.position.x + spriteSize.x * xOffset, playerTransform.position.y + spriteSize.y * yOffset, 0);

                tile.transform.position = offset;
            }
            else if (Id < 14)
            {
                // Second row of tiles.
                yOffset = 1;
                offset.Set(playerTransform.position.x + spriteSize.x * xOffset, playerTransform.position.y + spriteSize.y, 0);

                tile.transform.position = offset;
            }
            else if (Id < 21)
            {
                // Third row of tiles.
                yOffset = 0;
                offset.Set(playerTransform.position.x + spriteSize.x * xOffset, playerTransform.position.y, 0);

                tile.transform.position = offset;

                // If it's the middle tile (xOffset == 0), set it as the trigger tile.
                if (xOffset == 0)
                    triggerTile = tile;
            }
            else if (Id < 28)
            {
                // Fourth row of tiles.
                yOffset = -1;
                offset.Set(playerTransform.position.x + spriteSize.x * xOffset, playerTransform.position.y - spriteSize.y, 0);

                tile.transform.position = offset;
            }
            else
            {
                // Fifth row of tiles.
                yOffset = -2;
                offset.Set(playerTransform.position.x + spriteSize.x * xOffset, playerTransform.position.y - spriteSize.y * 2, 0);

                tile.transform.position = offset;
            }

            // Call the method to adjust additional settings for each spawned tile.
            GetSpawnTile(tile, xOffset, yOffset);

            // Increment the xOffset for the next tile.
            xOffset++;

            // If the xOffset exceeds 3, reset it back to -3 to create a tiling effect.
            if (xOffset > 3)
                xOffset = -3;
        }

        // Finalize the setup by setting the trigger tiles for further game logic.
        SetTriggerTiles();

        // Start the initial spawning process or logic for positioning.
        StartSpawnPos();
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
    /// <summary>
    /// This function is responsible for determining the spawn position of a background tile based on its x and y position.
    /// </summary>
    private void GetSpawnTile(GameObject bg, int xPos, int yPos)
    {
        // Get the LevelTile component from the background object (bg).
        var tile = bg.GetComponent<LevelTile>();

        // If the absolute value of xPos is 3 and the absolute value of yPos is not 2, set spawn position along the x-axis.
        if (MathF.Abs(xPos) == 3 && Mathf.Abs(yPos) != 2)
        {
            // Set the spawn position with an x offset based on the sign of xPos (either 1 or -1), and no offset on the y-axis.
            SetSpawnPos(tile, 1 * (int)Mathf.Sign(xPos), 0);
        }

        // If the absolute value of yPos is 2 and the absolute value of xPos is not 3, set spawn position along the y-axis.
        if (MathF.Abs(xPos) != 3 && Mathf.Abs(yPos) == 2)
        {
            // Set the spawn position with an offset on the y-axis based on the sign of yPos (either 1 or -1), and no offset on the x-axis.
            SetSpawnPos(tile, 0, 1 * (int)Mathf.Sign(yPos));
        }

        // If both xPos and yPos have an absolute value of 3 and 2 respectively, set spawn position with offsets on both axes.
        if (MathF.Abs(xPos) == 3 && Mathf.Abs(yPos) == 2)
        {
            // Set the spawn position with both x and y offsets, based on the signs of xPos and yPos.
            SetSpawnPos(tile, 1 * (int)Mathf.Sign(xPos), 1 * (int)Mathf.Sign(yPos));
        }
    }

    /// <summary>
    /// This function sets the spawn position for a LevelTile object.
    /// </summary>
    private void SetSpawnPos(LevelTile tileToSet, int xValue, int yValue)
    {
        // Mark the tile as a spawn point by setting its "IsSpawner" flag to true.
        tileToSet.SetIsSpawner(true);

        // Set the position ID for the tile using the provided x and y values.
        tileToSet.SetPosId(xValue, yValue);

        // Add the tile's position (rounded to the nearest integer) to the list of spawn positions.
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
        float shouldSpawn = UnityEngine.Random.value * 100;

        return shouldSpawn < lightsourceSpawnChance;
    }

    /// <summary>
    /// This function is responsible for spawning a light source at a random position or reusing an inactive one if available.
    /// </summary>
    private void SpawnLightSource()
    {
        // Get the LightSource component from the lightSource GameObject.
        LightSource lightSourceToSpawn = lightSource.GetComponent<LightSource>();

        // Get a random spawn position for the light source.
        Vector3Int spawnPos = GetRandomSpawnPosition();

        // If there are no active light sources, instantiate a new one at the spawn position and add it to the list.
        if (lightSources.Count == 0)
        {
            GameObject spawnedLightSource = Instantiate(lightSource, spawnPos, Quaternion.identity);
            lightSources.Add(spawnedLightSource);
            return;
        }

        // Check if there is already an active light source near the spawn position.
        // Find the closest light source by calculating the distance to each one.
        Vector3Int closestLightSource = Vector3Int.CeilToInt(lightSources
            .OrderBy(lightsource => Vector3Int.Distance(Vector3Int.CeilToInt(lightSource.transform.position), spawnPos))
            .FirstOrDefault()
            .transform.position);

        // If a light source is already located at the spawn position, do not spawn a new one and exit the function.
        if (closestLightSource == spawnPos)
            return;

        // Check if there are any inactive light sources available in the Object Pool.
        // If an inactive light source is found, reuse it by setting its position and activating it.
        if (ObjectPool.IsAnyObjectInactive(lightSourceToSpawn))
        {
            // Get the inactive light source from the object pool and set its position to the spawn position.
            lightSourceToSpawn = ObjectPool.GetInactiveObject(lightSourceToSpawn);
            lightSourceToSpawn.gameObject.transform.position = spawnPos;
            lightSourceToSpawn.gameObject.SetActive(true);
        }
        else
        {
            // If no inactive light sources are available, instantiate a new one and add it to the list.
            GameObject spawnedLightSource = Instantiate(lightSource, spawnPos, Quaternion.identity);
            lightSources.Add(spawnedLightSource);
        }
    }   

    private void DeactivateLightSource(GameState gameState)
    {
        if (gameState != GameState.GameOver)
            return;

        List<LightSource> lightSources = FindObjectsByType<LightSource>(FindObjectsSortMode.None).ToList();

        foreach(LightSource lightSource in lightSources)
        {
            lightSource.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Spawn position
    private void StartSpawnPos()
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

            // Add the spawner's position (rounded up to the nearest integer) to the spawnPositions list.
            spawnPositions.Add(Vector3Int.CeilToInt(spawner.transform.position));
        }

        // Trigger an event to notify that the spawn positions have changed.
        SpawnerEvent.SpawnPosChanged(spawnPositions);
    }

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
