using System;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SetInactive();
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

    private void SetInactive()
    {
        //Check if there are any Pickable object to be drop
        if (droppedItems.Count == 0)
            Debug.LogError("No item can be dropped from lightSource");

        //Create a new List that contains each droppable object once * it's weight
        List<PickableObject> rawDrop = new();
        foreach(PickableObject drop in droppedItems)
        {

            for(int objectToAdd = 0; objectToAdd < drop.Weight; objectToAdd++)
            {
                //Add each object for it's weight in rawDrop
                rawDrop.Add(drop);
            }
        }

        //Check if rawdrop is null
        if (rawDrop.Count == 0)
            Debug.LogError("No object can be dropped from light source");

        //Get a random id
        int objectToDropId = UnityEngine.Random.Range(0, rawDrop.Count - 1);

        //Get the pickable object at rawDrop placement
        PickableObject objectToDrop = rawDrop[objectToDropId];

        //check for active object
        bool truc = ObjectPool.IsAnyObjectActive(objectToDrop);

        if (truc)
        {
            GameObject objectToActivate = ObjectPool.GetInactiveObject(objectToDrop).gameObject;
            objectToActivate.transform.position = transform.position;
            objectToActivate.SetActive(true);
        }
        else
        {
            GameObject objectToActivate = objectToDrop.gameObject;
            Instantiate(objectToActivate, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false);
    }
}
