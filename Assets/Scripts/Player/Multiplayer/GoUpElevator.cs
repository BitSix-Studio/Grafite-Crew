using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class GoUpElevator : NetworkBehaviour
{
    [Header("Settings")]
    public float targetPositionFloor = 6f;
    public float moveDuration = 1f;

    [Header("References")]
    public PlayerControllerMultiplayer player;

    [SerializeField] private LayerMask elevatorLayer;
    [SerializeField] private float checkRadius = 0.5f;
    public Transform elevatorCheckPoint;

    [Networked] private NetworkBool IsUsingElevator { get; set; }
    [Networked] private TickTimer MoveTimer { get; set; }
    [Networked] private Vector3 StartPos { get; set; }
    [Networked] private Vector3 TargetPos { get; set; }

    public NetworkPrefabRef prefabCorridor;
    private Vector3 AddNextCorridor = new Vector3(0, 6, 0);

    private bool nextIsRight;

    public int numCorridorsActual = 0;
    public int maxNumCorridors;

    public override void FixedUpdateNetwork()
    {
        ElevatorMovement();

        if (!GetInput(out NetworkInputData data))
            return;

        if (!Object.HasStateAuthority)
            return;

        if (player.jumpPressedThisTick && !IsUsingElevator && player.IsGrounded() && IsNearElevator())
        {
            UpElevator();
            CreateCorridor();
        }
    }

    private void UpElevator()
    {
        IsUsingElevator = true;

        StartPos = transform.position;
        TargetPos = StartPos + Vector3.up * targetPositionFloor;

        MoveTimer = TickTimer.CreateFromSeconds(Runner, moveDuration);

        player.canMove = false;
    }

    private void ElevatorMovement()
    {
        if (!IsUsingElevator)
            return;

        if (MoveTimer.Expired(Runner))
        {
            transform.position = TargetPos;
            player.UpdateElevatorCamera(targetPositionFloor);

            player.ResetElevatorCamera();

            player.canMove = true;
            IsUsingElevator = false;

            return;
        }

        float progress = 1f - (MoveTimer.RemainingTime(Runner).Value / moveDuration);
        Vector3 newPos = Vector3.Lerp(StartPos, TargetPos, progress);

        transform.position = newPos;

        player.UpdateElevatorCamera(transform.position.y - StartPos.y);
    }

    public void CreateCorridor()
    {
        if (numCorridorsActual >= maxNumCorridors)
            return;

        if (!Object.HasStateAuthority)
            return;

        if (player.PlayerIndex == 0)
            AddNextCorridor = new Vector3(0f, AddNextCorridor.y + targetPositionFloor, 0f);
        else
            AddNextCorridor = new Vector3(-25f, AddNextCorridor.y + targetPositionFloor, 0f);

        Vector3 nextCorridorPos = AddNextCorridor;

        NetworkObject newCorridor = Runner.Spawn(prefabCorridor, nextCorridorPos, Quaternion.identity);
        
        CorridorController corridor = newCorridor.GetComponent<CorridorController>();

        corridor.Init(player.GetComponent<NetworkObject>());

        nextIsRight = !nextIsRight;
        corridor.SetSide(nextIsRight);

        numCorridorsActual++;

        if (numCorridorsActual == maxNumCorridors - 1)
            GameManager.Instance.WinGame(nextCorridorPos);
    }

    private bool IsNearElevator()
    {
        Collider[] hits = Physics.OverlapSphere(elevatorCheckPoint.position, checkRadius, elevatorLayer, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Elevator"))
            {
                return true;
            }
        }

        return false;
    }
}
