using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private AudioSource damageFx;

    private Transform playerTransform;

    private XpPoint xp;

    [SerializeField] protected SO_Enemy enemySO;

    private int health;
    private int attack;
    private float speed;
    private float attackSpeed;
    private float knockback;

    [SerializeField] protected bool canMove;

    private Color baseColor = Color.white;
    private Color DamagedColor = Color.red;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        damageFx = GetComponent<AudioSource>();

        sprite.sprite = enemySO.sprite;
        sprite.color = baseColor;

        attack = enemySO.Attack;
        speed = enemySO.Speed;
        attackSpeed = enemySO.AttackSpeed;
        knockback = enemySO.KnockBack;

        xp = enemySO.XpPoint;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canMove)
            Move();
    }

    private void OnEnable()
    {
        if (sprite != null)
            sprite.color = baseColor;

        health = enemySO.Health;

        FindPlayerTransform();

        SpawnerEvent.BecomeActive(gameObject);

        GameStateManager.OnGameStateChange += UpdateCanMove;

        PlayerEvent.OnAttackLand += TakeDamage;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= UpdateCanMove;

        PlayerEvent.OnAttackLand -= TakeDamage;
    }

    #region Movement
    private void FindPlayerTransform()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void UpdateCanMove(GameState gameState)
    {
        canMove = gameState switch
        {
            GameState.Pause => false,
            GameState.LevelUp => false,
            _ =>true
        };
    }

    private void Move()
    {
        Vector2 MovementDir = playerTransform.position - transform.position;

        rb.linearVelocity = MovementDir.normalized * speed;
    }
    #endregion

    #region Attack
    private void StartAttacking()
    {
        StartCoroutine(AttackRoutine());
    }
    private void StopAttacking()
    {
        StopCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            EnemyEvent.PlayerReached(attack);
            yield return new WaitForSeconds(1 / attackSpeed);
        }
    }
    #endregion

    private void TakeDamage(int Id, int DamageTaken)
    {
        if (Id != gameObject.GetInstanceID())
            return;

        damageFx.Play();

        StartCoroutine(HittenColorChange());

        if (health - DamageTaken > 0)
            health -= DamageTaken;
        else
        {
            health = 0;
            Death();
        }
    }

    private void PushBackOnHit()
    {
        Vector2 pushbackDir = playerTransform.position - transform.position;

        rb.AddForce(knockback * speed * -pushbackDir.normalized, ForceMode2D.Impulse);
    }

    private IEnumerator HittenColorChange()
    {
        sprite.color = DamagedColor;
        PushBackOnHit();
        yield return new WaitForSeconds(0.15f);
        sprite.color = baseColor;
    }

    private void Death()
    {
        if (ObjectPool.IsAnyObjectInactive(xp))
        {
            GameObject xpToSpawn = ObjectPool.GetInactiveObject<XpPoint>().gameObject;
            xpToSpawn.transform.position = transform.position;
            xpToSpawn.SetActive(true);
        }
        else Instantiate(xp.gameObject, transform.position, Quaternion.identity);

        SpawnerEvent.BecomeInactive(gameObject);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            StartCoroutine(AttackRoutine());
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            StopAllCoroutines();
    }
}

public static class EnemyEvent
{
    public static event Action<int> OnPlayerReached;
    public static void PlayerReached(int damageDone)
    {
        OnPlayerReached?.Invoke(damageDone);
    }
}
