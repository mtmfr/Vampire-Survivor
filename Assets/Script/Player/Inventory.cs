using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

public static class Inventory
{
    public static int gold;
    public static List<Weapon> Weapons { get; private set; } = new(6);

    public static void NewWeaponGot(Weapon newWeapon)
    {
        Weapons.Add(newWeapon);
    }

    public static void ClearWeaponList()
    {
        Weapons.Clear();
    }

    public static event Action<int> OnGoldValueChanged;
    public static void GoldValueChanged(int newGold)
    {
        OnGoldValueChanged?.Invoke(newGold);
    }
}
