using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int Attack { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
}
