using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    public GameObject networkConnectPanel;
    public TMP_InputField inputRoom;

    public GameObject waitConnectPlayersPanel;
    public Button playGameBtn;

    private void Start()
    {
        if(networkConnectPanel != null)
            networkConnectPanel.SetActive(false);

        if(waitConnectPlayersPanel != null)
            waitConnectPlayersPanel.SetActive(false);
    }

    public void PlayGame()
    {
        NetworkManager.Instance.StartLobby();
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
