using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await StartLobby();
    }

    // CREATE AND ENTER LOBBY GAME
    public async Task StartLobby()
    {
        if (runner != null)
        {
            await runner.Shutdown();
            Destroy(runner.gameObject);
            runner = null;
        }

        var runnerObj = new GameObject("NetworkRunner");
        runner = runnerObj.AddComponent<NetworkRunner>();

        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        await runner.JoinSessionLobby(SessionLobby.Shared);

        Debug.Log("Entrou no Lobby");
    }

    // CREATE ROOM
    public async Task StartHost(string roomName)
    {
        await StartLobby();

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
        if (runner == null)
            StartLobby();

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

        // KEYBOARD
        if (Input.GetKey(KeybindingManager.Instance.keyLeft))
            data.targetDirection = Vector3.left;

        if (Input.GetKey(KeybindingManager.Instance.keyRight))
            data.targetDirection = Vector3.right;

        if(Input.GetKey(KeybindingManager.Instance.keyUp))
            data.buttons.Set(InputButtons.Jump, true);

        //MOBILE
        if (PlatformHelper.IsMobile() && MobileInput.Instance != null)
        {
            data.targetDirection = MobileInput.Instance.MoveDirection;

            if (MobileInput.Instance.JumpPressed)
                data.buttons.Set(InputButtons.Jump, true);

            //if (MobileInput.Instance.SlidePressed)
            //    data.buttons.Set(InputButtons.Slide, true);

            MobileInput.Instance.ResetButtons();
        }

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
        if (!runner.IsServer)
            return;

        //CHECK THE NUMBER OF PLAYERS
        int playerCount = runner.ActivePlayers.Count();

        if (playerCount < 2)
        {
            playersConnectedText.text = $"Esperando Adversário... ({playerCount}/2)";
        }
        else if (playerCount >= 2)
        {
            playersConnectedText.text = $"Adversário Encontrado! Iniciando... ({playerCount}/2)";

            if (runner.IsSceneAuthority)
            {
                runner.LoadScene("Arena1v1");
            }
        }
        //runner.LoadScene("Arena1v1");
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
        if (!runner.IsServer)
            return;

        StartCoroutine(SpawnPlayers(runner));
    }

    private IEnumerator SpawnPlayers(NetworkRunner runner)
    {
        yield return new WaitForSeconds(0.1f);

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.Log("SpawnPoints não configurados!");
            yield return null;
        }

        foreach (var player in runner.ActivePlayers)
        {
            // SPAWN DO PLAYER
            int index = player.RawEncoded % spawnPoints.Length;
            Vector3 spawnPosition = spawnPoints[index].position;

            var obj = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

            obj.GetComponent<PlayerControllerMultiplayer>().PlayerIndex = index;

            spawnedCharacters[player] = obj;
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public event Action<List<SessionInfo>> SessionListUpdated;
    // LIST OF ROOMS CREATED
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        SessionListUpdated?.Invoke(sessionList);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
