using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerManager : MonoBehaviour
{
    public GameObject winPanel;
    private void Start()
    {
        winPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0f;
            winPanel.SetActive(true);
        }
    }
}
