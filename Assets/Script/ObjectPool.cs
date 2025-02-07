using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectPool
{
    public static bool IsAnyObjectActive<T>(T objectToCheck) where T : MonoBehaviour
    {
        // Retrieve all objects of type T (including inactive objects)
        List<T> Tobjects = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

        // Get the type of the object to check in advance to avoid calling GetType() on each iteration
        Type targetType = objectToCheck.GetType();

        Debug.Log(targetType);

        foreach (T Tobject in Tobjects)
        {
            // If the types do not match, skip this object
            if (Tobject.GetType() != targetType)
                continue;

            // If the object is active in the hierarchy, skip it
            if (Tobject.gameObject.activeInHierarchy)
                continue;

            // If the object is inactive, return true
            return true;
        }

        // If no matching inactive object is found, return false
        return false;
    }

    public static T GetInactiveObject<T>(T objectToGet) where T : MonoBehaviour
    {
        // Retrieve all objects of type T, including inactive ones
        List<T> values = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(element =>
        {
            bool type = element.GetType() == objectToGet.GetType();

            return type;
        }).ToList();

        // If no objects found, return null
        if (values.Count == 0)
            return null;

        // Find the first inactive object (inactive objects won't be active in the hierarchy)
        T inactiveObject = values.FirstOrDefault(chose => !chose.gameObject.activeInHierarchy);

        // Return the first inactive object found, or null if none found
        return inactiveObject;
    }
}
