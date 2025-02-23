using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<SO_EnemyWave> waves;
    private int enemyWavesId;

    private List<Vector3> enemySpawnPos = new();
    [SerializeField] private float spawnPosOffset;

    [SerializeField] private Reaper reaper;

    private void OnEnable()
    {
        LevelEvent.OnLevelSelected += GetEnemyWaves;
        SpawnerEvent.OnSpawnPosChanged += UpdateSpawnPos;

        TimerEvent.OnMinutesChange += SetCurrentWave;

        GameStateManager.OnGameStateChange += ControlSpawn;
        GameStateManager.OnGameStateChange += DeactivateEnemies;
    }

    private void OnDisable()
    {
        LevelEvent.OnLevelSelected -= GetEnemyWaves;
        SpawnerEvent.OnSpawnPosChanged -= UpdateSpawnPos;

        GameStateManager.OnGameStateChange -= ControlSpawn;
        GameStateManager.OnGameStateChange -= DeactivateEnemies;
    }

    #region Wave
    private void GetEnemyWaves(SO_Stage stage)
    {
        waves = stage.stageWaves;
    }

    private void ControlSpawn(GameState gameState)
    {
        if (gameState == GameState.InGame)
            StartCoroutine(CR_Spawn());
        else StopCoroutine(CR_Spawn());
    }

    private void SetCurrentWave(int id)
    {
        enemyWavesId = id;

        if (id != 0)
            StartCoroutine(CR_Spawn());
    }
    #endregion

    private void DeactivateEnemies(GameState gameState)
    {
        if (gameState != GameState.GameOver)
            return;

        List<Enemy> toDispawn = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();

        foreach (Enemy enemy in toDispawn)
        {
            enemy.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Coroutine responsible for spawning enemies in waves
    /// </summary>
    private IEnumerator CR_Spawn()
    {
        if (waves.Count <= enemyWavesId + 1)
        {
            Vector3 reaperSpawnPos = Camera.main.ScreenToWorldPoint(new Vector3(0.5f, 0.5f));
            if (ObjectPool.IsAnyObjectInactive(reaper))
            {
                GameObject reaperObject = ObjectPool.GetInactiveObject(reaper).gameObject;
                reaperObject.transform.position = reaperSpawnPos;
            }
            else
            {
                Instantiate(reaper.gameObject, reaperSpawnPos, Quaternion.identity);
            }
        }

        int enemiesInWave = waves[enemyWavesId].EnemiesInWave.Count;

        float spawnInterval = 60 / enemiesInWave;

        // Loop through each enemy to spawn
        for (int enemyId = 0; enemyId < enemiesInWave; enemyId++)
        {
            // Get the enemy to spawn based on the current wave's list
            Enemy enemyToSpawn = waves[enemyWavesId].EnemiesInWave[enemyId];

            if (enemyToSpawn == null)
                break;

            // Get the spawn position (can be a random or predefined position)
            Vector3 spawnPos = GetSpawnPosition();

            // Check if there are any inactive objects in the pool that can be reused
            if (!ObjectPool.IsAnyObjectInactive(enemyToSpawn))
            {
                // If no inactive objects are available, instantiate a new enemy at the spawn position
                Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
            }
            else
            {
                // If an inactive object is available in the pool, reuse it
                Enemy enemy = ObjectPool.GetInactiveObject(enemyToSpawn);

                // Set the position of the reused enemy and activate it
                enemy.transform.position = spawnPos;
                enemy.gameObject.SetActive(true);
            }

            // Wait for the specified interval before spawning the next enemy
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    #region SpawnPosition
    /// <summary>
    /// Get a random spawn position
    /// </summary>
    private Vector3 GetSpawnPosition()
    {
        if (enemySpawnPos.Count == 0)
            throw new ArgumentNullException("enemyPos.Count", "No object in enemySpawnPos");

        int randSpawnPosId = UnityEngine.Random.Range(0, enemySpawnPos.Count);

        Vector3 baseSpawnPos = enemySpawnPos[randSpawnPosId];

        float xPos = UnityEngine.Random.Range(baseSpawnPos.x - spawnPosOffset, baseSpawnPos.x + spawnPosOffset);
        float yPos = UnityEngine.Random.Range(baseSpawnPos.y - spawnPosOffset, baseSpawnPos.y + spawnPosOffset);

        return new Vector3(xPos, yPos);
    }

    private void UpdateSpawnPos(List<Vector3Int> spawnPositions)
    {
        enemySpawnPos.Clear();
        foreach(Vector3 spawnPosition in spawnPositions)
        {
            if (spawnPosition.HasNaanValue())
                continue;

            enemySpawnPos.Add(spawnPosition);
        }
    }
    #endregion
}

public static class SpawnerEvent
{
    public static event Action<GameObject> OnBecomeActive;
    public static void BecomeActive(GameObject gameObject)
    {
        OnBecomeActive?.Invoke(gameObject);
    }

    public static event Action<GameObject> OnBecomeInactive;
    public static void BecomeInactive(GameObject gameObject)
    {
        OnBecomeInactive?.Invoke(gameObject);
    }

    public static event Action<List<Vector3Int>> OnSpawnPosChanged;
    public static void SpawnPosChanged(List<Vector3Int> pos)
    {
        OnSpawnPosChanged?.Invoke(pos);
    }
}
