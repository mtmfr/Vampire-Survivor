using System;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelTile : MonoBehaviour
{
    public bool isSpawner { get; private set; }
    public Vector2Int posId { get; private set; }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 3)
            return;

        PlayerLeftBounds(ExitPoint(collision.transform.position));
    }

    public static event Action<Vector3> OnPlayerLeft;

    public void SetIsSpawner(bool isSpawner)
    {
        this.isSpawner = isSpawner;
    }

    /// <summary>
    /// Define the position id of this tile as vector2.
    /// <br> x = 1, y = 1 is up right</br>
    /// <br>x = -1, y = -1 is down left</br>
    /// </summary>
    public void SetPosId(int x, int y)
    {
        posId = new(x, y);
    }

    /// <summary>
    /// Lauch the OnPlayerLeft event
    /// </summary>
    /// <param name="leavingPosition">the possition of the object that left</param>
    private void PlayerLeftBounds(Vector3 leavingPosition)
    {
        OnPlayerLeft?.Invoke(leavingPosition);
    }

    //Get the the exit poitn of the player at either -1, 1, 0
    private Vector3 ExitPoint(Vector3 position)
    {
        //Get the collider
        Collider2D col = GetComponent<Collider2D>();
        Vector3 posFromCenter = col.bounds.ClosestPoint(position);

        //Get the position of the bound relative to the center
        posFromCenter -= col.bounds.center;

        //divide the position from the center by the extent to get a result that is eiter 1 or 0
        posFromCenter = posFromCenter.Divide(posFromCenter, col.bounds.extents);

        //Compare the value of x and y
        //if x is greater set y to 0
        if (Mathf.Abs(posFromCenter.x) > Mathf.Abs(posFromCenter.y))
            posFromCenter.Set(posFromCenter.x, 0, 0);

        //if y is greater set x to 0
        else if (Mathf.Abs(posFromCenter.x) < Mathf.Abs(posFromCenter.y))
            posFromCenter.Set(0, posFromCenter.y, 0);

        //if x and y are equal then nothing happen
        else posFromCenter.Set(posFromCenter.x, posFromCenter.y, 0);

        return posFromCenter;
    }
}