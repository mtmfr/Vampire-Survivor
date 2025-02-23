using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SO_Weapon : SO_Equipement
{
    [Header("Weapon")]
    [field: SerializeField] public GameObject weaponOriginal { get; protected set; }

    [Header("Spawn offset")]
    [field: SerializeField] public float offset { get; protected set; }
 
    [Header("Weapon stats")]
    [Tooltip("Level of the weapon")]
    [SerializeField, Min(1)] protected int level;
    [field: SerializeField, Min(1)] public int attack { get; protected set; }
    [field: SerializeField] public int pierce { get; protected set; }

    [field: SerializeField, Min(0)] public int projectileAmount { get; protected set; }

    [Tooltip("Speed at which the weapon moves. Not used on every weapon")]
    [field: SerializeField, Min(0)] public float speed { get; protected set; }

    [Header("Time (in seconds)")]
    [Tooltip("Time between 2 salves of attack")]
    [field: SerializeField, Min(0.1f)] public float cooldown { get; protected set; }
    [Tooltip("delay between 2 ammo in a salves")]
    [field: SerializeField, Min(0)] public float delay {  get; protected set; }

    public LayerMask attackLayer { get; private set; } = 1 << 6;
}
