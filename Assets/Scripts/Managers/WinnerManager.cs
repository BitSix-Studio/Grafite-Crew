using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerManager : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority)
            return;

        if (!other.CompareTag("Player"))
            return;

        NetworkObject playerObj = other.GetComponent<NetworkObject>();

        if (!Object.HasStateAuthority)
            return;

        GameManager.Instance.FinishMatch(playerObj.InputAuthority);
    }
}
