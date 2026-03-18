using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public bool canMove;

    public Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (canMove) {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        if (Input.GetKeyDown(KeybindingManager.Instance.keyLeft))
        {
            float rightDir = 1;
            direction = new Vector3(rightDir, 0f, 0f);

            rig.MovePosition(direction * speed * Time.deltaTime);
        }
    }
}
