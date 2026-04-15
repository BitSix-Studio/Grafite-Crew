using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerUI : MonoBehaviour
{
    public NetworkManager networkManager;
    public GameObject networkConnectPanel;
    public TMP_InputField inputRoom;

    public void PlayGame()
    {
        SceneManager.LoadScene("SceneTeste");
    }

    public void CreateRoom()
    {
        networkManager.StartHost(inputRoom.text);
        networkConnectPanel.SetActive(false);
    }
    
    public void JoinRoom()
    {
        networkManager.JoinGame(inputRoom.text);
        networkConnectPanel.SetActive(false);
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
