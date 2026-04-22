using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomItem : MonoBehaviour
{
    public TextMeshProUGUI roomNameText;
    public Button joinButton;

    private string roomName;

    public void Setup(string name)
    {
        roomName = name;
        roomNameText.text = name;

        joinButton.onClick.AddListener(JoinRoom);
    }

    void JoinRoom()
    {
        NetworkManager.Instance.JoinGame(roomName);
    }
}