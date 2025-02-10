using UnityEngine;

[CreateAssetMenu(fileName = "CurrentLevel", menuName = "Scriptable Objects/CurrentLevel")]
public class SO_Stage : ScriptableObject
{
    [Header("Stage")]
    [field: SerializeField] public string StageName { get; private set; }
    [field: SerializeField] public string StageDescription { get; private set; }
    [field: SerializeField] public Sprite BgSprite { get; private set; }

    [field: SerializeField] public int Duration { get; private set; }

    [Header("LightSource")]
    [field: SerializeField] public Sprite LightSourceSprite { get; private set; }

    private void OnValidate()
    {
        #if UNITY_EDITOR
        StageName = name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
