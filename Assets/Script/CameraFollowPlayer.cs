using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float offset;

    void LateUpdate()
    {
        transform.position = new Vector3(0, target.transform.position.y - offset, transform.position.z);
    }
}
