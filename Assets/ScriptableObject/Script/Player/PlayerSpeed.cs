using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSpeed", menuName = "Scriptable Objects/Player/PlayerSpeed")]
public class PlayerSpeed : ScriptableObject
{
    [field: SerializeField] private int speed;
    public int Speed { get { return speed; } }
}
