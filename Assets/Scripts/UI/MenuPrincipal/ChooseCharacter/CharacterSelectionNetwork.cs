using Fusion;
using UnityEngine;

public class CharacterSelectionNetwork : NetworkBehaviour
{
    public static CharacterSelectionNetwork Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SendCharacterChoice(int characterId, RpcInfo info = default)
    {
        NetworkManager.Instance.RegisterCharacter(
            info.Source,
            characterId
        );
    }
}