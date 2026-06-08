using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Chiqui")]
public class ChiquiAbility : AbilityData
{
    public float gravityForceJump;

    public override void Execute(PlayerControllerMultiplayer player)
    {
        Vector3 direction =
            player.transform.right;

        player.networkController.Move(
            direction * gravityForceJump
        );
    }
}
