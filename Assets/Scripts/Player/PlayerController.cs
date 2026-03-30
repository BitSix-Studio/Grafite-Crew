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
    private Vector3 currentDirection, targetDirection, jumpDirection, slideDirection;
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

    private void Update()
    {
        InputVerify();
    }

    // Checks if the player pressed the key
    void InputVerify()
    {
        if (Input.GetKey(KeybindingManager.Instance.keyLeft))
        {
            targetDirection = Vector3.left;
        }
        else if (Input.GetKey(KeybindingManager.Instance.keyRight))
        {
            targetDirection = Vector3.right;
        }
        
        if (Input.GetKeyDown(KeybindingManager.Instance.keyUp))
        {
            JumpPlayer();
        }
        else if (Input.GetKeyDown(KeybindingManager.Instance.keyDown))
        {
            StartCoroutine(SlidePlayer());
        }
    }

    private void FixedUpdate()
    {
        // Interpolação suave da direção
        currentDirection = Vector3.Lerp(currentDirection, targetDirection, directionChangeSpeed * Time.fixedDeltaTime);

        // Always moves right or left - game mechanics
        MovePlayer(currentDirection);

        // If it's in the air, add gravity to make it fall further.
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

    void JumpPlayer()
    {
        if (canJump)
        {
            canJump = false;
            // It reduces movement speed to normalize the jump.
            rig.velocity = new Vector3(rig.velocity.x, 0, rig.velocity.z); 

            jumpDirection = Vector3.up * jumpForce;
            rig.AddForce(jumpDirection, ForceMode.Impulse);
        }
    }

    IEnumerator SlidePlayer()
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

                // Interpolação de rotação (vai e volta)
                Quaternion currentRotation = Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    Mathf.Sin(t * Mathf.PI) // vai até 90° e volta
                );

                rig.MoveRotation(currentRotation);

                // Mantém movimento no chão
                rig.velocity = new Vector3(rig.velocity.x, 0, rig.velocity.z);

                // Aplica força contínua (melhor que Impulse dentro de loop)
                slideDirection = currentDirection * slideForce;
                rig.AddForce(slideDirection, ForceMode.Acceleration);

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // Reset final
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
