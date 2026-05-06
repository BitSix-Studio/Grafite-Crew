using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager Instance;

    private NetworkRunner runner;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    // CREATE AND ENTER LOBBY GAME
    public async void StartLobby()
    {
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;
            runner.AddCallbacks(this);
        }

        await runner.JoinSessionLobby(SessionLobby.Shared);

        Debug.Log("Entrou no Lobby");
    }

    // CREATE ROOM
    public async void StartHost(string roomName)
    {
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = roomName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 2
        });
    }

    // JOIN ROOM
    public async void JoinGame(string roomName)
    {
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = roomName
        });
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    // PLAYER INPUTS CALL
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeybindingManager.Instance.keyLeft))
            data.targetDirection = Vector3.left;

        if (Input.GetKey(KeybindingManager.Instance.keyRight))
            data.targetDirection = Vector3.right;

        if(Input.GetKey(KeybindingManager.Instance.keyUp))
            data.buttons.Set(InputButtons.Jump, true);

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public NetworkPrefabRef playerPrefab;
    [HideInInspector] public Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    public Transform[] spawnPoints;
    public TextMeshProUGUI playersConnectedText;

    // PLAYER CONTROL ENTERING THE ROOM
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // CHECK THE NUMBER OF PLAYERS
        //int playerCount = runner.ActivePlayers.Count();

        //if (playerCount < 2)
        //{
        //    playersConnectedText.text = $"Esperando Adversário... ({playerCount}/2)";
        //}
        //else if (playerCount >= 2)
        //{
        //    playersConnectedText.text = $"Adversário Encontrado! Iniciando... ({playerCount}/2)";

        //    if (runner.IsSceneAuthority)
        //    {
        //        runner.LoadScene("Arena1v1");
        //    }
        //}
        runner.LoadScene("Arena1v1");
    }

    // PLAYER CONTROL LEFT THE ROOM
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    // WHEN THE SCENE FINISHES LOADING
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (runner.IsServer)
        {
            foreach (var player in runner.ActivePlayers)
            {
                if(PlayerSpawn.Instance != null)
                    PlayerSpawn.Instance.SpawnPlayer(runner, player);
            }
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public Transform panelList;
    public GameObject roomPrefab;

    // LIST OF ROOMS CREATED
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        foreach (Transform child in panelList)
        {
            Destroy(child.gameObject);
        }

        foreach (var session in sessionList)
        {
            GameObject obj = Instantiate(roomPrefab, panelList);

            RoomItem item = obj.GetComponent<RoomItem>();
            item.Setup(session.Name);
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
