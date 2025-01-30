using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrentLevel", menuName = "Scriptable Objects/CurrentLevel")]
public class SO_CurrentLevel : ScriptableObject
{
    [field: SerializeField] public string levelName;
    [field: SerializeField] public GameObject LevelBg { get; private set; }
    [field: SerializeField] public Sprite BgSprite { get; private set; }
}
