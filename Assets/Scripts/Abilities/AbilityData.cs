using UnityEngine;

public abstract class AbilityData : ScriptableObject
{
    public Sprite icon;
    public float cooldown;
    public float duration;

    public abstract void Execute(PlayerControllerMultiplayer player);
}