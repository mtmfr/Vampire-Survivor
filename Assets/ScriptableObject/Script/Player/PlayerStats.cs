using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int Speed { get; private set; }
    [field: SerializeField] public int Attack { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
}
