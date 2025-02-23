using System;
using UnityEngine;

public static class WeaponEvents
{
    public static event Action<SO_Weapon> OnGetNewWeapon;
    public static void GetNewWeapon(SO_Weapon newWeapon)
    {
        OnGetNewWeapon?.Invoke(newWeapon);
    }

    public static event Action<SO_Weapon> OnUpgradeWeapon;
    public static void UpgradeWeapon(SO_Weapon weaponToUpgrade)
    {
        OnUpgradeWeapon?.Invoke(weaponToUpgrade);
    }
}
