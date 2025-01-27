using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Equipement
{
    [Header("Weapon")]
    [SerializeField] protected GameObject WeaponOriginal;
    protected List<GameObject> weaponsToUse = new();

    [Header("Spawn offset")]
    [SerializeField] protected float offsetFromPlayer;
 
    [Header("Weapon stats")]
    [Tooltip("Level of the weapon")]
    [SerializeField, Range(1, 5)] protected byte level;
    [SerializeField, Min(0)] protected int attack;

    [SerializeField, Min(0)] protected int projectileAmount;
    protected int currentProjectileAmount;
    [SerializeField, Min(0)] protected float speed;

    [Header("Time (in seconds)")]
    [Tooltip("Time between 2 salves of attack")]
    [SerializeField, Min(0.1f)] protected float cooldown;
    [Tooltip("Duration of the attack")]
    [SerializeField, Min(0)] protected float duration;

    protected LayerMask attackLayer = 1 << 6;

    public abstract void CreateNewProjectile();
    public abstract void StartAttack(MonoBehaviour player);
    public abstract void StopAttack(MonoBehaviour player);

    protected abstract IEnumerator AttackRoutine(MonoBehaviour player);
}
