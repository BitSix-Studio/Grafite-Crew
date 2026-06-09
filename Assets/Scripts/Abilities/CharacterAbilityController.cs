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

    #region Gets
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

    public PlayerRef GetEnemyPlayer(PlayerRef sourcePlayer)
    {
        foreach (var pair in NetworkManager.Instance.spawnedCharacters)
        {
            if (pair.Key != sourcePlayer)
                return pair.Key;
        }

        return PlayerRef.None;
    }
    #endregion

    public void TryUseAbility()
    {
        if (!HasInputAuthority)
            return;

        RPC_RequestUseAbility();
    }

    // REQUEST USE ABILITY FOR ALL CHARACTERS
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

    // CHIQUI ABILITY
    public void ShowSprayOnEnemy(PlayerRef sourcePlayer, float duration)
    {
        PlayerRef enemy = GetEnemyPlayer(sourcePlayer);

        if (enemy == PlayerRef.None)
            return;

        RPC_ShowSpray(enemy, duration);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowSpray(PlayerRef target, float duration)
    {
        if (Runner.LocalPlayer != target)
            return;

        Debug.Log($"Local={Runner.LocalPlayer} Target={target}");

        ChiquiSprayAbilityUI.Instance.ShowEffect(duration);
    }

    // CALLIE ABILITY
    public void ApplyStunOnEnemy(PlayerRef sourcePlayer, float duration)
    {
        PlayerRef enemyPlayer = GetEnemyPlayer(sourcePlayer);

        if (enemyPlayer == PlayerRef.None)
            return;

        if (!NetworkManager.Instance.spawnedCharacters.TryGetValue(enemyPlayer, out NetworkObject enemyObject))
            return;

        PlayerControllerMultiplayer enemy = enemyObject.GetComponent<PlayerControllerMultiplayer>();

        enemy.IsStunned = true;
        enemy.StunTimer = TickTimer.CreateFromSeconds(Runner, duration);
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