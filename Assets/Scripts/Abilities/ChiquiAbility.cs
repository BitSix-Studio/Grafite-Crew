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

        RPC_RequestShowEnemyEffect(duration);

        Debug.Log("CHIQUI ABILITY ACTIVATE");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestShowEnemyEffect(float duration)
    {
        PlayerRef enemy = abilityController.GetEnemyPlayer();

        if (enemy == PlayerRef.None)
            return;

        RPC_ShowEffect(enemy, duration);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RPC_ShowEffect([RpcTarget] PlayerRef target, float duration)
    {
        ChiquiSprayAbilityUI.Instance.ShowEffect(duration);
    }
}
