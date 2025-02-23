using System.Collections;
using UnityEngine;

public class Whip : Weapon
{
    [SerializeField] private Vector2 attackHitbox;

    private WhipProjectile whipProjectile { get => weaponObject.GetComponent<WhipProjectile>(); }

    protected override void Start()
    {
        base.Start();
    }

    public override void LevelUp()
    {
        if (currentLevel > maxLevel)
            return;

        currentLevel++;

        switch (currentLevel)
        {
            case 2:
                numberOfProjectile += 1;
                break;
            case 3:
                attackPower += 5;
                break;
            case 4:
                attackHitbox = attackHitbox * 1.1f;
                attackPower += 5;
                break;
            case 5:
                attackPower += 5;
                break;
            case 6:
                attackHitbox = attackHitbox * 1.1f;
                break;
            case 7:
                attackPower += 5;
                break;
            default:
                attackPower += 5;
                break;
        }
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            for (int nbOfAttack = 0; nbOfAttack < numberOfProjectile; nbOfAttack++)
            {
                bool playerIsFacingLeft = player.GetComponent<SpriteRenderer>().flipX;
                if (playerIsFacingLeft)
                {
                    if (nbOfAttack % 2 == 0)
                        LeftAttack();
                    else RightAttack();
                }
                else
                {
                    if (nbOfAttack % 2 == 0)
                        RightAttack();
                    else LeftAttack();
                }
                
                yield return new WaitForSeconds(projectileDelay);
            }
            yield return new WaitForSeconds(weaponCooldown);
        }
    }

    private void RightAttack()
    {
        Vector2 RightWhipPos = player.transform.position + Vector3.right * offsetFromPlayer;
        Vector3 scale = new Vector3(-1, -1, 1);

        if (ObjectPool.IsAnyObjectInactive(whipProjectile))
        {
            var whip = ObjectPool.GetInactiveObject<WhipProjectile>();

            //DrawAttackHitbox(RightWhipPos);

            whip.SetWhip(RightWhipPos, scale, attackHitbox);
            whip.gameObject.SetActive(true);
            whip.Attack(attackPower);
        }
        else
        {
            GameObject whip = Instantiate(whipProjectile.gameObject, RightWhipPos, Quaternion.identity);
            whip.transform.localScale = scale;
        }
    }

    private void LeftAttack()
    {
        //Set the transform of the whip
        Vector2 LeftWhipPos = player.transform.position - Vector3.right * offsetFromPlayer;
        Vector3 scale = Vector3.one;

        if (ObjectPool.IsAnyObjectInactive(whipProjectile))
        {
            WhipProjectile whip = ObjectPool.GetInactiveObject<WhipProjectile>();

            //DrawAttackHitbox(LeftWhipPos);

            whip.SetWhip(LeftWhipPos, scale, attackHitbox);
            whip.gameObject.SetActive(true);
            whip.Attack(attackPower);
        }
        else
        {
            GameObject whip = Instantiate(whipProjectile.gameObject, LeftWhipPos, Quaternion.identity);
            whip.transform.localScale = scale;
            whip.GetComponent<WhipProjectile>().Attack(attackPower);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(player.transform.position + Vector3.right * offsetFromPlayer, attackHitbox);
        Gizmos.DrawWireCube(player.transform.position - Vector3.right * offsetFromPlayer, attackHitbox);
    }
}
