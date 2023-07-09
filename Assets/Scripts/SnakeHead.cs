using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeedMin;
    [SerializeField] private float moveSpeedMax;
    [SerializeField] private float playerCloseDistance;
    [SerializeField] private float playerFarDistance;
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private float accel;

    [Header("Snake")]
    [SerializeField] [Range(1, 10)] private int startLength;
    [SerializeField] private float initialSpawnInterval;
    [SerializeField] private float followDelay;
    [SerializeField] private int fixedUpdateSaveInterval;
    [SerializeField] private float stopPauseLength;
    [SerializeField] private float crashPauseLength;

    [Header("Animation")]
    [SerializeField] private float bounceAnimationLength;
    [SerializeField] private float bounceInterval;
    [SerializeField] private float bounceSegmentDelay;
    [SerializeField] private float bounceHeight;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private Player player;
    [SerializeField] private GameManager manager;
    [SerializeField] private GameObject trailObject;

    private List<SnakeSegment> segmentList = new List<SnakeSegment>();
    private Transform target;
    private float targetRotationY;
    private float rotationVelocity;
    private float currentSpeed;
    private float pauseTimer;
    private float animateIntervalTimer;
    private TrailRenderer[] trails;

    private void Start()
    {
        // setup head segment component
        segmentList.Add(GetComponent<SnakeSegment>());
        segmentList[0].SetFollowDelay(followDelay);
        segmentList[0].SetSaveInterval(fixedUpdateSaveInterval);

        segmentList[0].SetBounceAnimationTime(bounceAnimationLength);
        segmentList[0].SetBounceHeight(bounceHeight);
        segmentList[0].SetTrail(trails);

        StartCoroutine(InitialSpawn());

        target = player.transform;

        trails = trailObject.GetComponentsInChildren<TrailRenderer>();
    }

    private void Update()
    {
        Vector3 targetDirection = -(transform.position - target.position).normalized;
        targetRotationY = Quaternion.LookRotation(targetDirection).eulerAngles.y;
        if (pauseTimer == 0)
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotationY, ref rotationVelocity, turnSmoothTime);

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        playerDistance = Mathf.Clamp(playerDistance, playerCloseDistance, playerFarDistance);
        playerDistance = (playerDistance - playerCloseDistance) / (playerFarDistance - playerCloseDistance);
        float moveSpeed = moveSpeedMin + playerDistance * (moveSpeedMax - moveSpeedMin);
        currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, accel * Time.deltaTime);

        if (pauseTimer > 0)
        {
            pauseTimer = Mathf.Max(pauseTimer - Time.deltaTime, 0);
            if (pauseTimer == 0)
                PauseSnake(false);
        }

        // animation
        animateIntervalTimer = Mathf.Max(animateIntervalTimer - Time.deltaTime, 0);
        if (animateIntervalTimer == 0)
        {
            StartCoroutine(AnimateBounce());
        }
    }

    private void FixedUpdate()
    {
        if (pauseTimer == 0)
        {
            rb.velocity = transform.forward * currentSpeed * Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void AddSegment()
    {
        SnakeSegment segment = Instantiate(segmentPrefab, transform.parent).GetComponent<SnakeSegment>();
        segment.SetSegmentParent(segmentList[segmentList.Count - 1]);
        segment.SetFollowDelay(followDelay);
        segment.SetSaveInterval(fixedUpdateSaveInterval);

        segment.SetBounceAnimationTime(bounceAnimationLength);
        segment.SetBounceHeight(bounceHeight);
        segment.SetTrail(trails);

        segmentList.Add(segment);

        trailObject.transform.parent = segment.transform;
        trailObject.transform.localPosition = Vector3.zero;
        trailObject.transform.localRotation = Quaternion.identity;
        foreach (TrailRenderer trail in trails)
        {
            trail.Clear();
            trail.emitting = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out SnakeSegment segment) && segment.CanSelfCollide())
        {
            DestroySnake();
        }
        else if (other.gameObject.TryGetComponent(out Stop stop))
        {
            manager.SnakeCollectStop(stop);
            AddSegment();

            PauseSnake(true);
            pauseTimer = stopPauseLength;
        }
        else if (other.gameObject.TryGetComponent(out Destroyable destroyObject))
        {
            manager.SnakeDestroyProp(destroyObject);

            PauseSnake(true);
            pauseTimer = crashPauseLength;
        }
    }

    private void DestroySnake()
    {
        Destroy(transform.parent.gameObject);
    }

    private IEnumerator AnimateBounce()
    {
        for (int i = 0; i < segmentList.Count; i++)
        {
            segmentList[i].AnimateBounce();
            if (i == segmentList.Count - 1)
                animateIntervalTimer = bounceInterval;
            yield return new WaitForSeconds(bounceSegmentDelay);
        }
    }

    private IEnumerator InitialSpawn()
    {
        for (int i = 0; i < startLength - 1; i++)
        {
            yield return new WaitForSeconds(initialSpawnInterval);
            AddSegment();
        }
    }

    private void PauseSnake(bool pause)
    {
        /*
        if (pause)
        {
            animateIntervalTimer = 0;
        }
        */
            
        foreach (SnakeSegment segment in segmentList)
        {
            segment.Pause(pause);
        }
    }
}
