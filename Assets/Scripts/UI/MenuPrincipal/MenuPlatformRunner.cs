using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPlatformRunner : MonoBehaviour
{
    public Button quitBtn;

    // Start is called before the first frame update
    void Start()
    {
        SwapButtons();
    }

    void SwapButtons()
    {
        if (PlatformHelper.IsMobile())
        {
            // The mobile menu buttons appear.
            quitBtn.gameObject.SetActive(false);
        }
        else
        {
            // The PC menu buttons appear
            quitBtn.gameObject.SetActive(true);
        }
    }
}
