using Fusion;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "Abilities/Callie")]
public class CallieAbility : AbilityData
{
    private CharacterAbilityController abilityController;

    public override void Execute(PlayerControllerMultiplayer player)
    {
        abilityController = player.GetComponent<CharacterAbilityController>();
        
        player.AbilityActive = true;

        player.AbilityEffectTimer = TickTimer.CreateFromSeconds(player.Runner, duration);

        abilityController.ApplyStunOnEnemy(player.Object.InputAuthority, duration);

        Debug.Log("CALLIE ABILITY ACTIVATE");
    }
}
