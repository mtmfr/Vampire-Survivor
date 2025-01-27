using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Weapon> Weapons { get; private set; } = new(6);

    private void OnEnable()
    {
        GameStateManager.OnGameStateChange += ClearWeaponList;
        WeaponEvents.OnGetNewWeapon += NewWeaponGot;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= ClearWeaponList;
        WeaponEvents.OnGetNewWeapon -= NewWeaponGot;
    }

    private void NewWeaponGot(Weapon newWeapon)
    {
        if (Weapons.Contains(newWeapon))
            return;

            Weapons.Add(newWeapon);
    }

    private void ClearWeaponList(GameState gameState)
    {
        if (gameState != GameState.GameOver)
            return;

        Weapons.Clear();
    }
}
