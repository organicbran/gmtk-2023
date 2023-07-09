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

    [Header("Machine")]
    [SerializeField] private int machineCoinCost;
    [SerializeField] private float machineInteractRadius;
    
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider hitbox;
    [SerializeField] private LayerMask machineLayer;
    [SerializeField] private TMP_Text coinCountText;
    [SerializeField] private GameObject modelObject;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem[] jumpReadyParticles;

    private Vector2 inputDir;
    private Vector2 moveDir;
    private int coins;
    private bool canJump;

    private void Start()
    {
        Physics.gravity = Vector3.up * gravity;
        coins = 0;
        canJump = false;

        coinCountText.text = coins + " COINS";

        foreach (ParticleSystem particle in jumpReadyParticles)
        {
            particle.Stop();
        }
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

        if (Input.GetButtonDown("Jump"))
        {
            // jumping
            if (canJump)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                canJump = false;

                animator.SetTrigger("Jump");
                foreach (ParticleSystem particle in jumpReadyParticles)
                {
                    particle.Stop();
                }
            }
            // detect machine
            else if (Physics.CheckSphere(transform.position, machineInteractRadius, machineLayer) && coins >= machineCoinCost)
            {
                coins -= machineCoinCost;
                canJump = true;
                coinCountText.text = coins + " COINS";

                foreach (ParticleSystem particle in jumpReadyParticles)
                {
                    particle.Play();
                }
            }
        }

        // player model
        if (inputDir != Vector2.zero)
        {
            modelObject.transform.localEulerAngles = Vector3.up * -1 * Vector2.SignedAngle(Vector2.up, inputDir);
        }
        animator.SetBool("Running", inputDir != Vector2.zero);
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
