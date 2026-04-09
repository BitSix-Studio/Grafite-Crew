using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlatformSettings", menuName = "Game/Platform Settings")]
public class PlatformSettings : ScriptableObject
{
    [Header("Debug")]
    public bool forceMobileInEditor;
}
