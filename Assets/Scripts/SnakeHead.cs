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
    [SerializeField] private Vector3 playerStartTrackingPosition;

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
    [SerializeField] private ParticleSystem propParticles;
    [SerializeField] private GameObject explodeParticles;
    [SerializeField] private float explodeInterval;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private Player player;
    [SerializeField] private GameManager manager;
    [SerializeField] private GameObject trailObject;
    [SerializeField] private AudioSource propSound;
    [SerializeField] private AudioSource stopSound;
    [SerializeField] private AudioSource crashSound;

    private List<SnakeSegment> segmentList = new List<SnakeSegment>();
    private Transform target;
    private float targetRotationY;
    private float rotationVelocity;
    private float currentSpeed;
    private float pauseTimer;
    private float animateIntervalTimer;
    private TrailRenderer[] trails;
    private Transform startingTarget;

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

        target = GameObject.Find("Manager").transform;
        startingTarget = target;

        trails = trailObject.GetComponentsInChildren<TrailRenderer>();
        propParticles.Stop(true);
    }

    private void Update()
    {
        if (target == startingTarget && (transform.position.x <= playerStartTrackingPosition.x || transform.position.z <= playerStartTrackingPosition.z))
        {
            target = player.transform;
        }

        if (pauseTimer > 0)
        {
            pauseTimer = Mathf.Max(pauseTimer - Time.deltaTime, 0);
            if (pauseTimer == 0)
            {
                PauseSnake(false);
            } 
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
        // moved from Update, fixes train not turning when under 60 fps issue
        if (target != null)
        {
            Vector3 targetDirection = -(transform.position - target.position).normalized;
            targetRotationY = Quaternion.LookRotation(targetDirection).eulerAngles.y;
            if (pauseTimer == 0)
            {
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotationY, ref rotationVelocity, turnSmoothTime);
            }

            float playerDistance = Vector3.Distance(transform.position, player.transform.position);
            playerDistance = Mathf.Clamp(playerDistance, playerCloseDistance, playerFarDistance);
            playerDistance = (playerDistance - playerCloseDistance) / (playerFarDistance - playerCloseDistance);
            float moveSpeed = moveSpeedMin + playerDistance * (moveSpeedMax - moveSpeedMin);
            currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, accel * Time.deltaTime);
        }

        if (pauseTimer == 0 && target != null)
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
        if (target != null && other.gameObject.TryGetComponent(out SnakeSegment segment) && segment.CanSelfCollide())
        {
            StartCoroutine(DestroySnake());
            PauseSnake(true);
            pauseTimer = Mathf.Infinity;
            manager.TrainCrashed(segmentList.Count);
            crashSound.Play();
        }
        else if (other.gameObject.TryGetComponent(out Stop stop))
        {
            manager.SnakeCollectStop(stop);
            AddSegment();

            PauseSnake(true);
            pauseTimer = stopPauseLength;
            stopSound.Play();
        }
        else if (other.gameObject.TryGetComponent(out Destroyable destroyObject))
        {
            manager.SnakeDestroyProp(destroyObject);

            PauseSnake(true);
            pauseTimer = crashPauseLength;

            propParticles.Play(true);
            propSound.Play();
        }
    }

    private IEnumerator DestroySnake()
    {
        for (int i = segmentList.Count - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(explodeInterval);

            Instantiate(explodeParticles, segmentList[i].transform.position, Quaternion.identity);
            Destroy(segmentList[i].gameObject);
            if (i == 0)
            {
                Destroy(transform.parent.gameObject);
            }
        }
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
        foreach (SnakeSegment segment in segmentList)
        {
            segment.Pause(pause);
        }
    }

    public Transform GetHeadModel()
    {
        return segmentList[0].model.transform;
    }

    public Transform GameOver()
    {
        target = null;
        PauseSnake(true);
        pauseTimer = Mathf.Infinity;
        AddSegment();
        return segmentList[segmentList.Count - 1].transform;
    }

    public void Setup(Player player, GameManager manager, int length, int speedAdd)
    {
        this.player = player;
        this.manager = manager;
        this.startLength = length;
        moveSpeedMin += speedAdd;
        moveSpeedMax += speedAdd;
    }
}
