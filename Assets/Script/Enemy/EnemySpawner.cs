using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPos;

    [SerializeField] private List<GameObject> enemies;

    private List<GameObject> activeEnemies = new();
    private List<GameObject> inactiveEnemies = new();

    [Tooltip("The time between 2 search to see if there are any inactive enemies")]
    [SerializeField] private float enemySearchCooldown;
    [SerializeField, Range(0, 1)] private float enemySpawnCooldown;

    private IEnumerator inactiveEnemySearch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inactiveEnemySearch = CheckForInactiveEnemies();
    }

    private void OnEnable()
    {
        GameStateManager.OnGameStateChange += UpdateInactiveEnemySearch;

        PlayerEvent.OnLevelUp += CreateNewEnemy;

        SpawnerEvent.OnBecomeActive += AddObjectToActiveList;
        SpawnerEvent.OnBecomeInactive += AddObjectToInactiveList;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= UpdateInactiveEnemySearch;

        PlayerEvent.OnLevelUp -= CreateNewEnemy;

        SpawnerEvent.OnBecomeActive -= AddObjectToActiveList;
        SpawnerEvent.OnBecomeInactive -= AddObjectToInactiveList;
    }

    /// <summary>
    /// Add an object to the activeEnemy list.
    /// And remove it from the inactive list.
    /// </summary>
    /// <param name="gameObject">the object to add</param>
    private void AddObjectToActiveList(GameObject gameObject)
    {
        if (activeEnemies.Contains(gameObject))
            return;

        if (inactiveEnemies.Contains(gameObject))
            inactiveEnemies.Remove(gameObject);

        activeEnemies.Add(gameObject);
    }

    /// <summary>
    /// Add an object to the inactiveEnemy list.
    /// And remove it from the active list.
    /// </summary>
    /// <param name="gameObject"></param>
    private void AddObjectToInactiveList(GameObject gameObject)
    {
        if (inactiveEnemies.Contains(gameObject))
            return;

        if (activeEnemies.Contains(gameObject))
            activeEnemies.Remove(gameObject);

        inactiveEnemies.Add(gameObject);
    }

    /// <summary>
    /// Get a random spawn position for enemies
    /// </summary>
    /// <returns>A random position</returns>
    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnPos.Count == 0)
            Debug.LogError("No position in List", this);

        int posId = UnityEngine.Random.Range(0, spawnPos.Count - 1);

        return spawnPos[posId].position;
    }

    /// <summary>
    /// Get a random enemy from the enemy List
    /// </summary>
    /// <returns>A random enemy</returns>
    private GameObject GetRandomEnemy()
    {
        if (enemies.Count == 0)
            Debug.LogException(new ArgumentNullException("enemies", "No enemy in enemies"), this);

        int enemyId = UnityEngine.Random.Range(0, enemies.Count - 1);

        return enemies[enemyId];
    }

    /// <summary>
    /// create a new enemy for each level the player Ggined
    /// </summary>
    /// <param name="playerLevel">The current level of the player</param>
    private void CreateNewEnemy(int playerLevel)
    {
        for (int enemyToSpawn = 0; enemyToSpawn < playerLevel; enemyToSpawn++)
        {
            Instantiate(GetRandomEnemy(), GetRandomSpawnPosition(), Quaternion.identity);
        }
    }

    /// <summary>
    /// Set wether or not the spawner should check for inactive enemies.
    /// </summary>
    /// <param name="state">the current state of the game</param>
    private void UpdateInactiveEnemySearch(GameState state)
    {
        if (state != GameState.InGame)
            StopCoroutine(inactiveEnemySearch);

        StartCoroutine(CheckForInactiveEnemies());
    }

    private IEnumerator CheckForInactiveEnemies()
    {
        List<GameObject> enemyToSpawn = new(inactiveEnemies);
        foreach(GameObject enemy in enemyToSpawn)
        {
            enemy.transform.position = GetRandomSpawnPosition();
            enemy.SetActive(true);
        }
        yield return new WaitForSeconds(enemySearchCooldown);
        StartCoroutine(CheckForInactiveEnemies());
    }
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
}
