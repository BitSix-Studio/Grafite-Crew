using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeybindingManager : MonoBehaviour
{
    public static KeybindingManager Instance;

    public KeyCode keyUp, keyDown, keyLeft, keyRight;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        DontDestroyOnLoad(gameObject);
    }
}
