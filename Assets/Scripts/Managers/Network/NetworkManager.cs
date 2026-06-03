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

    // CREATE AND ENTER LOBBY GAME
    public async Task StartLobby()
    {
        if (runner != null)
        {
            Debug.Log("Already in lobby");
            return;
        }

        await ShutdownRunner();

        var runnerObj = new GameObject("NetworkRunner");
        runner = runnerObj.AddComponent<NetworkRunner>();

        runnerObj.AddComponent<NetworkSceneManagerDefault>();

        runner.ProvideInput = true; 
        runner.AddCallbacks(this);

        await runner.JoinSessionLobby(SessionLobby.Shared);

        Debug.Log("Entered Lobby");
    }

    // CREATE ROOM
    public async Task StartHost(string roomName)
    {
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = roomName,
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 2
        });

        playerSelections[runner.LocalPlayer] = PlayerSelection.CharacterId;
    }

    // JOIN ROOM
    public async void JoinGame(string roomName)
    {
        byte[] token = BitConverter.GetBytes(PlayerSelection.CharacterId);

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = roomName,
            ConnectionToken = token
        });
    }

    public async Task ShutdownRunner()
    {
        if (runner == null)
            return;

        runner.RemoveCallbacks(this);

        await runner.Shutdown();

        Destroy(runner.gameObject, 0.1f);

        runner = null;

        await Task.Delay(500);
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

        if (Input.GetKey(KeybindingManager.Instance.keyUp))
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

    [HideInInspector] public Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private Dictionary<PlayerRef, int> playerSelections = new Dictionary<PlayerRef, int>();

    public Transform[] spawnPoints;
    public TextMeshProUGUI playersConnectedText;

    // PLAYER CONTROL ENTERING THE ROOM
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player entrou: {player}");
        Debug.Log($"LocalPlayer: {runner.LocalPlayer}");

        if (!runner.IsServer)
            return;

        if (player != runner.LocalPlayer)
        {
            byte[] token = runner.GetPlayerConnectionToken(player);

            Debug.Log($"Token recebido? {(token == null ? "NULL" : token.Length.ToString())}");

            if (token != null && token.Length >= 4)
            {
                int characterId = BitConverter.ToInt32(token, 0);

                Debug.Log($"Character recebido: {characterId}");

                playerSelections[player] = characterId;
            }
        }

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

    [SerializeField] private CharacterDatabase characterDB;

    private NetworkPrefabRef GetCharacterPrefab(int characterId)
    {
        foreach (var character in characterDB.characters)
        {
            if (character.characterId == characterId)
                return character.characterPrefab;
        }

        Debug.LogError($"Character ID {characterId} não encontrado!");

        return default;
    }

    public void RegisterCharacter(PlayerRef player, int characterId)
    {
        playerSelections[player] = characterId;

        Debug.Log(
            $"Jogador {player} escolheu personagem {characterId}"
        );
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

            if (!playerSelections.TryGetValue(player, out int characterId))
            {
                Debug.LogWarning($"Personagem não encontrado para {player}");
                characterId = 0;
            }

            NetworkPrefabRef prefab = GetCharacterPrefab(characterId);

            var obj = runner.Spawn(prefab, spawnPosition, Quaternion.Euler(0, -90, 0), player);

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
