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

    public static bool HasNaanValue(this Vector3 vector3, Vector3 toCheck)
    {
        if (float.IsNaN(toCheck.x))
            return true;
        if (float.IsNaN(toCheck.y)) 
            return true;
        if (float.IsNaN (toCheck.z))
            return true;

        return false;
    }

    public static Vector3 Set(this Vector3 vector3, float newX, float newY)
    {
        Vector3 newVector3 = new Vector3(newX, newY);
        return newVector3;
    }
}
