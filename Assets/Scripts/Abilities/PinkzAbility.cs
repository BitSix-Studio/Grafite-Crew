using Fusion;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Pinkzzibzib")]
public class PinkzAbility : AbilityData
{
    public float gravityForceJump;

    public override void Execute(PlayerControllerMultiplayer player)
    {
        player.networkController.gravity = gravityForceJump;

        player.AbilityActive = true;

        player.AbilityEffectTimer = TickTimer.CreateFromSeconds(player.Runner, duration);

        Debug.Log("PINKZZ'IB'ZIB ABILITY ACTIVATE");
    }
}
