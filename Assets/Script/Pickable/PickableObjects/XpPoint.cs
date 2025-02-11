using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
public class XpPoint : PickableObject
{
    private Collider2D col;

    [SerializeField] private SO_Xp xpSO;
    LayerMask playerLayer = 1 << 3;
    LayerMask enemyLayer = 1 << 6;

    protected override void Start()
    {
        base.Start();
        col = GetComponent<Collider2D>();
        col.includeLayers = playerLayer;
        col.excludeLayers = enemyLayer;

        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = xpSO.xpSprite;
    }

    protected override void Pickup()
    {
        XpEvent.XpGain(xpSO.xpGiven);
        StartCoroutine(PlaySfx());
    }

    private IEnumerator PlaySfx()
    {
        sfx.Play();
        sprite.enabled = false;
        yield return new WaitForSeconds(sfx.clip.length);
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
