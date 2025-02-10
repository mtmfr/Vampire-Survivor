using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LightSource : MonoBehaviour
{
    private SpriteRenderer lightSourceSprite;

    [SerializeField] private int health;

    [SerializeField] private List<PickableObject> droppedItems;

    private void Start()
    {
        lightSourceSprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        PlayerEvent.OnAttackLand += DamageTaken;
    }

    private void OnDisable()
    {
        PlayerEvent.OnAttackLand -= DamageTaken;
    }

    public void SetLightSourceSprite(Sprite spriteToSet)
    {
        if (spriteToSet == null)
            throw new System.ArgumentNullException("Sprite sprite", "No sprite to set");

        lightSourceSprite.sprite = spriteToSet;
    }

    private void DamageTaken(int id, int damage)
    {
        if (id != gameObject.GetInstanceID())
            return;

        if (health - damage <= 0)
            SetInactive();
    }

    /// <summary>
    /// This function handles setting the light source object to inactive and dropping a pickable item from it.
    /// </summary>
    private void SetInactive()
    {
        // Check if there are any pickable objects to drop from the light source.
        if (droppedItems.Count == 0)
            Debug.LogError("No item can be dropped from lightSource");

        // Create a new list that contains each droppable object repeated by its weight.
        List<PickableObject> rawDrop = new();
        foreach (PickableObject drop in droppedItems)
        {
            // For each droppable object, repeat it based on its weight.
            for (int objectToAdd = 0; objectToAdd < drop.Weight; objectToAdd++)
            {
                // Add the object to the rawDrop list based on its weight.
                rawDrop.Add(drop);
            }
        }

        // Check if no objects were added to rawDrop.
        if (rawDrop.Count == 0)
            Debug.LogError("No object can be dropped from light source");

        // Get a random index from the rawDrop list to determine which object to drop.
        int objectToDropId = UnityEngine.Random.Range(0, rawDrop.Count - 1);

        // Get the pickable object to drop based on the randomly chosen index.
        PickableObject objectToDrop = rawDrop[objectToDropId];

        // Check if there is an inactive object available in the object pool.
        if (ObjectPool.IsAnyObjectInactive(objectToDrop))
        {
            // If an inactive object is available, get it from the object pool and activate it at the current position.
            GameObject objectToActivate = ObjectPool.GetInactiveObject(objectToDrop).gameObject;
            objectToActivate.transform.position = transform.position;
            objectToActivate.SetActive(true);
        }
        else
        {
            // If no inactive object is available, instantiate a new object at the current position.
            GameObject objectToActivate = objectToDrop.gameObject;
            Instantiate(objectToActivate, transform.position, Quaternion.identity);
        }

        // Set the current game object (light source) to inactive after the object drop.
        gameObject.SetActive(false);
    }
}
