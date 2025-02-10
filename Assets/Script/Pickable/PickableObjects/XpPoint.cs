using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
public class XpPoint : PickableObject
{
    private Collider2D col;
    private SpriteRenderer sprite;

    [SerializeField] private SO_Xp xpSO;
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

    protected override void Pickup()
    {
        XpEvent.XpGain(xpSO.xpGiven);
        gameObject.SetActive(false);
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
