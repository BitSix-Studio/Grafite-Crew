using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CorridorController : NetworkBehaviour
{
    public NetworkObject reference;
    public float offset = 10f;

    public GameObject wallRight, wallLeft, elevatorRight, elevatorLeft;
    public static int left = 1, right = 0;

    [Networked] public NetworkBool IsRightSide { get; set; }

    [Header("Background")]
    public Renderer backgroundRenderer;
    public Texture2D[] randomBackgrounds;

    NetworkObject objNetwork;

    public override void Spawned()
    {
        objNetwork = GetComponent<NetworkObject>();
        AlternateElevator();
        RandomizeBackground();
    }

    public void Init(NetworkObject playerRef)
    {
        reference = playerRef;
    }

    public override void FixedUpdateNetwork()
    {
        if (reference == null)
            return;

        if (transform.position.y < reference.transform.position.y - offset)
        {
            Runner.Despawn(objNetwork);
        }
    }

    public void SetSide(bool right)
    {
        IsRightSide = right;

        AlternateElevator();
    }

    public void AlternateElevator()
    {
        if (IsRightSide) // ELEVADOR DIREITA
        {
            elevatorLeft.SetActive(false);
            wallRight.SetActive(false);

            elevatorRight.SetActive(true);
            wallLeft.SetActive(true);
        }
        else // ELEVADOR ESQUERDA
        {
            elevatorLeft.SetActive(true);
            wallRight.SetActive(true);

            elevatorRight.SetActive(false);
            wallLeft.SetActive(false);
        }
    }

    private void RandomizeBackground()
    {
        if (randomBackgrounds.Length == 0)
            return;

        int randomIndex = Random.Range(0, randomBackgrounds.Length);

        Texture2D selectedTexture = randomBackgrounds[randomIndex];

        backgroundRenderer.material.SetTexture("_BaseMap", selectedTexture);
    }
}
