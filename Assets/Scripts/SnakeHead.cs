using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float turnAccel;

    [Header("Snake")]
    [SerializeField] [Range(1, 10)] private int startLength;
    [SerializeField] private float followDelay;

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject segmentPrefab;

    private SnakeSegment lastSegment;
    private int snakeLength;

    private void Start()
    {
        snakeLength = 1;

        // setup head segment component
        lastSegment = GetComponent<SnakeSegment>();
        lastSegment.SetFollowDelay(followDelay);

        for (int i = 0; i < startLength - 1; i++)
        {
            AddSegment();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddSegment();
        }

        Vector3 playerDirection = -(transform.position - player.transform.position).normalized;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(playerDirection), turnSpeed * Time.deltaTime);

        //float angleToPlayer = Vector3.SignedAngle(transform.position, player.transform.position, Vector3.up);
        //float direction = Mathf.Sign(angleToPlayer);
        //transform.localEulerAngles += Vector3.up * turnSpeed * direction * Time.deltaTime;

        //transform.localEulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.localEulerAngles.y, angleToPlayer, turnSpeed * Time.deltaTime);

        //Debug.Log(angleToPlayer);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * moveSpeed * Time.deltaTime;
    }

    private void AddSegment()
    {
        SnakeSegment segment = Instantiate(segmentPrefab, transform.parent).GetComponent<SnakeSegment>();
        segment.SetSegmentParent(lastSegment);
        segment.SetFollowDelay(followDelay);
        lastSegment = segment;

        segment.transform.position = lastSegment.transform.position;
        segment.transform.rotation = lastSegment.transform.rotation;

        snakeLength++;
    }
}
