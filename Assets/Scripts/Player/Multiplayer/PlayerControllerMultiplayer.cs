using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMultiplayer : NetworkBehaviour
{
    [Header("Move Config")]
    public float speed;

    [HideInInspector] public bool canMove, canJump, canSlide;

    private NetworkCharacterController networkController;

    public override void Spawned()
    {
        networkController = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (Object.HasStateAuthority)
            {
                data.Direction.Normalize();
                networkController.Move(speed * data.Direction * Runner.DeltaTime);
            }
        }
    }
}
