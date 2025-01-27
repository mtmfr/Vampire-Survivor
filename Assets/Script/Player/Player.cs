using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    #region object components
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private Inventory inventory;
    #endregion

    #region Stats
    [Header("Player Stats")]
    [SerializeField] private PlayerStats playerStats;

    private int hp;
    private float speed;
    #endregion

    [SerializeField] private SO_Character currentCharacter;

    private Vector2 startingPos = Vector2.zero;

    [SerializeField] private bool canMove;

    public static Rigidbody2D playerRb;

    #region unity functions
    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        inventory = GetComponentInChildren<Inventory>();

        sprite.sprite = currentCharacter.CharacterSprite;
        anim.runtimeAnimatorController = currentCharacter.CharacterAnim;
        inventory.Weapons.Add(currentCharacter.StartingWeapon);

        PlayerEvent.SetHealth(hp);
    }

    private void OnEnable()
    {
        transform.position = startingPos;

        hp = playerStats.Health;
        speed = playerStats.Speed;


        GameStateManager.OnGameStateChange += UpdateCanMove;
        GameStateManager.OnGameStateChange += Attack;

        EnemyEvent.OnPlayerReached += TakeDamage;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= UpdateCanMove;
        GameStateManager.OnGameStateChange -= Attack;

        EnemyEvent.OnPlayerReached -= TakeDamage;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canMove)
            Move(InputManager.Instance.MovementDir);
    }
    #endregion

    #region movement
    /// <summary>
    /// Make the player move on the Y-Axis at constant speed and make go toward the cursor position
    /// </summary>
    /// <param name="mousePos">Position of the mouse on the screen</param>
    private void Move(Vector2 mousePos)
    {
        if (mousePos != Vector2.zero)
        {
            rb.linearVelocity = mousePos.normalized * speed;
            anim.SetBool("IsMoving", true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("IsMoving", false);
        }

        if (rb.linearVelocityX > 0)
            sprite.flipX = false;
        else if (rb.linearVelocityX < 0)
            sprite.flipX = true;
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
        switch (state)
        {
            case GameState.InGame:
                foreach (Weapon weapon in inventory.Weapons)
                {
                    weapon.StartAttack(this);
                }
                break;
            case GameState.Pause:
                foreach (Weapon weapon in inventory.Weapons)
                {
                    weapon.StopAttack(this);
                }
                break;
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
        sprite.enabled = false;
        yield return new WaitForSeconds(1);
        GameStateManager.UpdateGameState(GameState.GameOver);
        gameObject.SetActive(false);
    }
    #endregion
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
