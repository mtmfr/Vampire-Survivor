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
    #endregion

    #region ChildrenComponent
    [SerializeField] private SpriteRenderer healthBar;
    #endregion

    #region Stats
    [Header("Player Stats")]
    [SerializeField] private SO_PlayerStats playerStats;

    private int maxHp;
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

        maxHp = playerStats.Health;
        speed = playerStats.Speed;
    }

    private void OnEnable()
    {
        transform.position = startingPos;

        PlayerEvent.OnCharacterChosen += UpdateCurrentCharacter;
        PlayerEvent.OnRegainHealth += RegainHealth;

        GameStateManager.OnGameStateChange += UpdateCanMove;
        GameStateManager.OnGameStateChange += Attack;

        EnemyEvent.OnPlayerReached += TakeDamage;
    }

    private void OnDisable()
    {
        PlayerEvent.OnCharacterChosen -= UpdateCurrentCharacter;
        PlayerEvent.OnRegainHealth -= RegainHealth;

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

    /// <summary>
    /// Set the character used by the player
    /// </summary>
    /// <param name="character">the used character</param>
    private void UpdateCurrentCharacter(SO_Character character)
    {
        currentCharacter = character;
        sprite.sprite = currentCharacter.CharacterSprite;
        anim.runtimeAnimatorController = currentCharacter.CharacterAnim;

        Weapon startWeapon = Instantiate(currentCharacter.StartingWeapon);
        Inventory.NewWeaponGot(startWeapon);

        hp = maxHp;
        PlayerEvent.UpdateHealth((float)hp / maxHp);
    }


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
        if (state != GameState.InGame)
        {
            foreach (Weapon weapon in Inventory.Weapons)
            {
                weapon.StopAttack(this);
            }
        }
        else
        {
            foreach (Weapon weapon in Inventory.Weapons)
            {
                weapon.StartAttack(this);
            }
        }
    }
    #endregion

    #region health related function

    private void RegainHealth(int healthRegained)
    {
        if (hp + healthRegained >= maxHp)
            hp = maxHp;
        else hp += healthRegained;
    }

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
        StartCoroutine(DamageColorChange());
        PlayerEvent.UpdateHealth((float)hp / maxHp);
    }

    private IEnumerator DamageColorChange()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sprite.color = Color.white;
    }

    /// <summary>
    /// make the player disappear
    /// </summary>
    private void Death()
    {
        //TODO fix bug where player doesn't lose weapon on death
        GameStateManager.UpdateGameState(GameState.GameOver);
        foreach(Weapon weapon in Inventory.Weapons)
        {
            weapon.StopAttack(this);
        }
        Inventory.Weapons.Clear();
        currentCharacter = null;
        sprite.color = Color.white;
        transform.position = startingPos;
    }
    #endregion
}

public static class PlayerEvent
{
    #region Health event

    public static event Action<int> OnRegainHealth;
    public static void RegainHealth(int healthRegained) => OnRegainHealth?.Invoke(healthRegained);

    public static event Action<float> OnUpdateHealth;
    public static void UpdateHealth(float health) => OnUpdateHealth?.Invoke(health);
    #endregion

    #region Xp event
    public static event Action<int> OnSetXpToLevelUp;
    public static void SetXpToLevelUp(int xpAmount) => OnSetXpToLevelUp?.Invoke(xpAmount);

    public static event Action<int> OnXpGain;
    public static void XpGain(int currentXpPoint) => OnXpGain?.Invoke(currentXpPoint);

    public static event Action<int> OnLevelUp;
    public static void LevelUp(int currentLevel) => OnLevelUp?.Invoke(currentLevel);

    public static event Action OnMaxlevelAttained;
    public static void MaxlevelAttained() => OnMaxlevelAttained?.Invoke();
    #endregion

    public static event Action<int, int> OnAttackLand;
    public static void AttackLand(int objectId, int damage) => OnAttackLand?.Invoke(objectId, damage);

    public static event Action<SO_Character> OnCharacterChosen;
    public static void CharacterChosen(SO_Character characterToUse) => OnCharacterChosen?.Invoke(characterToUse);
}
