using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PickableObject : MonoBehaviour
{
    [field: SerializeField] public int Weight { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
            Pickup();
    }

    protected abstract void Pickup();
}
