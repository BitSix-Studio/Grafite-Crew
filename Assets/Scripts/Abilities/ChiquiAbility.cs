using Fusion;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Chiqui")]
public class ChiquiAbility : AbilityData
{
    private CharacterAbilityController abilityController;

    public override void Execute(PlayerControllerMultiplayer player)
    {
        abilityController = player.GetComponent<CharacterAbilityController>();

        player.AbilityActive = true;

        abilityController.ShowSprayOnEnemy(player.Object.InputAuthority, duration);

        Debug.Log("CHIQUI ABILITY ACTIVATE");
    }
}
