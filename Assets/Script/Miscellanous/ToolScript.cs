using UnityEngine;

public static class SetObjectTransform
{
    public static void SetTransform(Transform transformToSet, Transform transformToCopy)
    {
        transformToSet.SetPositionAndRotation(transformToCopy.position, transformToCopy.rotation);
        transformToSet.localScale = transformToCopy.localScale;
    }
}
