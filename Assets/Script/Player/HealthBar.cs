using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private SpriteRenderer healthBar;

    private void OnEnable()
    {
        healthBar = GetComponent<SpriteRenderer>();
        PlayerEvent.OnUpdateHealth += UpdateHealthBar;
    }

    private void OnDisable()
    {
        PlayerEvent.OnUpdateHealth -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float ratio)
    {
        healthBar.size = new Vector2(ratio, healthBar.size.y);
    }
}
