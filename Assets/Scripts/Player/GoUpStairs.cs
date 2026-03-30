using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoUpStairs : MonoBehaviour
{
    private bool canUpStairs, isOnGround;
    PlayerController player;
    public float targetPositionFloor;
    public CameraFocus cam;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        isOnGround = false;
        canUpStairs = false;
    }

    private void Update()
    {
        if (canUpStairs && isOnGround)
        {
            if (Input.GetKeyDown(KeybindingManager.Instance.keyUp))
            {
                StartCoroutine(UpStairs());
            }
        }
    }

    IEnumerator UpStairs()
    {
        player.canMove = false;
        player.rig.velocity = Vector3.zero;

        Vector3 startPos = player.rig.position;
        Vector3 targetPos = startPos + Vector3.up * targetPositionFloor;

        Vector3 startCamPos = cam.transform.position;
        Vector3 targetCamPos = startCamPos + Vector3.up * targetPositionFloor;

        float duration = 1f;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            player.rig.MovePosition(newPos);

            Vector3 camPos = Vector3.Lerp(startCamPos, targetCamPos, t);
            cam.transform.position = camPos;

            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        player.rig.MovePosition(targetPos);
        cam.transform.position = targetCamPos;

        yield return new WaitForSeconds(player.delayMovement);
        player.canMove = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Stairs"))
        {
            canUpStairs = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Stairs"))
        {
            canUpStairs = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isOnGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isOnGround = false;
        }
    }
}
