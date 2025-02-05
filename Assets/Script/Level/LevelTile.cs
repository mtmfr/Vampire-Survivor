using System;
using Unity.VisualScripting;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 3)
            return;

        PlayerLeftBounds(ExitPoint(collision.transform.position));
    }

    public static event Action<Vector3> OnPlayerLeft;

    /// <summary>
    /// Lauch the OnPlayerLeft event
    /// </summary>
    /// <param name="leavingPosition">the possition of the object that left</param>
    private void PlayerLeftBounds(Vector2 leavingPosition)
    {
        OnPlayerLeft?.Invoke(leavingPosition);
    }

    private Vector2 ExitPoint(Vector3 position)
    {
        Collider2D col = GetComponent<Collider2D>();
        Vector3 posFromCenter = col.bounds.ClosestPoint(position);

        posFromCenter -= col.bounds.center;
        posFromCenter = posFromCenter.Divide(posFromCenter, col.bounds.extents);

        if (Mathf.Abs(posFromCenter.x) > Mathf.Abs(posFromCenter.y))
            posFromCenter.Set(posFromCenter.x, 0, 0);

        else if (Mathf.Abs(posFromCenter.x) < Mathf.Abs(posFromCenter.y))
            posFromCenter.Set(0, posFromCenter.y, 0);

        else posFromCenter.Set(posFromCenter.x, posFromCenter.y, 0);

        return posFromCenter;
    }
}