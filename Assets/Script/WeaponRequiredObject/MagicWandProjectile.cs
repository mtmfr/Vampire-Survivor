using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class MagicWandProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    private float speed;
    private int damage;
    private int pierceable;

    private GameObject lastEnemyHitten;

    private LayerMask enemyLayer = 1 << 6;

    private Vector2 dir;

    bool canStartDespawnTimer;
    /// <summary>
    /// Lifetime of the bullet out of the screen
    /// </summary>
    [Tooltip("Lifetime of the bullet out of the screen")]
    [SerializeField] private int OosLifetime;

    private float lifeTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        col.includeLayers = enemyLayer;
    }

    private void FixedUpdate()
    {
        MoveTowardTarget();

        if (canStartDespawnTimer)
            DespawnTimer();
    }

    private void OnEnable()
    {
        canStartDespawnTimer = false;
        lifeTime = OosLifetime;

        lastEnemyHitten = null;
    }

    private void MoveTowardTarget()
    {
        rb.linearVelocity = dir.normalized * speed;
    }

    public void UpdateProjectileInfo(GameObject target, float speed, int projectileDamage, int nbPierceableEnemy)
    {
        this.speed = speed;
        damage = projectileDamage;
        dir = target.transform.position - transform.position;
        pierceable = nbPierceableEnemy;
    }

    private void DespawnTimer()
    {
        lifeTime -= Time.fixedDeltaTime;

        if (lifeTime <= 0)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject != lastEnemyHitten)
        {
            PlayerEvent.AttackLand(collision.gameObject.GetInstanceID(), damage);
            lastEnemyHitten = collision.gameObject;
            pierceable--;
        }

        if (pierceable == 0)
            gameObject.SetActive(false);
    }

    private void OnBecameInvisible()
    {
        canStartDespawnTimer = true;
    }
}
