using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitElevator : NetworkBehaviour
{
    public CorridorController corridorController;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        AlternateElevatorDoors();
    }

    void AlternateElevatorDoors()
    {
        if (corridorController.IsRightSide) // ELEVADOR DIREITA
        {
            corridorController.CloseLeftDoor();

            if (corridorController.elevatorCloseRight != null)
                corridorController.elevatorCloseRight.SetActive(false);
            if (corridorController.elevatorCloseLeft != null)
                corridorController.elevatorCloseLeft.SetActive(true);
        }
        else // ELEVADOR ESQUERDA
        {
            corridorController.CloseRightDoor();

            if (corridorController.elevatorCloseRight != null)
                corridorController.elevatorCloseRight.SetActive(true);
            if (corridorController.elevatorCloseLeft != null)
                corridorController.elevatorCloseLeft.SetActive(false);
        }

        Destroy(this);
    }
}
