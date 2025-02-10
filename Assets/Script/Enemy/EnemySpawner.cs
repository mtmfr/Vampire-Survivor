using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<SO_EnemyWave> waves;

    private List<Vector3Int> enemySpawnPos = new();
    [SerializeField] private float spawnOffset;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        SpawnerEvent.OnSpawnPosChanged += UpdateSpawnPos;
    }

    private void OnDisable()
    {
        
    }

    private void SpawnEnemy()
    {

    }

    private void UpdateSpawnPos(List<Vector3Int> spawnPos)
    {
        enemySpawnPos.Clear();
        enemySpawnPos = spawnPos;
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

    public static event Action<List<Vector3Int>> OnSpawnPosChanged;
    public static void SpawnPosChanged(List<Vector3Int> pos)
    {
        OnSpawnPosChanged?.Invoke(pos);
    }
}
