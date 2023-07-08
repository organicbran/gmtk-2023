using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accel;
    [SerializeField] private float turnAccel;
    [SerializeField] private float decel;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideEndSpeed;
    [SerializeField] private float slideCooldown;

    [Header("Machine")]
    [SerializeField] private int machineCoinCost;
    [SerializeField] private float machineInteractRadius;
    
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider hitbox;
    [SerializeField] private LayerMask machineLayer;
    [SerializeField] private TMP_Text coinCountText;

    private Vector2 inputDir;
    private Vector2 moveDir;
    private int coins;
    private int jumps;
    private bool sliding;
    private float slideCooldownTimer;

    private void Start()
    {
        Physics.gravity = Vector3.up * gravity;
        coins = 0;
        jumps = 0;
        slideCooldownTimer = 0;

        coinCountText.text = coins + " COINS";
    }

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

        // sliding
        slideCooldownTimer = Mathf.Max(slideCooldownTimer - Time.deltaTime, 0);

        if (Input.GetButtonDown("Slide") && slideCooldownTimer == 0)
        {
            moveDir = inputDir * slideSpeed;
            hitbox.height = 1;
            transform.position -= Vector3.up * 0.5f;
            slideCooldownTimer = slideCooldown;
            sliding = true;
        }

        if (sliding && moveDir.magnitude < slideEndSpeed)
        {
            sliding = false;
            hitbox.height = 2;
        }

        if (Input.GetButtonDown("Jump"))
        {
            // jumping
            if (jumps > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                jumps--;
            }
            // detect machine
            else if (Physics.CheckSphere(transform.position, machineInteractRadius, machineLayer) && coins >= machineCoinCost)
            {
                coins -= machineCoinCost;
                jumps++;
                coinCountText.text = coins + " COINS";
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDir.x * Time.deltaTime, rb.velocity.y, moveDir.y * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out SnakeSegment segment))
        {
            SnakeCollision();
        }
        else if (other.gameObject.TryGetComponent(out Coin coin))
        {
            coin.Collect();
            coins++;
            coinCountText.text = coins + " COINS";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out SnakeSegment segment))
        {
            SnakeCollision();
        }
    }

    private void SnakeCollision()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, machineInteractRadius);
    }
}
