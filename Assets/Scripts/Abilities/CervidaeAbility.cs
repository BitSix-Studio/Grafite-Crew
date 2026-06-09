using Fusion;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Cervidae")]
public class CervidaeAbility : AbilityData
{
    public override void Execute(PlayerControllerMultiplayer player)
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), true);

        player.AbilityActive = true;

        player.AbilityEffectTimer = TickTimer.CreateFromSeconds(player.Runner, duration);

        Debug.Log("CERVIDAE ABILITY ACTIVATE");
    }
}
