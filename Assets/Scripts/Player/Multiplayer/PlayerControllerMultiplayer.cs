using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerControllerMultiplayer : NetworkBehaviour
{
    [Networked] public NetworkBool canMove { get; set; }

    [HideInInspector] public NetworkCharacterController networkController;
    private Animator animator;
    [SerializeField] private GoUpElevator upElevator;

    [Header("Camera Player Config")]
    private CameraFocus cam;
    [Networked] public int PlayerIndex { get; set; }

    [Header("Move Character Config")]
    private Vector3 currentDirection;
    public float directionChangeSpeed;
    [Range(0f, 0.1f)] public float magnitudeLimitForStopAnimations;
    private int facingDirection = 1;

    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;

    [HideInInspector] public NetworkButtons previousButtons;
    public bool jumpPressedThisTick { get; private set; }

    private bool wasGrounded;

    public override void Spawned()
    {
        networkController = GetComponent<NetworkCharacterController>();
        animator = GetComponent<Animator>();
        upElevator = GetComponent<GoUpElevator>();

        wasGrounded = IsGrounded();
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
        float verticalSpeed = networkController.Velocity.y;

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", verticalSpeed);

        if (isGrounded && !wasGrounded)
        {
            animator.SetTrigger("Land");
        }

        wasGrounded = isGrounded;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(upElevator.elevatorCheckPoint.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void OnDrawGizmos()
    {
        if (upElevator == null)
            upElevator = GetComponent<GoUpElevator>();

        if (upElevator == null || upElevator.elevatorCheckPoint == null)
            return;

        Vector3 start = upElevator.elevatorCheckPoint.position;
        Vector3 end = start + Vector3.down * groundCheckDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, end);
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
