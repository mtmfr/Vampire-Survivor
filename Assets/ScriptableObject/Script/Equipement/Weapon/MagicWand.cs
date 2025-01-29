using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicWand", menuName = "Scriptable Objects/Equipement/Weapon/MagicWand")]
public class MagicWand : Weapon
{
    public override void CreateNewProjectile()
    {
        int projectileToCreate = projectileAmount - currentProjectileAmount;

        if (projectileToCreate == 0)
            return;

        for (int projo = 0; projo < projectileToCreate; projo++)
        {
            GameObject createdProjo = Instantiate(weaponOriginal, Player.playerRb.transform);
            createdProjo.SetActive(false);
            weaponAmmo.Add(createdProjo);
            currentProjectileAmount++;
        }
    }

    public override void LevelUp()
    {
        throw new System.NotImplementedException();
    }

    public override void StartAttack(MonoBehaviour player)
    {
        CreateNewProjectile();
        player.StartCoroutine(AttackRoutine());
    }

    public override void StopAttack(MonoBehaviour player)
    {
        player.StartCoroutine(AttackRoutine());
    }

    protected override IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (IsAnyProjectileActive() != true)
            {
                foreach (GameObject spell in weaponAmmo)
                {
                    spell.transform.position = Player.playerRb.position;
                    spell.SetActive(true);
                    spell.GetComponent<MagicWandProjectile>().UpdateProjectileInfo(FindClosestEnemy(), speed, attack);
                    yield return new WaitForSeconds(delay);
                }
                yield return new WaitForSeconds(cooldown);
            }
            else yield return new WaitForEndOfFrame();
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
        List<Enemy> enemy = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();

        if (enemy == null)
            return null;

        Enemy closestEnemy = enemy.OrderBy(order => Vector3.Distance(Player.playerRb.position, order.transform.position)).FirstOrDefault();
        return closestEnemy.gameObject;
    }
}
