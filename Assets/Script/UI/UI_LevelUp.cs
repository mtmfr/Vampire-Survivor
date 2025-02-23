using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LevelUp : MonoBehaviour
{
    [SerializeField] private List<Weapon> weaponSelection;
    private List<Weapon> weaponsInInventory = new();

    [SerializeField] private Button firstButton;
    [SerializeField] private Button secondButton;
    [SerializeField] private Button thirdButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        GameStateManager.OnGameStateChange += ClearWeaponsInInventory;
        Inventory.OnWeaponObtained += UpdateWeaponInInventory;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= ClearWeaponsInInventory;
        Inventory.OnWeaponObtained -= UpdateWeaponInInventory;
    }

    private void UpdateWeaponInInventory(Weapon weapon)
    {
        weaponsInInventory.Add(weapon);
    }

    private void ClearWeaponsInInventory(GameState gameState)
    {
        if (gameState != GameState.GameOver)
            return;

        weaponsInInventory.Clear();
    }
}
