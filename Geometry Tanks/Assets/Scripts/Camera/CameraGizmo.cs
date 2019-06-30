using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGizmo : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // Gizmo Frustum
        Gizmos.matrix = transform.localToWorldMatrix;           // For the rotation bug
        Gizmos.DrawFrustum(transform.position, Camera.main.fieldOfView, Camera.main.nearClipPlane, Camera.main.farClipPlane, Camera.main.aspect);
    }
}
