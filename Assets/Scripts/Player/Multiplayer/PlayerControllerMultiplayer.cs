using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.LightAnchor;

public class PlayerControllerMultiplayer : NetworkBehaviour
{
    [HideInInspector] public bool canMove, canSlide;

    private NetworkCharacterController networkController;
    private Vector3 currentDirection;

    private NetworkButtons previousButtons;
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private LayerMask groundLayer;
    private bool hasJumped;

    public override void Spawned()
    {
        networkController = GetComponent<NetworkCharacterController>();
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

                JumpPlayer(data);
            }
        }

        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);
    }

    void MovePlayer(Vector3 dir)
    {
        if (canMove)
        {
            networkController.Move(dir * networkController.maxSpeed * Runner.DeltaTime);
        }
    }

    public void JumpPlayer(NetworkInputData data)
    {
        bool jumpPressed = data.buttons.WasPressed(previousButtons, InputButtons.Jump);

        if (jumpPressed && IsGrounded())
        {
            networkController.Jump(true);
            hasJumped = true;
        }

        previousButtons = data.buttons;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}
