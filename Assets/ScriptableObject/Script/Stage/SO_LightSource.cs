using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_LightSource", menuName = "Scriptable Objects/SO_LightSource")]
public class SO_LightSource : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public AnimatorOverrideController Animator { get; private set; }
}
