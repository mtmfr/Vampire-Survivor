using UnityEngine;

[CreateAssetMenu(fileName = "XpScriptableObject", menuName = "Scriptable Objects/XpScriptableObject")]
public class XpScriptableObject : ScriptableObject
{
    [field: SerializeField] public int xpGiven { get; private set; }
    [field: SerializeField] public Sprite xpSprite { get; private set; }
}
