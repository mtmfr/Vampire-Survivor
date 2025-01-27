using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinValue))]
public class MinRangeAttribue : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);

    }
}
