using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // The target the camera will follow
    public Vector3 offset;    // The offset from the target

    void LateUpdate()
    {
        // Set the position of the camera to the target position plus the offset
        transform.position = target.position + offset;
    }
}
