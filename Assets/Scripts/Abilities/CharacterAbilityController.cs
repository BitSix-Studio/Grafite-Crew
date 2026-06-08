using Fusion;
using UnityEngine;

public class CharacterAbilityController : NetworkBehaviour
{
    [Networked] public TickTimer Cooldown { get; set; }

    private PlayerControllerMultiplayer player;

    private Character characterData;

    [Networked] public int CharacterId { get; set; }

    private bool initialized;
    private bool hasSpawned;
    public bool HasSpawned()
    {
        return hasSpawned;
    }

    public override void Spawned()
    {
        player = GetComponent<PlayerControllerMultiplayer>();

        hasSpawned = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (initialized)
            return;

        if (CharacterId < 0)
            return;

        characterData = NetworkManager.Instance.GetCharacterData(CharacterId);

        initialized = true;

        if (Object.HasInputAuthority)
        {
            AbilityButtonUI.Instance.SetPlayer(this);
        }
    }

    public Sprite GetAbilityIcon()
    {
        return characterData.ability.icon;
    }

    public float GetCooldownDuration()
    {
        if (characterData == null)
            return 0;

        return characterData.ability.cooldown;
    }

    public float GetRemainingCooldown()
    {
        if (!hasSpawned)
            return 0;

        if (Runner == null)
            return 0;

        if (Cooldown.ExpiredOrNotRunning(Runner))
            return 0;

        return Cooldown.RemainingTime(Runner) ?? 0;
    }

    public void TryUseAbility()
    {
        if (!HasInputAuthority)
            return;

        RPC_RequestUseAbility();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestUseAbility()
    {
        Debug.Log($"RPC_RequestUseAbility -> CharacterId={CharacterId} | characterData={(characterData != null)}");

        if (characterData == null)
            return;

        if (!Cooldown.ExpiredOrNotRunning(Runner))
            return;

        characterData.ability.Execute(player);

        Cooldown = TickTimer.CreateFromSeconds(
            Runner,
            characterData.ability.cooldown
        );
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Object.HasInputAuthority &&
            AbilityButtonUI.Instance != null)
        {
            AbilityButtonUI.Instance.ClearPlayer();
        }
    }
}