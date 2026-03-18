using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeybindingManager : MonoBehaviour
{
    public static KeybindingManager Instance;

    public KeyCode keyUp, keyLeft, keyRight;

    private void Awake()
    {
        Instance = this;
    }
}
