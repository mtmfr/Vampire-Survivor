using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectPool
{
    public static bool IsAnyObjectInactive<T>(T objectToCheck) where T : MonoBehaviour
    {
        // Get the type of the object to check in advance to avoid calling GetType() on each iteration
        Type targetType = objectToCheck.GetType();

        // Retrieve all objects of type T (including inactive objects)
        List<T> Tobjects = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(T =>
        {
            //check the type of the current object
            bool hasRightType = T.GetType() == targetType;

            //check if the current check object is inactive
            bool isActive = T.gameObject.activeInHierarchy;

            return hasRightType && !isActive;
        }).ToList();

        return Tobjects.Count != 0;
    }

    public static T GetInactiveObject<T>(T objectToGet) where T : MonoBehaviour
    {
        // Retrieve all objects of type T, including inactive ones
        List<T> values = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(element =>
        {
            bool type = element.GetType() == objectToGet.GetType();

            return type;
        }).ToList();

        // Find the first inactive object (inactive objects won't be active in the hierarchy)
        T inactiveObject = values.FirstOrDefault(chose => !chose.gameObject.activeInHierarchy);

        // Return the first inactive object found, or null if none found
        return inactiveObject;
    }
}
