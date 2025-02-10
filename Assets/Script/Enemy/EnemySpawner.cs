using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<SO_EnemyWave> waves;
    private int enemyWavesId;

    private List<Vector3> enemySpawnPos = new();
    [SerializeField] private float spawnPosOffset;


    private void OnEnable()
    {
        LevelEvent.OnLevelSelected += GetEnemyWaves;
        SpawnerEvent.OnSpawnPosChanged += UpdateSpawnPos;

        TimerEvent.OnMinutesChange += SetCurrentWave;
    }

    private void OnDisable()
    {
        LevelEvent.OnLevelSelected -= GetEnemyWaves;
        SpawnerEvent.OnSpawnPosChanged -= UpdateSpawnPos;

    }

    #region Wave
    private void GetEnemyWaves(SO_Stage stage)
    {
        waves = stage.stageWaves;
    }

    private void SetCurrentWave(int id)
    {
        enemyWavesId = id;
        StopCoroutine(Spawn());

        StartCoroutine(Spawn());
    }
    #endregion

    /// <summary>
    /// Coroutine responsible for spawning enemies in waves
    /// </summary>
    private IEnumerator Spawn()
    {
        // Get the number of enemies to spawn in the current wave
        int enemiesInWave = waves[enemyWavesId].EnemiesInWave.Count;

        // Calculate the time interval between each spawn based on the number of enemies
        float spawnInterval = 60 / enemiesInWave;

        // Loop through each enemy to spawn
        for (int enemyId = 0; enemyId <= enemiesInWave; enemyId++)
        {
            // Get the enemy to spawn based on the current wave's list
            Enemy enemyToSpawn = waves[enemyWavesId].EnemiesInWave[enemyId];

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
    /// This function returns a random spawn position for an enemy.
    /// </summary>
    private Vector3 GetSpawnPosition()
    {
        // Checks if there are no positions in the enemySpawnPos list, and if so, throws an exception.
        if (enemySpawnPos.Count == 0)
            throw new ArgumentNullException("enemyPos.Count", "No object in enemySpawnPos");

        // Randomly selects an index from the enemySpawnPos list.
        int randSpawnPosId = UnityEngine.Random.Range(0, enemySpawnPos.Count - 1);

        // Retrieves the base spawn position from the list using the randomly selected index.
        Vector3 baseSpawnPos = enemySpawnPos[randSpawnPosId];

        // Generates random x and y coordinates within a defined offset from the base spawn position.
        float xPos = UnityEngine.Random.Range(baseSpawnPos.x - spawnPosOffset, baseSpawnPos.x + spawnPosOffset);
        float yPos = UnityEngine.Random.Range(baseSpawnPos.y - spawnPosOffset, baseSpawnPos.y + spawnPosOffset);

        // Returns the newly calculated random spawn position as a Vector3.
        return new Vector3(xPos, yPos);
    }

    private void UpdateSpawnPos(List<Vector3Int> spawnPositions)
    {
        enemySpawnPos.Clear();
        foreach(Vector3 spawnPosition in spawnPositions)
        {
            if (spawnPosition.HasNaanValue(spawnPosition))
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
