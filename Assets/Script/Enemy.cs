using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private Transform playerTransform;

    [SerializeField] private GameObject Xp;

    [SerializeField] private EnemyStats enemyStats;

    private int health;
    private int attack;
    private float speed;
    private float attackSpeed;
    [SerializeField] private float pushbackForce;

    [SerializeField] private bool canMove;

    IEnumerator attackRoutine;

    private Color baseColor = Color.white;
    private Color DamagedColor = Color.red;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = baseColor;
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

        health = enemyStats.Health;
        attack = enemyStats.Attack;
        speed = enemyStats.Speed;
        attackSpeed = enemyStats.AttackSpeed;

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
            GameState.InGame => true,
            _ => false
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
        if (attackRoutine != null)
            return;

        attackRoutine = AttackRoutine();
        StartCoroutine(attackRoutine);
    }
    private void StopAttacking()
    {
        if (attackRoutine == null)
            return;

        StopCoroutine(attackRoutine);
        attackRoutine = null;
    }

    private IEnumerator AttackRoutine()
    {
        EnemyEvent.PlayerReached(attack);
        yield return new WaitForSeconds(1 / attackSpeed);
        attackRoutine = null;
        StartAttacking();
    }
    #endregion

    private void TakeDamage(int Id, int DamageTaken)
    {
        if (Id != gameObject.GetInstanceID())
            return;

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

        rb.AddForce(-pushbackDir.normalized * pushbackForce);
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
        Instantiate(Xp, transform.position, Quaternion.identity);
        SpawnerEvent.BecomeInactive(gameObject);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartAttacking();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        StopAttacking();
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
