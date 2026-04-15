using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static SwipeDirection;

public class InputVerify : MonoBehaviour
{
    PlayerController player;
    SwipeDirection swipeDirection;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        swipeDirection = GetComponent<SwipeDirection>();
    }

    private void Update()
    {
        VerifyPlatformInput();
    }

    // Checks if the player pressed the key or swipe on mobile screen
    void VerifyPlatformInput()
    {
        if (PlatformHelper.IsMobile())
        {
            swipeDirection.swipeAction += ChangeDirectionSwipe;
        }
        else
        {
            if (Input.GetKey(KeybindingManager.Instance.keyLeft))
            {
                player.targetDirection = Vector3.left;
            }
            else if (Input.GetKey(KeybindingManager.Instance.keyRight))
            {
                player.targetDirection = Vector3.right;
            }

            if (Input.GetKeyDown(KeybindingManager.Instance.keyUp))
            {
                player.JumpPlayer();
            }
            else if (Input.GetKeyDown(KeybindingManager.Instance.keyDown))
            {
                StartCoroutine(player.SlidePlayer());
            }
        }
    }

    void ChangeDirectionSwipe(Swipe swipe)
    {
        switch (swipe)
        {
            case Swipe.Left:
                player.targetDirection = Vector3.left;
                break;

            case Swipe.Right:
                player.targetDirection = Vector3.right;
                break;

            case Swipe.Up:
                player.JumpPlayer();
                break;

            case Swipe.Down:
                if (player.canSlide)
                    StartCoroutine(player.SlidePlayer());
                break;
        }
    }

    private void OnDestroy()
    {
        swipeDirection.swipeAction -= ChangeDirectionSwipe;
    }
}
