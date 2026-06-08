using UnityEngine;

public abstract class AbilityData : ScriptableObject
{
    public Sprite icon;
    public float cooldown;

    public abstract void Execute(PlayerControllerMultiplayer player);
}