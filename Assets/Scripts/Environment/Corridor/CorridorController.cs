using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CorridorController : NetworkBehaviour
{
    public NetworkObject reference;
    public float offset = 10f;

    public GameObject elevatorCloseRight, elevatorCloseLeft, elevatorExitRight, elevatorExitLeft;
    public Collider elevatorColliderRight, elevatorColliderLeft;
    public Animator animDoorRight, animDoorLeft;
    [Networked] public NetworkBool RightDoorOpen { get; set; }

    [Networked] public NetworkBool LeftDoorOpen { get; set; }

    public static int left = 1, right = 0;

    [Networked] public NetworkBool IsRightSide { get; set; }

    [Header("Background")]
    public Renderer backgroundRenderer;
    public Texture2D[] randomBackgrounds;

    NetworkObject objNetwork;

    public bool corridorInitScene;

    public override void Spawned()
    {
        objNetwork = GetComponent<NetworkObject>();

        RandomizeBackground();

        if (!corridorInitScene)
            return;

        AlternateElevator();
        OpenDoor();
    }

    public void Init(NetworkObject playerRef)
    {
        reference = playerRef;
    }

    public override void FixedUpdateNetwork()
    {
        if (reference == null)
            return;

        if (transform.position.y < reference.transform.position.y - offset)
        {
            Runner.Despawn(objNetwork);
        }
    }

    public void SetSide(bool right)
    {
        IsRightSide = right;
        AlternateElevator();
        OpenDoor();

        if(IsRightSide)
            OpenLeftDoor();
        else
            OpenRightDoor();
    }

    public void AlternateElevator()
    {
        if (IsRightSide) // ELEVADOR DIREITA
        {
            if (elevatorColliderRight != null)
                elevatorColliderRight.enabled = true;
            if (elevatorColliderLeft != null)
                elevatorColliderLeft.enabled = false;

            if(elevatorExitRight != null)
                elevatorExitRight.SetActive(false);
            if(elevatorExitLeft != null)
                elevatorExitLeft.SetActive(true);
        }
        else // ELEVADOR ESQUERDA
        {
            if (elevatorColliderRight != null)
                elevatorColliderRight.enabled = false;
            if (elevatorColliderLeft != null)
                elevatorColliderLeft.enabled = true;

            if (elevatorExitRight != null)
                elevatorExitRight.SetActive(true);
            if (elevatorExitLeft != null)
                elevatorExitLeft.SetActive(false);
        }
    }

    public void CloseDoor()
    {
        if (!Object.HasStateAuthority)
            return;

        if (IsRightSide)
        {
            RightDoorOpen = false;
        }
        else
        {
            LeftDoorOpen = false;
        }
    }

    public void OpenDoor()
    {
        if (!Object.HasStateAuthority)
            return;

        if (IsRightSide)
        {
            RightDoorOpen = true;
        }
        else
        {
            LeftDoorOpen = true;
        }
    }

    public void OpenRightDoor()
    {
        if (!Object.HasStateAuthority)
            return;

        RightDoorOpen = true;
    }

    public void OpenLeftDoor()
    {
        if (!Object.HasStateAuthority)
            return;

        LeftDoorOpen = true;
    }

    public void CloseRightDoor()
    {
        if (!Object.HasStateAuthority)
            return;

        RightDoorOpen = false;
    }

    public void CloseLeftDoor()
    {
        if (!Object.HasStateAuthority)
            return;

        LeftDoorOpen = false;
    }

    public override void Render()
    {
        if (animDoorRight != null)
            animDoorRight.SetBool("IsOpen", RightDoorOpen);

        if (animDoorLeft != null)
            animDoorLeft.SetBool("IsOpen", LeftDoorOpen);

        if (elevatorCloseRight != null)
            elevatorCloseRight.SetActive(!RightDoorOpen);

        if (elevatorCloseLeft != null)
            elevatorCloseLeft.SetActive(!LeftDoorOpen);
    }

    private void RandomizeBackground()
    {
        if (randomBackgrounds.Length == 0)
            return;

        int randomIndex = Random.Range(0, randomBackgrounds.Length);

        Texture2D selectedTexture = randomBackgrounds[randomIndex];

        backgroundRenderer.material.SetTexture("_BaseMap", selectedTexture);
    }
}
