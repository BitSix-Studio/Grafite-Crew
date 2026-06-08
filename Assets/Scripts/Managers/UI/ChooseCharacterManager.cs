using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCharacterManager : MonoBehaviour
{
    [Header("Characters Database Reference")]
    public CharacterDatabase characterDB;

    [Header("UI References in the Scene")]
    public TextMeshProUGUI nameText;
    public Image iconChar, iconAbility;
    public TextMeshProUGUI nameHabilityText, descriptionText;
    public NetworkPrefabRef prefabChar;
    public GameObject prefabCharDisplay;

    [Header("Local for Display Character")]
    public Transform localCharacterDisplay;

    private static Vector3 rotationCharDisplay = new(0f, -40f, 0f);
    private static Vector3 scaleCharDisplay = new(40f, 40f, 40f);

    private int selectedOption = 0;
    private GameObject lastPrefabCharDisplay;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCharacter(selectedOption);
    }

    public void NextOption()
    {
        selectedOption++;

        if (selectedOption >= characterDB.characterCount)
        {
            selectedOption = 0;
        }

        UpdateCharacter(selectedOption);
    }

    public void BackOption()
    {
        selectedOption--;

        if (selectedOption < 0)
        {
            selectedOption = characterDB.characterCount - 1;
        }

        UpdateCharacter(selectedOption);
    }

    private void UpdateCharacter(int selected)
    {
        if (lastPrefabCharDisplay != null)
        {
            Destroy(lastPrefabCharDisplay);
        }

        Character character = characterDB.GetCharacter(selected);

        PlayerSelection.CharacterId = character.characterId;

        nameText.text = character.characterName;
        iconChar.sprite = character.characterIcon;
        iconAbility.sprite = character.abilityIcon;
        nameHabilityText.text = character.characterNameHability;
        descriptionText.text = character.characterDescription;
        prefabChar = character.characterPrefab;
        prefabCharDisplay = character.prefabCharDisplay;

        var characterObj = Instantiate(prefabCharDisplay, localCharacterDisplay);
        characterObj.transform.rotation = Quaternion.Euler(rotationCharDisplay);
        characterObj.transform.localScale = scaleCharDisplay;

        lastPrefabCharDisplay = characterObj;
    }
}
