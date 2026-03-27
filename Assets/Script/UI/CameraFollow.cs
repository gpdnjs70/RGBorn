using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);

    void LateUpdate() // Update 대신 이걸 써야 안 흔들립니다.
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
