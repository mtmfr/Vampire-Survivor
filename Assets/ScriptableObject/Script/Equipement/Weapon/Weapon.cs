using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : SO_Equipement
{
    [Header("Weapon")]
    [SerializeField] protected GameObject weaponOriginal;
    protected List<GameObject> weaponAmmo = new();

    [Header("Spawn offset")]
    [SerializeField] protected float offsetFromPlayer;
 
    [Header("Weapon stats")]
    [Tooltip("Level of the weapon")]
    [SerializeField, Min(1)] protected int level;
    [SerializeField] protected byte maxLevel;
    [SerializeField, Min(0)] protected int attack;

    [SerializeField, Min(0)] protected int projectileAmount;
    protected int currentProjectileAmount;
    [Tooltip("Speed at which the weapon moves. Not used on every weapon")]
    [SerializeField, Min(0)] protected float speed;

    [Header("Time (in seconds)")]
    [Tooltip("Time between 2 salvos of attack")]
    [SerializeField, Min(0.1f)] protected float cooldown;
    [Tooltip("delay between 2 ammo in a salvo")]
    [SerializeField, Min(0)] protected float delay;

    protected LayerMask attackLayer = 1 << 6;

    /// <summary>
    /// Create all the projectiles used by the wepon
    /// </summary>
    public abstract void CreateNewProjectile();

    /// <summary>
    /// Effect of the weapon level up
    /// </summary>
    public abstract void LevelUp();

    /// <summary>
    /// Logic of the attack
    /// </summary>
    public abstract IEnumerator AttackRoutine();
}
