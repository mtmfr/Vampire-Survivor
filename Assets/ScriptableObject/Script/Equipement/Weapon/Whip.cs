using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon/Whip", menuName = "Scriptable Objects/Equipement/Weapon/Whip")]
public class Whip : Weapon
{
    [Header("Weapon")]

    [Tooltip("Lenght and height of the whip attack hitbox")]
    [SerializeField] private Vector2 attackHitbox;

    private WhipProjectile whipProjectile { get => weaponOriginal.GetComponent<WhipProjectile>(); }

    public override void CreateNewProjectile() {}

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

    public override IEnumerator AttackRoutine()
    {
        SpriteRenderer playerSprite = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();

        while (true)
        {
            for (int nbOfAttack = 0; nbOfAttack < projectileAmount; nbOfAttack++)
            {
                if (playerSprite.flipX == false)
                {
                    if (nbOfAttack % 2 != 0)
                        LeftAttack();
                    else RightAttack();
                }
                else
                {
                    if (nbOfAttack % 2 != 0)
                        RightAttack();
                    else LeftAttack();
                }

                
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(cooldown);
        }
    }
    #endregion

    #region left/right attack logic
    /// <summary>
    /// Do the attack on the left side of the player character
    /// </summary>
    private void LeftAttack()
    {
        //Set the transform of the whip
        Vector2 LeftWhipPos = Player.playerRb.position - Vector2.right * offsetFromPlayer;
        Vector3 scale = Vector3.one;

        if (ObjectPool.IsAnyObjectInactive(whipProjectile))
        {
            WhipProjectile whip = ObjectPool.GetInactiveObject(whipProjectile);

            DrawAttackHitbox(LeftWhipPos);

            whip.SetWhip(LeftWhipPos, scale, attackHitbox);
            whip.gameObject.SetActive(true);
            whip.Attack(attack);
        }
        else
        {
            GameObject whip = Instantiate(whipProjectile.gameObject, LeftWhipPos, Quaternion.identity);
            whip.transform.localScale = scale;
            whip.GetComponent<WhipProjectile>().Attack(attack);
        }
    }

    /// <summary>
    /// Do the attack on the right side of the player character
    /// </summary>
    private void RightAttack()
    {
        Vector2 RightWhipPos = Player.playerRb.position + Vector2.right * offsetFromPlayer;
        Vector3 scale = new Vector3(-1, -1, 1);

        if (ObjectPool.IsAnyObjectInactive(whipProjectile))
        {
            var whip = ObjectPool.GetInactiveObject(whipProjectile);
            
            DrawAttackHitbox(RightWhipPos);

            whip.SetWhip(RightWhipPos, scale, attackHitbox);
            whip.gameObject.SetActive(true);
            whip.Attack(attack);
        }
        else
        {
            GameObject whip = Instantiate(whipProjectile.gameObject, RightWhipPos, Quaternion.identity);
            whip.transform.localScale = scale;
        }
    }
    #endregion

    private void DrawAttackHitbox(Vector2 attPos)
    {
        float duration = 0.1f;
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
