using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance;

    public Vector3 MoveDirection { get; private set; }

    public bool JumpPressed { get; private set; }
    public bool SlidePressed { get; private set; }

    private SwipeDirection swipe;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        swipe = GetComponent<SwipeDirection>();

        swipe.swipeAction += OnSwipe;
    }

    private void OnSwipe(SwipeDirection.Swipe swipeDir)
    {
        switch (swipeDir)
        {
            case SwipeDirection.Swipe.Left:
                MoveDirection = Vector3.left;
                break;

            case SwipeDirection.Swipe.Right:
                MoveDirection = Vector3.right;
                break;

            case SwipeDirection.Swipe.Up:
                JumpPressed = true;
                break;

            //case SwipeDirection.Swipe.Down:
            //    SlidePressed = true;
            //    break;
        }
    }

    public void ResetButtons()
    {
        JumpPressed = false;
        SlidePressed = false;
    }

    private void OnDestroy()
    {
        if (swipe != null)
            swipe.swipeAction -= OnSwipe;
    }
}
