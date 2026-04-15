using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDirection : MonoBehaviour
{
    private Vector2 startTouch;
    private Vector2 endTouch;

    private float minDistance = Screen.height * 0.05f;

    public enum Swipe
    {
        None,
        Left,
        Right,
        Up,
        Down
    }

    public System.Action<Swipe> swipeAction;

    private void Update()
    {
        if (!PlatformHelper.IsMobile()) return;

        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            startTouch = touch.position;
        }

        if (touch.phase == TouchPhase.Ended)
        {
            endTouch = touch.position;
            DetectSwipe();
        }
    }

    public void DetectSwipe()
    {
        Vector2 direction = endTouch - startTouch;

        if (direction.magnitude < minDistance)
            return;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal movement swipe
            if (direction.x > 0)
                SwipeRight();
            else
                SwipeLeft();
        }
        else
        {
            // Vertical movement swipe
            if (direction.y > 0)
                SwipeUp();
            else
                SwipeDown();
        }
    }

    void SwipeRight() => swipeAction?.Invoke(Swipe.Right);
    void SwipeLeft() => swipeAction?.Invoke(Swipe.Left);
    void SwipeUp() => swipeAction?.Invoke(Swipe.Up);
    void SwipeDown() => swipeAction?.Invoke(Swipe.Down);
}
