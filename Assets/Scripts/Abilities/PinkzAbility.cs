using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Pinkzzibzib")]
public class PinkzAbility : AbilityData
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
