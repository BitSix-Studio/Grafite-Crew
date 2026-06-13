using Fusion;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/LeoRush")]
public class LeoRushAbility : AbilityData
{
    public float speedIncrease;

    public override void Execute(PlayerControllerMultiplayer player)
    {
        player.networkController.maxSpeed = speedIncrease;

        player.AbilityActive = true;

        player.AbilityEffectTimer = TickTimer.CreateFromSeconds(player.Runner, duration);

        Debug.Log("LEORUSH ABILITY ACTIVATE");
    }
}
