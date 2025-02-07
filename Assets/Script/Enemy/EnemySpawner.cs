using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<SO_EnemyWave> waves;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        
    }

    private void SpawnEnemy()
    {

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
