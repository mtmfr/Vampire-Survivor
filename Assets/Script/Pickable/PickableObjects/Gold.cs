using UnityEngine;

public class Gold : PickableObject
{
    protected override void Pickup()
    {
        Inventory.gold++;
        Inventory.GoldValueChanged(Inventory.gold);

        gameObject.SetActive(false);
    }
}
