using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : NetworkBehaviour
{
    public Transform[] spawnPoints;

    private void Start()
    {
        if(NetworkManager.Instance != null)
            NetworkManager.Instance.spawnPoints = spawnPoints;
    }
}
