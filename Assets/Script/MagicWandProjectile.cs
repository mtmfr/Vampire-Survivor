using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class MagicWandProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    private float speed;

    private int damage;

    private LayerMask enemyLayer = 1 << 6;

    Vector2 dir;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        col.includeLayers = enemyLayer;
    }

    private void FixedUpdate()
    {
        MoveTowardTarge();
    }

    private void MoveTowardTarge()
    {
        rb.linearVelocity = dir.normalized * speed;
    }

    public void UpdateProjectileInfo(GameObject target, float speed, int projectileDamage)
    {
        this.speed = speed;
        damage = projectileDamage;
        dir = target.transform.position - transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerEvent.AttackLand(collision.gameObject.GetInstanceID(), damage);
        gameObject.SetActive(false);
    }
}
