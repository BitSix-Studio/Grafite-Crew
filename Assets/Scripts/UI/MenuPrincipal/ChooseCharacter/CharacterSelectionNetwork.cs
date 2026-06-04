using Fusion;
using UnityEngine;

public class CharacterSelectionNetwork : NetworkBehaviour
{
    public static CharacterSelectionNetwork Instance;

    public override void Spawned()
    {
        Instance = this;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SendCharacterChoice(int characterId, RpcInfo info = default)
    {
        NetworkManager.Instance.RegisterCharacter(
            info.Source,
            characterId
        );

        Debug.Log(
            $"Player {info.Source} escolheu {characterId}"
        );
    }
}