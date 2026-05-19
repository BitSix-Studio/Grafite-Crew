using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    public static ManagerUI Instance;

    public GameObject networkConnectPanel;
    public TMP_InputField inputRoom;

    public GameObject waitConnectPlayersPanel;
    public Button playGameBtn;
    public Button createRoomBtn;

    public Transform panelList;
    public GameObject roomPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.SessionListUpdated += RefreshRooms;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.SessionListUpdated -= RefreshRooms;
        }
    }
    public void RegisterUI(
    GameObject connectPanel,
    TMP_InputField roomInput,
    GameObject waitingPanel,
    Button playBtn,
    Transform roomList,
    GameObject roomPrefabObj)
    {
        networkConnectPanel = connectPanel;
        inputRoom = roomInput;
        waitConnectPlayersPanel = waitingPanel;
        playGameBtn = playBtn;
        panelList = roomList;
        roomPrefab = roomPrefabObj;

        InitializeUI();
    }

    private void InitializeUI()
    {
        if (networkConnectPanel != null)
            networkConnectPanel.SetActive(false);

        if (waitConnectPlayersPanel != null)
            waitConnectPlayersPanel.SetActive(false);

        if (playGameBtn != null)
            playGameBtn.onClick.AddListener(() => PlayGame());

        Debug.Log("UI Initialized");
    }

    private void RefreshRooms(List<SessionInfo> sessions)
    {
        if (!panelList || !roomPrefab)
            return;

        foreach (Transform child in panelList)
        {
            Destroy(child.gameObject);
        }

        foreach (var session in sessions)
        {
            GameObject obj = Instantiate(roomPrefab, panelList);

            RoomItem item = obj.GetComponent<RoomItem>();
            item.Setup(session.Name);
        }
    }

    public void PlayGame()
    {
        networkConnectPanel.SetActive(true);
    }

    public void CreateRoom()
    {
        NetworkManager.Instance.StartHost(inputRoom.text);
        networkConnectPanel.SetActive(false);
        waitConnectPlayersPanel.SetActive(true);
        playGameBtn.interactable = false;
    }
    
    public void JoinRoom()
    {
        NetworkManager.Instance.JoinGame(inputRoom.text);
        networkConnectPanel.SetActive(false);
        waitConnectPlayersPanel.SetActive(true);
    }

    public void ResetGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
