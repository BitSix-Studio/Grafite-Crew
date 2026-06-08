using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoorClose : NetworkBehaviour
{
    public CorridorController corridorController;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        corridorController.CloseDoor();
    }
}
