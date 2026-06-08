using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Cervidae")]
public class CervidaeAbility : AbilityData
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
