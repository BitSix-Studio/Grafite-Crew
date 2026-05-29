using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[System.Serializable]
public class Character
{
    public string characterName;
    public Sprite characterIcon;
    public string characterNameHability;
    public string characterDescription;
    public NetworkPrefabRef characterPrefab;
    public GameObject prefabCharDisplay;
}
