using UnityEngine;

public static class Vector3Extension
{
    /// <summary>
    /// Divide a by b
    /// </summary>
    public static Vector3 Divide(this Vector3 vector3, Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }
}
