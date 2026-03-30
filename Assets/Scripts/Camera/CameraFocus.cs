using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    [Header("Initial Config")]
    public Transform target;
    public float offSetX, offSetY, offSetZ;
    [HideInInspector] public Vector3 cameraPos;

    private void Start()
    {
        transform.position = target.position;

        cameraPos = new Vector3(offSetX, offSetY, offSetZ);

        transform.position += cameraPos;
    }
}
