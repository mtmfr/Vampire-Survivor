using UnityEngine;

public class RoastChicken : PickableObject
{
    [SerializeField] private int healthRegained;

    protected override void Pickup()
    {
        PlayerEvent.RegainHealth(healthRegained);
    }
}
