using UnityEngine;

public abstract class SO_Character : ScriptableObject
{
    [field: SerializeField] public Sprite CharacterSprite { get; protected set; }
    [field: SerializeField] public RuntimeAnimatorController CharacterAnim { get; protected set; }
    [field: SerializeField] public Weapon StartingWeapon { get; protected set; }
}
