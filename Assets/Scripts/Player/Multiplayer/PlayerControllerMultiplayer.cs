using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMultiplayer : NetworkBehaviour
{
    [Networked] public NetworkBool canMove { get; set; }

    [HideInInspector] public NetworkCharacterController networkController;
    public CameraFocus cam;
    [Networked] public int PlayerIndex { get; set; }
    private Vector3 currentDirection;

    [HideInInspector] public NetworkButtons previousButtons;
    public bool jumpPressedThisTick { get; private set; }
    public bool jumpConsumed = false;

    public override void Spawned()
    {
        networkController = GetComponent<NetworkCharacterController>();

        canMove = true;

        if (!Object.HasInputAuthority)
            return;

        StartCoroutine(SetupCamera());
    }

    private IEnumerator SetupCamera()
    {
        while (Camera.main == null)
        {
            yield return null;
        }

        cam = Camera.main.GetComponent<CameraFocus>();

        if (cam == null)
        {
            yield break;
        }

        if (PlayerIndex == 0)
        {
            cam.CamFocusPlayer(cam.playerOneCam);
        }
        else
        {
            cam.CamFocusPlayer(cam.playerTwoCam);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!canMove)
            return;

        if (GetInput(out NetworkInputData data))
        {
            jumpPressedThisTick = data.buttons.WasPressed(previousButtons, InputButtons.Jump);
            if (Object.HasStateAuthority)
            {
                data.targetDirection.Normalize();

                currentDirection = Vector3.Lerp(currentDirection, data.targetDirection, networkController.rotationSpeed * Runner.DeltaTime);
                MovePlayer(currentDirection);

                JumpPlayer();

                previousButtons = data.buttons;
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

    public void JumpPlayer()
    {
        if (jumpPressedThisTick && !jumpConsumed && IsGrounded())
        {
            networkController.Jump(true);
        }
    }

    public bool IsGrounded()
    {
        //return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        return networkController.Grounded;
    }

    public void UpdateElevatorCamera(float offset)
    {
        if (!Object.HasInputAuthority || cam == null)
            return;

        cam.SetElevatorOffset(offset);
    }

    public void ResetElevatorCamera()
    {
        if (!Object.HasInputAuthority || cam == null)
            return;

        cam.ResetElevatorOffset();
    }
}
