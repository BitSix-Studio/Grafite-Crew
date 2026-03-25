using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerController : MonoBehaviour
{
    public float speed, jumpForce, gravityMultiplier;
    public Vector3 direction, jumpDirection;

    public bool canJump;
    public Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        canJump = false;
    }

    private void Update()
    {
        InputVerify();
    }

    void InputVerify()
    {
        if (Input.GetKey(KeybindingManager.Instance.keyLeft))
        {
            direction = Vector3.left;
        }
        else if (Input.GetKey(KeybindingManager.Instance.keyRight))
        {
            direction = Vector3.right;
        }
        
        if (Input.GetKey(KeybindingManager.Instance.keyUp))
        {
            JumpPlayer();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer(direction);

        if (!canJump)
        {
            rig.AddForce(Vector3.up * Physics.gravity.y * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    void MovePlayer(Vector3 dir)
    {
        rig.MovePosition(rig.position + dir * speed * Time.fixedDeltaTime);
    }

    void JumpPlayer()
    {
        if (canJump)
        {
            rig.velocity = new Vector3(rig.velocity.x, 0, rig.velocity.z);

            jumpDirection = Vector3.up * jumpForce;
            rig.AddForce(jumpDirection, ForceMode.Impulse);
            canJump = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            canJump = true;
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
