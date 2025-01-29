using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon/Whip", menuName = "Scriptable Objects/Equipement/Weapon/Whip")]
public class Whip : Weapon
{
    [Header("Weapon")]
    [Tooltip("The visual effect of the whip")]
    private ParticleSystem whipFx;

    [Tooltip("Lenght and height of the whip attack hitbox")]
    [SerializeField] private Vector2 attackHitbox;

    private bool isWeaponSpawned = false;

    public override void CreateNewProjectile()
    {
        if (isWeaponSpawned == true)
            return;

        GameObject whip = Instantiate(weaponOriginal, Player.playerRb.transform);
        
        if (whip != null)
            whipFx = whip.GetComponent<ParticleSystem>();
        isWeaponSpawned = false;
    }

    /// <summary>
    /// Level up the weapon
    /// </summary>
    public override void LevelUp()
    {
        //check if the weapon is at level max
        if (level + 1 > maxLevel)
            return;

        //augment it's level
        level++;

        // do the logic of it's level up
        switch (level)
        {
            case 2:
                projectileAmount += 1;
                break;
            case 3:
                attack += 5;
                break;
            case 4:
                attackHitbox = attackHitbox * 1.1f;
                attack += 5;
                break;
            case 5:
                attack += 5;
                break;
            case 6:
                attackHitbox = attackHitbox * 1.1f;
                break;
            case 7:
                attack += 5;
                break;
            default:
                attack += 5;
                break;
        }
    }

    #region Attack controller
    public override void StartAttack(MonoBehaviour player)
    {
        CreateNewProjectile();
        player.StartCoroutine(AttackRoutine());
    }

    public override void StopAttack(MonoBehaviour player)
    {
        player.StopCoroutine(AttackRoutine());
    }

    protected override IEnumerator AttackRoutine()
    {
        while (true)
        {
            for (int nbOfAttack = 0; nbOfAttack < projectileAmount; nbOfAttack++)
            {
                if (IsFirstAttackLeft())
                {
                    if (nbOfAttack % 2 != 0)
                        RightAttack();
                    else LeftAttack();
                    yield return new WaitForSeconds(whipFx.main.duration);
                }
                else
                {
                    if (nbOfAttack % 2 != 0)
                        LeftAttack();
                    else RightAttack();
                    yield return new WaitForSeconds(whipFx.main.duration);
                }
            }
            yield return new WaitForSeconds(cooldown);
        }
    }

    private bool isLastAttackLeft = false;
    private bool IsFirstAttackLeft()
    {
        if (Player.playerRb.linearVelocityX < 0)
        {
            isLastAttackLeft = true;
            return isLastAttackLeft;
        }
        else if (Player.playerRb.linearVelocityX > 0)
        {
            isLastAttackLeft = false;
            return isLastAttackLeft;
        }

        return isLastAttackLeft;
    }
    #endregion

    #region left/right attack logic
    /// <summary>
    /// Do the attack on the left side of the player character
    /// </summary>
    private void LeftAttack()
    {
        //Create the collider list of hit object
        List<Collider2D> leftAttackCollision;

        //Set the transform of the whip
        Vector2 LeftWhipPos = Player.playerRb.position - Vector2.right * offsetFromPlayer;

        DrawAttackHitbox(LeftWhipPos);

        whipFx.transform.position = LeftWhipPos;
        whipFx.transform.rotation = Quaternion.Euler(Vector3.zero);
        whipFx.transform.localScale = Vector3.one;

        //Attack
        whipFx.Emit(1);
        leftAttackCollision = Physics2D.OverlapBoxAll(LeftWhipPos, attackHitbox, 0, attackLayer)
                              .ToList();

        //Get all the collider in the object in the collider 
        foreach (Collider2D enemy in leftAttackCollision)
        {
            if (enemy == null)
                continue;

            PlayerEvent.AttackLand(enemy.gameObject.GetInstanceID(), attack);
        }
    }

    /// <summary>
    /// Do the attack on the right side of the player character
    /// </summary>
    private void RightAttack()
    {
        //Create the collider list of hit object
        List<Collider2D> rightAttackCollision;

        //Set the transform of the whip
        Vector2 RightWhipPos = Player.playerRb.position + Vector2.right * offsetFromPlayer;

        DrawAttackHitbox(RightWhipPos);

        whipFx.transform.position = RightWhipPos;
        whipFx.transform.rotation = Quaternion.Euler(Vector3.zero);
        whipFx.transform.localScale = new Vector3(-1, -1, 1);

        //Attack
        whipFx.Emit(1);
        rightAttackCollision = Physics2D.OverlapBoxAll(RightWhipPos, attackHitbox, 0, attackLayer)
                               .ToList();

        //Get all the collider in the object in the collider 
        foreach (Collider2D enemy in rightAttackCollision)
        {
            if (enemy == null)
                continue;

            PlayerEvent.AttackLand(enemy.gameObject.GetInstanceID(), attack);
        }
    }
    #endregion

    private void DrawAttackHitbox(Vector2 attPos)
    {
        float duration = whipFx.main.duration;
        var UL = attPos + Vector2.up * (attackHitbox.y / 2) + Vector2.left * (attackHitbox.x / 2);
        var UR = attPos + Vector2.up * (attackHitbox.y / 2) + Vector2.right * (attackHitbox.x / 2);

        var DL = attPos + Vector2.down * (attackHitbox.y / 2) + Vector2.left * (attackHitbox.x / 2);
        var DR = attPos + Vector2.down * (attackHitbox.y / 2) + Vector2.right * (attackHitbox.x / 2);

        Debug.DrawLine(UL, UR, Color.red, duration);
        Debug.DrawLine(DL, DR, Color.red, duration);
        Debug.DrawLine(UL, DL, Color.red, duration);
        Debug.DrawLine(UR, DR, Color.red, duration);
    }
}
