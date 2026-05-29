using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMultiplayer : NetworkBehaviour
{
    [Networked] public NetworkBool canMove { get; set; }

    [HideInInspector] public NetworkCharacterController networkController;
    private Animator animator;

    [Header("Camera Player Config")]
    private CameraFocus cam;
    [Networked] public int PlayerIndex { get; set; }

    [Header("Move Character Config")]
    private Vector3 currentDirection;
    public float directionChangeSpeed;
    [Range(0f, 0.1f)] public float magnitudeLimitForStopAnimations;
    private int facingDirection = 1;

    [HideInInspector] public NetworkButtons previousButtons;
    public bool jumpPressedThisTick { get; private set; }

    private enum JumpState
    {
        Grounded,
        JumpStart,
        InAir,
        Landing
    }

    private JumpState jumpState;

    private bool wasGrounded;
    private bool justLanded;

    public override void Spawned()
    {
        networkController = GetComponent<NetworkCharacterController>();
        animator = GetComponent<Animator>();

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

                currentDirection = Vector3.Lerp(currentDirection, data.targetDirection, directionChangeSpeed * Runner.DeltaTime);
                currentDirection.z = 0f;

                MovePlayer(currentDirection);

                RotatePlayer(data);

                JumpPlayer();

                ChangeAnimations();
                ChangeJumpAnimations();

                // LOCKS THE Z-AXIS
                Vector3 pos = transform.position;
                pos.z = 0;
                transform.position = pos;

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

    void RotatePlayer(NetworkInputData data)
    {
        // SAVE LAST INPUT DIRECTION
        if (data.targetDirection.x > 0)
        {
            facingDirection = 1;
        }
        else if (data.targetDirection.x < 0)
        {
            facingDirection = -1;
        }

        // APPLY VISUAL ROTATION
        if (facingDirection == 1)
        {
            transform.localRotation = Quaternion.Euler(0, -90, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private void ChangeAnimations()
    {
        bool isMoving = networkController.Velocity.magnitude > magnitudeLimitForStopAnimations;

        if (isMoving)
        {
            animator.SetBool("IsRun", true);
        }
        else
        {
            animator.SetBool("IsRun", false);
        }
    }

    public void JumpPlayer()
    {
        if (jumpPressedThisTick && IsGrounded())
        {
            networkController.Jump(true);
        }
    }

    private void ChangeJumpAnimations()
    {
        bool isGrounded = IsGrounded();
        float yVel = networkController.Velocity.y;

        justLanded = !isGrounded && wasGrounded && yVel < 0f;

        if (isGrounded)
        {
            jumpState = JumpState.Grounded;
        }
        else
        {
            if (yVel > 0.1f)
            {
                jumpState = JumpState.JumpStart;
            }
            else if (yVel < -0.1f)
            {
                if (justLanded)
                    jumpState = JumpState.Landing;
                else
                    jumpState = JumpState.InAir;
            }
        }

        wasGrounded = isGrounded;

        animator.SetInteger("JumpState", (int)jumpState);
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
