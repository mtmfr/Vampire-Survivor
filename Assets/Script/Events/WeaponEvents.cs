using System;
using UnityEngine;

public static class WeaponEvents
{
    public static event Action<Weapon> OnGetNewWeapon;
    public static void GetNewWeapon(Weapon newWeapon)
    {
        OnGetNewWeapon?.Invoke(newWeapon);
    }

    public static event Action<Weapon> OnUpgradeWeapon;
    public static void UpgradeWeapon(Weapon weaponToUpgrade)
    {
        OnUpgradeWeapon?.Invoke(weaponToUpgrade);
    }
}
