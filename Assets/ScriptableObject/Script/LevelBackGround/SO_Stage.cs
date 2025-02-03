using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrentLevel", menuName = "Scriptable Objects/CurrentLevel")]
public class SO_Stage : ScriptableObject
{
    [field: SerializeField] public string StageName { get; private set; }
    [field: SerializeField] public string StageDescription { get; private set; }
    [field: SerializeField] public Sprite BgSprite { get; private set; }

    private void OnValidate()
    {
        #if UNITY_EDITOR
        StageName = name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
