using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[System.Serializable]
public class Character
{
    public int characterId;

    [Header("Character Info")]
    public string characterName;
    public Sprite characterIcon;
    public NetworkPrefabRef characterPrefab;
    public GameObject prefabCharDisplay;

    [Header("Hability Character Info")]
    public string characterNameHability;
    public string characterDescription;
    public Sprite abilityIcon;
    public AbilityData ability;
}