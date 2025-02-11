using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WhipProjectile : MonoBehaviour
{

    [Tooltip("The visual effect of the whip")]
    private ParticleSystem whipFx;

    [Tooltip("Lenght and height of the whip attack hitbox")]
    [SerializeField] private Vector2 attackHitbox;

    private LayerMask attackLayer = 1 << 6;

    public void SetWhip(Vector2 position, Vector3 scale, Vector2 attackHitbox)
    {
        //set the transform position and scale
        transform.position = position;
        transform.localScale = scale;

        this.attackHitbox = attackHitbox;
    }

    public void Attack(int damageToDeal)
    {
        //Create the collider list of hit object
        List<Collider2D> attackCollision;

        if (whipFx == null)
            whipFx = GetComponent<ParticleSystem>();

        //Attack
        attackCollision = Physics2D.OverlapBoxAll(transform.position, attackHitbox, 0, attackLayer)
                               .ToList();

        //Get all the collider in the object in the collider 
        foreach (Collider2D enemy in attackCollision)
        {
            if (enemy == null)
                continue;

            PlayerEvent.AttackLand(enemy.gameObject.GetInstanceID(), damageToDeal);
        }

        StartCoroutine(AttackFx());
    }

    IEnumerator AttackFx()
    {
        whipFx.Emit(1);
        yield return new WaitForSeconds(whipFx.main.duration);
        gameObject.SetActive(false);
    }
}
