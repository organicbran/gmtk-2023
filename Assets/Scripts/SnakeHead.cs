using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private float accel;

    [Header("Snake")]
    [SerializeField] [Range(1, 10)] private int startLength;
    [SerializeField] private float followDelay;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private Player player;
    [SerializeField] private GameManager manager;

    private List<SnakeSegment> segmentList = new List<SnakeSegment>();
    private int snakeLength;
    private Transform target;
    private float targetRotationY;
    private float rotationVelocity;
    private float currentSpeed;

    private void Start()
    {
        snakeLength = 1;
        // setup head segment component
        segmentList.Add(GetComponent<SnakeSegment>());
        segmentList[0].SetFollowDelay(followDelay);

        for (int i = 0; i < startLength - 1; i++)
        {
            AddSegment();
        }

        target = player.transform;
    }

    private void Update()
    {
        Vector3 targetDirection = -(transform.position - target.position).normalized;
        targetRotationY = Quaternion.LookRotation(targetDirection).eulerAngles.y;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotationY, ref rotationVelocity, turnSmoothTime);
        currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, accel * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * currentSpeed * Time.deltaTime;
    }

    private void AddSegment()
    {
        SnakeSegment segment = Instantiate(segmentPrefab, transform.parent).GetComponent<SnakeSegment>();
        segment.SetSegmentParent(segmentList[segmentList.Count - 1]);
        segment.SetFollowDelay(followDelay);
        segmentList.Add(segment);

        segment.transform.position = segmentList[segmentList.Count - 1].transform.position;
        segment.transform.rotation = segmentList[segmentList.Count - 1].transform.rotation;

        snakeLength++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Stop>(out Stop stop))
        {
            manager.SnakeCollectStop(stop);
            AddSegment();
        }
        else if (other.gameObject.TryGetComponent<Destroyable>(out Destroyable destroyObject))
        {
            destroyObject.DestroyProp();
        }
    }
}
