using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHp", menuName = "Scriptable Objects/Player/PlayerHp")]
public class PlayerHp : ScriptableObject
{
    [field: SerializeField] private int playerMaxHp;
    public int PlayerMaxHp { get { return playerMaxHp; } }
}
