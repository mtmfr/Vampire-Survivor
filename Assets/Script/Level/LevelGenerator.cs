using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private SO_Stage currentLevel;

    [SerializeField] private LevelTile stageTile;
    [SerializeField] private GameObject lightSource;

    [SerializeField] private Vector2Int gridSize;
    private List<LevelTile> tiles = new();

    private List<GameObject> lightSources = new();

    private List<Vector3Int> spawnPositions = new();

    [Tooltip("The chance of spawning lightSources (in %)")]
    [SerializeField, Range(0, 100)] private int lightsourceSpawnChance;

    private bool hasTileBeenSpawned = false;

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

        stageTile.GetComponent<SpriteRenderer>().sprite = currentLevel.BgSprite;


        if (hasTileBeenSpawned == false)
        {
            SpawnLevelTiles();
            hasTileBeenSpawned = true;
        }
    }

    #region level Tiles

    /// <summary>
    /// Spawn the tiles of the game
    /// </summary>
    private void SpawnLevelTiles()
    {
        Vector3 spriteSize = currentLevel.BgSprite.bounds.size;

        Vector3 offset = new();

        for (int row = 0; row < gridSize.x; row++)
        {
            for (int column = 0; column < gridSize.y; column++)
            {
                offset = Vector3.Scale(spriteSize, new Vector3(row - gridSize.x / 2, column - gridSize.y / 2));
                //offset = new Vector3(row - gridSize.x/2, column - gridSize.y/2, 0);

                Vector3 spawnPosition = transform.position + offset;

                LevelTile tileToSpawn = Instantiate(stageTile, spawnPosition, Quaternion.Euler(Vector3.zero));
                tiles.Add(tileToSpawn);
                tileToSpawn.name = $"x:{row}, y:{column}";

                if (offset == Vector3.zero)
                    SetTriggerTiles(tileToSpawn);
                spawnPosition = transform.position + offset;

                //Set the spawner tiles
                //Update the positions id of the tile depending of it's offset
                //Define a spawner tile as one which it's xPosIs + YPosId > 0
                int xPosId = 0;
                int yPosId = 0;

                float currentXOffset = row - gridSize.x / 2;
                float minXOffest = 0 - gridSize.x / 2;
                float maxXOffset = 0 + gridSize.x / 2;

                if (currentXOffset == minXOffest)
                    xPosId = -1;
                else if (currentXOffset == maxXOffset)
                    xPosId = 1;

                float currentYOffset = column - gridSize.y / 2;
                float minYOffset = 0 - gridSize.y / 2;
                float maxYOffset = 0 + gridSize.y / 2;

                if (currentYOffset == minYOffset)
                    yPosId = -1;
                else if(currentYOffset == maxYOffset)
                    yPosId = 1;

                if (Mathf.Abs(xPosId) + Mathf.Abs(yPosId) > 0)
                    SetSpawnPos(tileToSpawn, xPosId, yPosId);
            }
        }

        StartSpawnPos();
    }

    /// <summary>
    /// Move the tiles in the desired direction
    /// </summary>
    /// <param name="dir">The direction of the movement</param>
    private void MoveTiles(Vector3 dir)
    {
        foreach(LevelTile tile in tiles)
        {
            if (tile == null)
                continue;

            Vector3 nextPos = Vector3.Scale(currentLevel.BgSprite.bounds.size, dir);

            if (!nextPos.HasNaanValue())
                tile.transform.position += nextPos;
        }

        UpdateSpawnPositions(dir);

        if (ShouldSpawnLightSource())
            SpawnLightSource();
    }
    #endregion

    #region setTiles
    /// <summary>
    /// Set the data of the spawner tile
    /// </summary>
    private void SetSpawnPos(LevelTile spawnerTile, int xPosId, int yPosId)
    {
        spawnerTile.SetIsSpawner(true);

        spawnerTile.SetPosId(xPosId, yPosId);

        spawnPositions.Add(Vector3Int.CeilToInt(spawnerTile.transform.position));
    }

    /// <summary>
    /// Set the tile that will control the movement of the tiles
    /// </summary>
    private void SetTriggerTiles(LevelTile triggerTile)
    {
        Collider2D centerTileCol = triggerTile.gameObject.AddComponent<BoxCollider2D>();
        centerTileCol.isTrigger = true;

        Rigidbody2D centerTileRb = triggerTile.gameObject.AddComponent<Rigidbody2D>();
        centerTileRb.bodyType = RigidbodyType2D.Static;
    }
    #endregion

    #region LightSource
    /// <summary>
    /// Get a random number and check if a lightsource can be spawned
    /// </summary>
    /// <returns>true if a lightsource can be spawned.
    /// <br>false otherwise</br></returns>
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
        LightSource lightSourceToSpawn = lightSource.GetComponent<LightSource>();

        Vector3Int spawnPos = GetRandomSpawnPosition();

        if (lightSources.Count == 0)
        {
            GameObject spawnedLightSource = Instantiate(lightSource, spawnPos, Quaternion.identity);
            lightSources.Add(spawnedLightSource);
            return;
        }

        Vector3Int closestLightSource = Vector3Int.CeilToInt(lightSources
            .OrderBy(lightsource => Vector3Int.Distance(Vector3Int.CeilToInt(lightSource.transform.position), spawnPos))
            .FirstOrDefault()
            .transform.position);

        if (closestLightSource == spawnPos)
            return;

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

    /// <summary>
    /// Deactivate all active lightsources on game over
    /// </summary>
    /// <param name="gameState"></param>
    private void DeactivateLightSource(GameState gameState)
    {
        if (gameState != GameState.GameOver)
            return;

        foreach(LightSource lightSource in ObjectPool.GetActiveObjects<LightSource>())
        {
            lightSource.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Spawn position
    private void StartSpawnPos()
    {
        spawnPositions.Clear();

        foreach (LevelTile spawnerTile in tiles)
        {
            if (!spawnerTile.isSpawner)
                continue;

            if (spawnerTile.transform.position.HasNaanValue())
                continue;

            spawnPositions.Add(Vector3Int.CeilToInt(spawnerTile.transform.position));
        }

        SpawnerEvent.SpawnPosChanged(spawnPositions);
    }

    private void UpdateSpawnPositions(Vector2 dir)
    {
        spawnPositions.Clear();

        foreach (LevelTile spawnerTile in tiles)
        {
            if (spawnerTile == null)
                continue;

            if (spawnerTile.transform.position.HasNaanValue())
                continue;

            if (spawnerTile.posId != Vector2Int.CeilToInt(dir))
                continue;

            spawnPositions.Add(Vector3Int.CeilToInt(spawnerTile.transform.position));
        }

        SpawnerEvent.SpawnPosChanged(spawnPositions);
    }

    /// <summary>
    /// Get a random poisition
    /// </summary>
    private Vector3Int GetRandomSpawnPosition()
    {
        if (spawnPositions.Count == 0)
            throw new NullReferenceException("no object in spawnPosition");

        int randomId = UnityEngine.Random.Range(0, spawnPositions.Count);

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
