using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    [Header("Initial Config")]
    public Transform playerOneCam, playerTwoCam;

    private Transform currentCamTarget;
    private Vector3 elevatorOffset;

    public void CamFocusPlayer(Transform playerCam)
    {
        currentCamTarget = playerCam;
        if (playerCam == null) return;

        transform.position = playerCam.position + elevatorOffset;
    }

    private void LateUpdate()
    {
        if (currentCamTarget == null) return;

        transform.position = currentCamTarget.position + elevatorOffset;
    }

    public void SetElevatorOffset(float height)
    {
        elevatorOffset = Vector3.up * height;
    }

    public void ResetElevatorOffset()
    {
        elevatorOffset = Vector3.zero;

        var yAbsolute = currentCamTarget.position.y;
        yAbsolute = Mathf.RoundToInt(transform.position.y);
        currentCamTarget.position = new Vector3(transform.position.x, yAbsolute, transform.position.z);
    }
}
