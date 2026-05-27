using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    public static ManagerUI Instance;
    [Header("UI of Connection Panel")]
    public GameObject networkConnectPanel;
    public TMP_InputField inputRoom;
    public Transform panelList;
    public GameObject roomPrefab;

    [Header("UI of Indicator Players Connect")]
    public GameObject waitConnectPlayersPanel;
    public TextMeshProUGUI playersConnectedText;

    [Header("Menu Buttons")]
    public Button playGameBtn;

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
            NetworkManager.Instance.SessionListUpdated += RefreshRooms;
    }

    private void OnDisable() 
    { 
        if (NetworkManager.Instance != null)
            NetworkManager.Instance.SessionListUpdated -= RefreshRooms;
    }

    private async void Start() 
    { 
        await Task.Yield(); 
        
        while (GameManager.Instance == null && NetworkManager.Instance == null) 
        {
            await Task.Yield();
        } 
        
        await NetworkManager.Instance.StartLobby();

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
        NetworkManager.Instance.playersConnectedText = playersConnectedText; 
    }

    public async void CreateRoom() 
    { 
        await NetworkManager.Instance.StartHost(inputRoom.text); 
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

    public async void ResetGame() 
    { 
        Time.timeScale = 1f; 

        if (NetworkManager.Instance != null) 
            await NetworkManager.Instance.ShutdownRunner(); 

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