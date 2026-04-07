using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerManager : MonoBehaviour
{
    public GameObject canvas;
    private void Start()
    {
        canvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0f;
            canvas.SetActive(true);
        }
    }
}
