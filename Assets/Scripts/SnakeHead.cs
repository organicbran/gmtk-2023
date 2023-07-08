using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float turnSmoothTime;

    [Header("Snake")]
    [SerializeField] [Range(1, 10)] private int startLength;
    [SerializeField] private float followDelay;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private Player player;
    [SerializeField] private GameManager manager;

    private SnakeSegment lastSegment;
    private int snakeLength;
    private Transform target;
    private float targetRotationY;
    private float rotationVelocity;

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

        target = player.transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddSegment();
        }

        Vector3 targetDirection = -(transform.position - target.position).normalized;
        targetRotationY = (Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), turnSpeed * Time.deltaTime)).eulerAngles.y;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotationY, ref rotationVelocity, turnSmoothTime);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Stop>(out Stop stop))
        {
            if (stop.activated)
            {
                manager.SnakeCollectStop(stop);
                AddSegment();
            }
        }
    }
}
