using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
public class XpPoint : MonoBehaviour
{
    private Collider2D col;
    private SpriteRenderer sprite;

    [SerializeField] private XpScriptableObject xpSO;
    LayerMask playerLayer = 1 << 3;
    LayerMask enemyLayer = 1 << 6;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        col.includeLayers = playerLayer;
        col.excludeLayers = enemyLayer;

        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = xpSO.xpSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        XpEvent.XpGain(xpSO.xpGiven);
        Destroy(gameObject);
    }
}

public static class XpEvent
{
    public static event Action<int> OnXpGain;
    public static void XpGain(int xp)
    {
        OnXpGain?.Invoke(xp);
    }
}
