using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    #region object components
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    #endregion

    #region Stats
    [Header("ScriptableObject")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Player Stats")]
    private int hp;
    private float speed;
    private int attack;
    private float attackSpeed;
    #endregion

    #region whip
    [Header("Whip")]
    [Tooltip("The visual effect of the whip")]
    [SerializeField] private ParticleSystem WhipFx;

    [Tooltip("The position of the right attack of the whip")]
    [SerializeField] private Transform RightWhipPos;

    [Tooltip("The position of the left attack of the whip")]
    [SerializeField] private Transform LeftWhipPos;

    [Tooltip("Lenght and height of the whip attack hitbox")]
    [SerializeField] private Vector2 attackHitbox;
    private LayerMask attackLayer = 1 << 6;
    #endregion

    #region Xp
    [Header("Level")]
    private int currentLevel;
    [SerializeField] private int maxLevel;
    private int xpToLevelUp;
    private int currentXp;
    #endregion

    private Vector2 startingPos = Vector2.zero;

    [SerializeField] private bool canMove;

    #region unity functions
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        sprite.color = new Color32(67, 120, 229, 255);

        xpToLevelUp = 15;
        currentXp = 0;

        PlayerEvent.SetHealth(hp);
        PlayerEvent.SetXpToLevelUp(xpToLevelUp);
        PlayerEvent.XpGain(currentXp);
        PlayerEvent.LevelUp(currentLevel);

        StopAllCoroutines();
    }

    private void OnEnable()
    {
        transform.position = startingPos;

        hp = playerStats.Health;
        speed = playerStats.Speed;
        attack = playerStats.Attack;
        attackSpeed = playerStats.AttackSpeed;

        currentLevel = 1;


        GameStateManager.OnGameStateChange += UpdateCanMove;
        GameStateManager.OnGameStateChange += SetVisibility;
        GameStateManager.OnGameStateChange += Attack;

        EnemyEvent.OnPlayerReached += TakeDamage;

        XpEvent.OnXpGain += GainXp;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= UpdateCanMove;
        GameStateManager.OnGameStateChange -= SetVisibility;
        GameStateManager.OnGameStateChange -= Attack;

        EnemyEvent.OnPlayerReached -= TakeDamage;

        XpEvent.OnXpGain -= GainXp;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canMove)
            Move(InputManager.Instance.MovementDir);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(LeftWhipPos.position, attackHitbox);
        Gizmos.DrawWireCube(RightWhipPos.position, attackHitbox);
    }
    #endregion

    #region movement
    /// <summary>
    /// Make the player move on the Y-Axis at constant speed and make go toward the cursor position
    /// </summary>
    /// <param name="mousePos">Position of the mouse on the screen</param>
    private void Move(Vector2 mousePos)
    {
        rb.linearVelocity = mousePos.normalized * speed;
    }

    /// <summary>
    /// update the value of canMove
    /// </summary>
    /// <param name="canMove">wather canMove is true or false</param>
    private void UpdateCanMove(GameState gameState)
    {
        canMove = gameState switch
        {
            GameState.InGame => true,
            _ => false
        };
    }
    #endregion

    #region Attack
    private void Attack(GameState state)
    {
        if (state != GameState.InGame)
            return;

        StartCoroutine(AttackRoutine());
    }
    private IEnumerator AttackRoutine()
    {
        LeftAttack();
        yield return new WaitForSeconds(WhipFx.main.duration);

        RightAttack();
        yield return new WaitForSeconds(1 / attackSpeed);

        StartCoroutine(AttackRoutine());
    }

    /// <summary>
    /// Do the attack on the left side of the player character
    /// </summary>
    private void LeftAttack()
    {
        List<Collider2D> leftAttackCollision;

        SetObjectTransform.SetTransform(WhipFx.transform, LeftWhipPos);
        WhipFx.Emit(1);
        leftAttackCollision = Physics2D.OverlapBoxAll(LeftWhipPos.position, attackHitbox, 0, attackLayer)
                              .ToList();

        foreach (Collider2D enemy in leftAttackCollision)
        {
            if (enemy == null)
                continue;

            PlayerEvent.AttackLand(enemy.gameObject.GetInstanceID(), attack);
        }
    }

    /// <summary>
    /// Do the attack on the right side of the player character
    /// </summary>
    private void RightAttack()
    {
        List<Collider2D> rightAttackCollision;

        SetObjectTransform.SetTransform(WhipFx.transform, RightWhipPos);
        WhipFx.Emit(1);
        rightAttackCollision = Physics2D.OverlapBoxAll(RightWhipPos.transform.position, attackHitbox, 0, attackLayer)
                               .ToList();

        foreach (Collider2D enemy in rightAttackCollision)
        {
            if (enemy == null)
                continue;

            PlayerEvent.AttackLand(enemy.gameObject.GetInstanceID(), attack);
        }
    }

    #endregion

    #region health related function
    /// <summary>
    /// Make the player lose health
    /// </summary>
    /// <param name="damage">The amount of health removed</param>
    private void TakeDamage(int damage)
    {
        if (hp - damage > 0)
            hp -= damage;
        else
        {
            hp = 0;
            Death();
        }
        PlayerEvent.UpdateHealth(hp);
    }

    /// <summary>
    /// make the player disappear
    /// </summary>
    private void Death()
    {
        StartCoroutine(OnDeath());
    }

    /// <summary>
    /// Coroutine that control the death of the player
    /// </summary>
    private IEnumerator OnDeath()
    {
        StopCoroutine(AttackRoutine());
        sprite.enabled = false;
        yield return new WaitForSeconds(1);
        GameStateManager.UpdateGameState(GameState.GameOver);
        gameObject.SetActive(false);
    }
    #endregion

    private void GainXp(int xpGained)
    {
        if (xpGained < 0)
            return;

        if (xpToLevelUp < 0)
            return;

        if (xpGained + currentXp < xpToLevelUp)
        {
            currentXp += xpGained;
            PlayerEvent.XpGain(currentXp);
        }
        else
        {
            currentXp = currentXp + xpGained - xpToLevelUp;
            PlayerEvent.XpGain(currentXp);
            currentLevel++;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        if (currentLevel + 1 < maxLevel)
        {
            currentLevel++;
            PlayerEvent.LevelUp(currentLevel);
            xpToLevelUp *= 2;
            PlayerEvent.SetXpToLevelUp(xpToLevelUp);
        }
        else if(currentLevel + 1 == maxLevel)
        {
            PlayerEvent.LevelUp(currentLevel);
            xpToLevelUp = -1;
            PlayerEvent.MaxlevelAttained();
        }
    }

    private void SetVisibility(GameState state)
    {
        sprite.enabled = state switch
        {
            GameState.InGame => true,
            GameState.GameOver => false,
            _ => true,
        };
    }
}

public static class PlayerEvent
{
    #region Health event
    public static event Action<int> OnSetHealth;
    public static void SetHealth(int health)
    {
        OnSetHealth?.Invoke(health);
    }

    public static event Action<int> OnUpdateHealth;
    public static void UpdateHealth(int health)
    {
        OnUpdateHealth?.Invoke(health);
    }
    #endregion

    #region Xp event
    public static event Action<int> OnSetXpToLevelUp;
    public static void SetXpToLevelUp(int xpAmount)
    {
        OnSetXpToLevelUp?.Invoke(xpAmount);
    }

    public static event Action<int> OnXpGain;
    public static void XpGain (int currentXpPoint)
    {
        OnXpGain?.Invoke(currentXpPoint);
    }

    public static event Action<int> OnLevelUp;
    public static void LevelUp(int currentLevel)
    {
        OnLevelUp?.Invoke(currentLevel);
    }

    public static event Action OnMaxlevelAttained;
    public static void MaxlevelAttained()
    {
        OnMaxlevelAttained?.Invoke();
    }
    #endregion

    public static event Action<int, int> OnAttackLand;
    public static void AttackLand(int objectId, int damage)
    {
        OnAttackLand?.Invoke(objectId, damage);
    }
}
