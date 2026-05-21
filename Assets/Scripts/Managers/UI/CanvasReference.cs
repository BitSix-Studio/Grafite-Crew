using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasReference : MonoBehaviour
{
    [Header("UI of ARENA")]
    public GameObject winPanel;
    public GameObject losePanel;
    public Button[] resetBtn;

    private IEnumerator Start()
    {
        yield return null;

        while (GameManager.Instance == null && ManagerUI.Instance == null)
        {
            yield return null;
        }

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
