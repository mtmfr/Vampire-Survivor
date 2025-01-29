using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacter", menuName = "Scriptable Objects/Character")]
public class SO_Character : ScriptableObject
{
    [field: SerializeField] public string characterName { get; protected set; }
    [field: SerializeField] public Sprite CharacterSprite { get; protected set; }
    [field: SerializeField] public RuntimeAnimatorController CharacterAnim { get; protected set; }
    [field: SerializeField] public Weapon StartingWeapon { get; protected set; }

    private void OnValidate()
    {
        #if UNITY_EDITOR
        characterName = name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif

    }
}
