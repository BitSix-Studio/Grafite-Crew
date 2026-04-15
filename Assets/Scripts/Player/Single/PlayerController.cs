using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerController : MonoBehaviour
{
    [Header("Move Config")]
    public float speed;
    public float jumpForce, gravityMultiplier, slideForce, timeSlide, delayMovement;
    [HideInInspector] public Vector3 currentDirection, targetDirection;
    private Vector3 jumpDirection, slideDirection;
    public float directionChangeSpeed;

    // slide demonstrate - (after remove)
    private const float rotatePlayer = 90f;

    [HideInInspector] public bool canMove, canJump, canSlide;
    [HideInInspector] public Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        canJump = false;
        canSlide = true;
        canMove = true;
    }

    private void FixedUpdate()
    {
        // Interpolação suave da direção
        currentDirection = Vector3.Lerp(currentDirection, targetDirection, directionChangeSpeed * Time.fixedDeltaTime);

        // Always moves right or left - game mechanics
        MovePlayer(currentDirection);

        // If it's in the air, add gravity to make it fall further
        if (!canJump)
        {
            rig.AddForce(Vector3.up * Physics.gravity.y * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    void MovePlayer(Vector3 dir)
    {
        if (canMove) 
        {
            rig.MovePosition(rig.position + dir * speed * Time.fixedDeltaTime);
        }
    }

    public void JumpPlayer()
    {
        if (canJump)
        {
            canJump = false;
            // It reduces movement speed to normalize the jump
            Vector3 vel = rig.velocity;
            vel.y = 0f;
            rig.velocity = vel;

            jumpDirection = Vector3.up * jumpForce;
            rig.AddForce(jumpDirection, ForceMode.Impulse);
        }
    }

    public IEnumerator SlidePlayer()
    {
        if (canJump && canSlide)
        {
            canSlide = false;
            float time = 0f;

            Quaternion startRotation = rig.rotation;
            Quaternion targetRotation = startRotation * Quaternion.Euler(0f, 0f, rotatePlayer);

            while (time <= timeSlide)
            {
                float t = time / timeSlide;

                // (It goes back and forth to the example - then it will be changed by the animation)
                Quaternion currentRotation = Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    Mathf.Sin(t * Mathf.PI) // It goes up to 90° and back
                );

                rig.MoveRotation(currentRotation);

                // Keep moving on the ground
                Vector3 vel = rig.velocity;
                vel.y = 0f;
                rig.velocity = vel;

                // Applies continuous force to give a slide boost
                slideDirection = currentDirection * slideForce;
                rig.AddForce(slideDirection, ForceMode.Acceleration);

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // Final reset
            rig.MoveRotation(startRotation);
            
        }
    }

    IEnumerator ResetMove()
    {
        yield return new WaitForSeconds(delayMovement);
        canSlide = true;
        canJump = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            StartCoroutine(ResetMove());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            canJump = false;
        }
    }
}
