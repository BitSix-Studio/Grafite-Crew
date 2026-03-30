using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHelper : MonoBehaviour
{
    public PlatformSettings settings;

    public static PlatformSettings Settings;

    private void Awake()
    {
        Settings = settings;
    }

    public static bool IsMobile()
    {
        #if UNITY_EDITOR
                return Settings != null && Settings.forceMobileInEditor;
        #else
                return Application.isMobilePlatform;
        #endif
    }
}
