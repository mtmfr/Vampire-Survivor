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
    private ParticleSystem WhipFx;

    [Tooltip("The position of the right attack of the whip")]
    private Vector2 RightWhipPos;

    [Tooltip("The position of the left attack of the whip")]
    private Vector2 LeftWhipPos;

    [Tooltip("Lenght and height of the whip attack hitbox")]
    [SerializeField] private Vector2 attackHitbox;

    private bool isWeaponSpawned = false;

    public override void CreateNewProjectile()
    {
        if (isWeaponSpawned == true)
            return;

        GameObject whip = Instantiate(WeaponOriginal, Player.playerTransform);
        
        if (whip != null)
            WhipFx = whip.GetComponent<ParticleSystem>();
        isWeaponSpawned = false;
    }

    public override void StartAttack(MonoBehaviour player)
    {
        player.StartCoroutine(AttackRoutine(player));
    }

    public override void StopAttack(MonoBehaviour player)
    {
        player.StopCoroutine(AttackRoutine(player));
    }

    protected override IEnumerator AttackRoutine(MonoBehaviour player)
    {
        CreateNewProjectile();

        var UL = LeftWhipPos + Vector2.up * (attackHitbox.y / 2) + Vector2.left * (attackHitbox.x / 2);
        var UR = LeftWhipPos + Vector2.up * (attackHitbox.y / 2) + Vector2.right * (attackHitbox.x / 2);

        var DL = LeftWhipPos + Vector2.down * (attackHitbox.y / 2) + Vector2.left * (attackHitbox.x / 2);
        var DR = LeftWhipPos + Vector2.down * (attackHitbox.y / 2) + Vector2.right * (attackHitbox.x / 2);

        Debug.DrawLine(LeftWhipPos, RightWhipPos, Color.red);
        Debug.DrawLine(DL, DR, Color.red);
        Debug.DrawLine(UL, DL, Color.red);
        Debug.DrawLine(UR, DR, Color.red);

        while (true)
        {
            for (int nbOfAttack = 0; nbOfAttack < projectileAmount; nbOfAttack++)
            {
                if (nbOfAttack % 2 != 0)
                    LeftAttack();
                else RightAttack();
                yield return new WaitForSeconds(WhipFx.main.duration);
            }
            yield return new WaitForSeconds(cooldown);
        }
    }

    /// <summary>
    /// Do the attack on the left side of the player character
    /// </summary>
    private void LeftAttack()
    {
        List<Collider2D> leftAttackCollision;

        LeftWhipPos = Player.playerTransform.position - Vector3.right * offsetFromPlayer;

        SetObjectTransform.SetTransform(WhipFx.transform, Player.playerTransform);
        WhipFx.transform.position = LeftWhipPos;
        WhipFx.Emit(1);
        leftAttackCollision = Physics2D.OverlapBoxAll(LeftWhipPos, attackHitbox, 0, attackLayer)
                              .ToList();

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
        List<Collider2D> rightAttackCollision;

        RightWhipPos = Player.playerTransform.position + Vector3.right * offsetFromPlayer;

        SetObjectTransform.SetTransform(WhipFx.transform, Player.playerTransform);
        WhipFx.transform.position = RightWhipPos;
        WhipFx.Emit(1);
        rightAttackCollision = Physics2D.OverlapBoxAll(RightWhipPos, attackHitbox, 0, attackLayer)
                               .ToList();

        foreach (Collider2D enemy in rightAttackCollision)
        {
            if (enemy == null)
                continue;

            PlayerEvent.AttackLand(enemy.gameObject.GetInstanceID(), attack);
        }
    }
}
