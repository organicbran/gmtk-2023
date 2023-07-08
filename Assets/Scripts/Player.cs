using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accel;
    [SerializeField] private float turnAccel;
    [SerializeField] private float decel;

    [Header("References")]
    [SerializeField] private Rigidbody rb;

    private Vector2 inputDir;
    private Vector2 moveDir;

    private void Update()
    {
        inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (!inputDir.Equals(Vector2.zero))
        {
            if (!inputDir.Equals(moveDir.normalized))
            {
                // turning
                moveDir = Vector2.MoveTowards(moveDir, inputDir * moveSpeed, turnAccel * Time.deltaTime);
            }
            else
            {
                // accelerating
                moveDir = Vector2.MoveTowards(moveDir, inputDir * moveSpeed, accel * Time.deltaTime);
            }
        }
        else
        {
            // decelerating
            moveDir = Vector2.MoveTowards(moveDir, Vector2.zero, decel * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDir.x * Time.deltaTime, 0, moveDir.y * Time.deltaTime);
    }
}
