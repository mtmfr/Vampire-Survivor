using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class SO_Enemy: ScriptableObject
{
    [Header("Experience")]
    [field: SerializeField] public XpPoint XpPoint { get; private set; }

    #region stats
    [Header("Stats")]
    [field: SerializeField, Min(0)] public int Health { get; private set; }
    [field: SerializeField, Min(0)] public int Attack { get; private set; }
    [field: SerializeField, Min(0)] public float Speed { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float KnockBack { get; private set; }
    #endregion

    [Header("Sprite")]
    [field: SerializeField] public Sprite sprite { get; private set; }
}
