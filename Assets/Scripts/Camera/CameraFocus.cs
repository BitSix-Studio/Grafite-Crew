using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    [Header("Initial Config")]
    public Transform playerOneCam, playerTwoCam;

    public void CamFocusPlayer(Transform playerCam)
    {
        if (playerCam == null) return;

        transform.position = playerCam.position;
    }
}
