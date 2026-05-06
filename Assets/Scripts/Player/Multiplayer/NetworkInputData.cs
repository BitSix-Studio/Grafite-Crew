using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector3 targetDirection;
    public NetworkButtons buttons;
}

public enum InputButtons
{
    Jump = 0
}
