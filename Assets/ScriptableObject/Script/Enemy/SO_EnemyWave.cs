using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_EnemyWave", menuName = "Scriptable Objects/SO_EnemyWave")]
public class SO_EnemyWave : ScriptableObject
{
    [Header("Enemies")]
    [field: SerializeField] public List<Enemy> EnemiesInWave { get; private set; }

    [field: SerializeField] public List<Enemy> BossInWave { get; private set; }
}
