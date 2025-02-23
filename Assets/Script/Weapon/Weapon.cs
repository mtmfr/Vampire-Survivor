using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected int currentLevel = 1;
    protected int maxLevel = 8;

    [SerializeField] SO_Weapon weaponInfo;

    protected GameObject weaponObject;

    protected int attackPower;
    protected int numberOfProjectile;
    protected int numberOfPierce;

    protected float weaponSpeed;

    protected float projectileDelay;
    protected float weaponCooldown;

    protected float offsetFromPlayer;

    private IEnumerator attackRoutine;

    protected GameObject player { get => GameObject.FindWithTag("Player"); }

    protected virtual void Start()
    {
        UpdateWeaponStat();
    }

    private void OnEnable()
    {
        GameStateManager.OnGameStateChange += StartAttacking;
        GameStateManager.OnGameStateChange += StopAttacking;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= StartAttacking;
        GameStateManager.OnGameStateChange -= StopAttacking;
    }

    private void UpdateWeaponStat()
    {
        weaponObject = weaponInfo.weaponOriginal;

        attackPower = weaponInfo.attack;
        numberOfProjectile = weaponInfo.projectileAmount;
        numberOfPierce = weaponInfo.pierce;

        weaponSpeed = weaponInfo.speed;

        projectileDelay = weaponInfo.delay;
        weaponCooldown = weaponInfo.cooldown;

        offsetFromPlayer = weaponInfo.offset;
    }

    public abstract void LevelUp();

    private void StartAttacking(GameState gameState)
    {
        if (gameState != GameState.InGame)
            return;

        if (attackRoutine == null)
            attackRoutine = Attack();

        StartCoroutine(attackRoutine);
    }

    private void StopAttacking(GameState gameState)
    {
        if (gameState == GameState.LevelUp)
        {
            StopCoroutine(attackRoutine);
        }

        else if (gameState == GameState.GameOver)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    protected abstract IEnumerator Attack();
}
