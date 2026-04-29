using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMultiplayer : NetworkBehaviour
{
    [HideInInspector] public bool canMove, canJump, canSlide;

    private NetworkCharacterController networkController;
    private Vector3 currentDirection;

    public override void Spawned()
    {
        networkController = GetComponent<NetworkCharacterController>();

        canJump = false;
        canSlide = true;
        canMove = true;

        if (Object.HasInputAuthority)
        {
            var cam = Camera.main.GetComponent<CameraFocus>();

            if (Object.HasStateAuthority)
            {
                cam.CamFocusPlayer(cam.playerOneCam);
            }
            else
            {
                cam.CamFocusPlayer(cam.playerTwoCam);
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (Object.HasStateAuthority)
            {
                data.targetDirection.Normalize();

                currentDirection = Vector3.Lerp(currentDirection, data.targetDirection, networkController.rotationSpeed * Runner.DeltaTime);
                MovePlayer(currentDirection);
            }
        }
    }

    void MovePlayer(Vector3 dir)
    {
        if (canMove)
        {
            networkController.Move(dir * networkController.maxSpeed * Runner.DeltaTime);
        }
    }
}
