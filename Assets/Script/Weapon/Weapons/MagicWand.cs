using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MagicWand : Weapon
{
    private MagicWandProjectile projectile { get => weaponObject.GetComponent<MagicWandProjectile>(); }

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
                numberOfProjectile++;
                break;
            case 3:
                weaponCooldown -= 0.2f;
                break;
            case 4:
                numberOfProjectile++;
                break;
            case 5:
                attackPower += 10;
                break;
            case 6:
                numberOfProjectile++;
                break;
            case 7:
                numberOfPierce++;
                break;
            case 8:
                attackPower += 10;
                break;
        }
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                for (int toShoot = 0; toShoot < numberOfProjectile; toShoot++)
                {
                    if (ObjectPool.IsAnyObjectInactive(projectile))
                    {
                        var usedProjectile = ObjectPool.GetInactiveObject<MagicWandProjectile>();
                        usedProjectile.transform.position = player.transform.position;
                        usedProjectile.gameObject.SetActive(true);
                        usedProjectile.UpdateProjectileInfo(closestEnemy, weaponSpeed, attackPower, numberOfPierce);
                    }
                    else
                    {
                        GameObject usedProjectile = Instantiate(projectile.gameObject, player.transform.position, Quaternion.identity);
                        usedProjectile.GetComponent<MagicWandProjectile>().UpdateProjectileInfo(closestEnemy, weaponSpeed, attackPower, numberOfPierce);
                    }
                    yield return new WaitForSeconds(projectileDelay);
                }
            }

            yield return new WaitForSeconds(weaponCooldown);
        }
    }

    private GameObject FindClosestEnemy()
    {
        List<Enemy> enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();

        if (enemies.Count == 0)
            return null;

        Enemy closestEnemy = enemies.OrderBy(order => Vector3.Distance(player.transform.position, order.transform.position)).FirstOrDefault();

        return closestEnemy.gameObject;
    }
}
