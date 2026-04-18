using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    [Header("Initial Config")]
    public Vector3 offSet;

    public void CamFocusPlayer(Transform target)
    {
        if (target == null) return;

        transform.position = target.position + offSet;
    }
}
