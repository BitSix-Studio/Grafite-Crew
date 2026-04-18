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

        if (Object.HasInputAuthority)
        {
            var cam = Camera.main.GetComponent<CameraFocus>();

            if (Object.InputAuthority.RawEncoded == 0)
            {
                cam.offSet = new Vector3(5.5f, 1, -8.5f);
            }
            else
            {
                cam.offSet = new Vector3(-25, 1, -8.5f); // lado oposto
            }

            cam.CamFocusPlayer(transform);
        }
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
