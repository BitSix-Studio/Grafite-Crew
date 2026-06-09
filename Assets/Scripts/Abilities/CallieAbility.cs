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

        RPC_RequestStun(duration);

        player.AbilityActive = true;

        player.AbilityEffectTimer = TickTimer.CreateFromSeconds(player.Runner, duration);

        Debug.Log("CALLIE ABILITY ACTIVATE");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestStun(float duration)
    {
        PlayerRef enemyPlayer = abilityController.GetEnemyPlayer();

        if (enemyPlayer == PlayerRef.None)
            return;

        if (!NetworkManager.Instance.spawnedCharacters.TryGetValue(enemyPlayer, out NetworkObject enemyObject))
            return;

        PlayerControllerMultiplayer enemy = enemyObject.GetComponent<PlayerControllerMultiplayer>();

        RPC_StunEffect(enemy, duration);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RPC_StunEffect([RpcTarget] PlayerControllerMultiplayer target, float duration)
    {
        target.IsStunned = true;

        target.StunTimer = TickTimer.CreateFromSeconds(target.Runner, duration);
    }
}
