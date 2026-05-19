using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject winPanel;
    public GameObject losePanel;
    private bool resultShown;

    public NetworkPrefabRef winCollider;

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

    public void RegisterUI(GameObject win, GameObject lose)
    {
        winPanel = win;
        losePanel = lose;

        resultShown = false;

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);
    }

    private bool UIValid()
    {
        return winPanel != null && losePanel != null;
    }

    public void WinGame(Vector3 posCollider)
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        posCollider.x += 12.5f;
        posCollider.y += 1f;
        runner.Spawn(winCollider, posCollider, Quaternion.identity);
    }

    public void FinishMatch(PlayerRef winner)
    {
        if (Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL");
            return;
        }

        RPC_MatchFinished(winner);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_MatchFinished(PlayerRef winner, RpcInfo info = default)
    {
        ShowResult(winner);
    }

    private void ShowResult(PlayerRef winner)
    {
        if (resultShown)
            return;

        if (!UIValid())
        {
            Debug.LogWarning("UI not registered yet.");
            return;
        }

        resultShown = true;

        FreezePlayers();

        var localPlayer = FindLocalPlayer();

        if (localPlayer == null)
            return;

        if (localPlayer.Object.InputAuthority == winner)
        {
            winPanel.SetActive(true);
        }
        else
        {
            losePanel.SetActive(true);
        }
    }

    private PlayerControllerMultiplayer FindLocalPlayer()
    {
        foreach (var p in FindObjectsOfType<PlayerControllerMultiplayer>())
        {
            if (p.Object.HasInputAuthority)
                return p;
        }
        return null;
    }

    private void FreezePlayers()
    {
        foreach (var p in FindObjectsOfType<PlayerControllerMultiplayer>())
        {
            p.canMove = false;
        }
    }
}
