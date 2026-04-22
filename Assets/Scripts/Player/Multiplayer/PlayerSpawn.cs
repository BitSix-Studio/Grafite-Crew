using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public static PlayerSpawn Instance;

    public Transform[] spawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        NetworkManager.Instance.spawnPoints = spawnPoints;
    }

    public void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.Log("SpawnPoints não configurados!");
                return;
            }

            // SPAWN DO PLAYER
            int index = player.RawEncoded % spawnPoints.Length;
            Vector3 spawnPosition = spawnPoints[index].position;

            NetworkObject networkPlayerObj = runner.Spawn(NetworkManager.Instance.playerPrefab, spawnPosition, Quaternion.identity, player);
            // Manter o controle dos avatares dos jogadores para fácil acesso
            if (!NetworkManager.Instance.spawnedCharacters.ContainsKey(player))
            {
                NetworkManager.Instance.spawnedCharacters.Add(player, networkPlayerObj);
            }
        }
    }
}
