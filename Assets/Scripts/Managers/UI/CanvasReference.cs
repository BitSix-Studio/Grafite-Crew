using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasReference : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;
    public Button[] resetBtn;

    public GameObject networkConnectPanel;
    public TMP_InputField inputRoom;

    public GameObject waitConnectPlayersPanel;
    public Button playGameBtn;

    public Transform panelList;
    public GameObject roomPrefab;

    private IEnumerator Start()
    {
        yield return null;

        while (GameManager.Instance == null && ManagerUI.Instance == null)
        {
            yield return null;
        }

        ManagerUI.Instance.RegisterUI(
            networkConnectPanel,
            inputRoom,
            waitConnectPlayersPanel,
            playGameBtn,
            panelList,
            roomPrefab
        );

        if (resetBtn != null)
        {
            foreach (Button b in resetBtn)
            {
                if (b == null)
                    continue;

                b.onClick.AddListener(() =>
                {
                    if (ManagerUI.Instance != null)
                    {
                        ManagerUI.Instance.ResetGame();
                    }
                });
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterUI(winPanel, losePanel);
        }

        if (winPanel != null)
            winPanel.SetActive(false);
        if(losePanel != null)
            losePanel.SetActive(false);
    }
}
