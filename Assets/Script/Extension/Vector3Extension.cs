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

    public static bool HasNaanValue(this Vector3 vector3)
    {
        if (float.IsNaN(vector3.x))
            return true;
        if (float.IsNaN(vector3.y)) 
            return true;
        if (float.IsNaN (vector3.z))
            return true;

        return false;
    }

    public static Vector3 Set(this Vector3 vector3, float newX, float newY)
    {
        vector3.x = newX;
        vector3.y = newY;
        return vector3;
    }
}
