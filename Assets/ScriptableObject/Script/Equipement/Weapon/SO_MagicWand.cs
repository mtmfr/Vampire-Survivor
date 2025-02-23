using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicWand", menuName = "Scriptable Objects/Equipement/Weapon/MagicWand")]
public class SO_MagicWand : SO_Weapon
{
    [SerializeField, Min(1)] private int nbPierceableEnemy;

    private MagicWandProjectile projectile { get => weaponOriginal.GetComponent<MagicWandProjectile>(); }

    public override void CreateNewProjectile() { }

    public override void LevelUp()
    {
        switch (level)
        {
            case 2:
                projectileAmount++;
                break;
            case 3:
                cooldown -= 0.2f;
                break;
            case 4:
                projectileAmount++;
                break;
            case 5:
                attack += 10;
                break;
            case 6:
                projectileAmount++;
                break;
            case 7:
                nbPierceableEnemy++;
                break;
            case 8:
                attack += 10;
                break;
        }
    }

    public override IEnumerator AttackRoutine()
    {
        while (true)
        {
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                for (int toShoot = 0; toShoot < projectileAmount; toShoot++)
                {
                    if (ObjectPool.IsAnyObjectInactive(projectile))
                    {
                        var usedProjectile = ObjectPool.GetInactiveObject(projectile);
                        usedProjectile.transform.position = Player.playerRb.position;
                        usedProjectile.gameObject.SetActive(true);
                        usedProjectile.UpdateProjectileInfo(closestEnemy, speed, attack, nbPierceableEnemy);
                    }
                    else
                    {
                        GameObject usedProjectile = Instantiate(projectile.gameObject, Player.playerRb.position, Quaternion.identity);
                        usedProjectile.GetComponent<MagicWandProjectile>().UpdateProjectileInfo(closestEnemy, speed, attack, nbPierceableEnemy);
                    }
                    yield return new WaitForSeconds(delay);
                }
            }

            yield return new WaitForSeconds(cooldown);
        }
    }

    private bool IsAnyProjectileActive()
    {
        foreach(GameObject spell in weaponAmmo)
        {
            if (spell.activeInHierarchy == true)
                return true;
        }
        return false;
    }

    private GameObject FindClosestEnemy()
    {
        List<Enemy> enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();

        if (enemies.Count == 0)
            return null;

        Enemy closestEnemy = enemies.OrderBy(order => Vector3.Distance(Player.playerRb.position, order.transform.position)).FirstOrDefault();

        return closestEnemy.gameObject;
    }
}
