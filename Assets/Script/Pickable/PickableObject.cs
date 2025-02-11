using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PickableObject : MonoBehaviour
{
    [field: SerializeField] public int Weight { get; private set; }

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
    }

    private void OnEnable()
    {
        GameStateManager.OnGameStateChange += OnGameEnd;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChange -= OnGameEnd;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
            Pickup();
    }

    protected abstract void Pickup();

    private void OnGameEnd(GameState gameState)
    {
        if (gameState != GameState.GameOver)
            return;
        
        gameObject.SetActive(false);
    }
}
