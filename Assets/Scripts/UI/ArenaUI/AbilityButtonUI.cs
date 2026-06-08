using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonUI : MonoBehaviour
{
    public static AbilityButtonUI Instance;

    [SerializeField] private Image abilityIcon;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private TextMeshProUGUI cooldownText;

    private CharacterAbilityController playerAbility;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayer(CharacterAbilityController ability)
    {
        playerAbility = ability;

        abilityIcon.sprite = ability.GetAbilityIcon();
        cooldownFill.sprite = ability.GetAbilityIcon();
    }

    public void OnClick()
    {
        if (playerAbility == null)
            return;

        playerAbility.TryUseAbility();
    }

    private void Update()
    {
        if (playerAbility == null)
            return;

        if (!playerAbility.HasSpawned())
            return;

        UpdateCooldown();
    }

    private void UpdateCooldown()
    {
        float remaining =
            playerAbility.GetRemainingCooldown();

        float total =
            playerAbility.GetCooldownDuration();

        if (remaining <= 0)
        {
            cooldownFill.fillAmount = 0;
            cooldownText.text = "";
            return;
        }

        cooldownFill.fillAmount =
            remaining / total;

        cooldownText.text =
            Mathf.CeilToInt(remaining).ToString();
    }

    public void ClearPlayer()
    {
        playerAbility = null;
    }
}