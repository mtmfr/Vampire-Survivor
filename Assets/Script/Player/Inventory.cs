using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

public static class Inventory
{
    public static int gold;

    private static int weaponsInInventory = 0;
    private static Weapon[] weapons = new Weapon[6];

    public static event Action<Weapon> OnWeaponObtained;
    public static void AddNewWeapon(Weapon weaponToAdd)
    {
        if (weaponToAdd == null)
            throw new NullReferenceException("Ther is no weapon to add");

        if (weapons.Contains(weaponToAdd))
        {
            weaponToAdd.LevelUp();
            return;
        }

        if (weaponsInInventory + 1 == weapons.Length)
            return;

        weapons[weaponsInInventory] = weaponToAdd;
        OnWeaponObtained?.Invoke(weaponToAdd);

        weaponsInInventory++;
    }

    public static void ClearWeaponList()
    {
        for (int Id = 0; Id < weapons.Length; Id++)
        {
            weapons[Id] = null;
        }
    }

    public static event Action<int> OnGoldValueChanged;
    public static void GoldValueChanged(int newGold)
    {
        OnGoldValueChanged?.Invoke(newGold);
    }
}