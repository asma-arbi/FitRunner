using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 4f, -7f);

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = new Vector3(target.position.x * 0.5f, offset.y, target.position.z + offset.z);
        transform.LookAt(new Vector3(target.position.x * 0.5f, 1f, target.position.z + 5f));
    }
}