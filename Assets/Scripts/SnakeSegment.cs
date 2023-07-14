using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    public GameObject model;
    [SerializeField] private Collider hitbox;
    [SerializeField] private float spawnAnimateTime;

    private SnakeSegment segmentParent;
    private float followDelay;
    private int fixedUpdateSaveInterval;
    private float bounceAnimationTime;
    private float bounceHeight;
    private TrailRenderer[] trails;
    private List<Guide> guideList = new List<Guide>();
    private int fixedUpdateCount;
    private float animateTimer;
    private float spawnAnimateTimer;
    private bool paused;

    public class Guide
    {
        public Vector3 position;
        public Quaternion rotation;

        public Guide(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    private void Start()
    {
        if (IsSegmentParentNotNull())
        {
            hitbox.enabled = false;
            model.transform.localPosition = new Vector3(model.transform.localPosition.x, -2.5f, model.transform.localPosition.z);
        }

        paused = false;
        animateTimer = bounceAnimationTime;
    }

    private void Update()
    {
        // spawn animation
        if (IsSegmentParentNotNull() && spawnAnimateTimer < spawnAnimateTime)
        {
            spawnAnimateTimer = Mathf.Clamp(spawnAnimateTimer + Time.deltaTime, 0, spawnAnimateTime);
            float animateProgress = Mathf.SmoothStep(0, 1, spawnAnimateTimer / spawnAnimateTime);
            model.transform.localPosition = new Vector3(model.transform.localPosition.x, Utility.QuadraticBezier(-2.5f, bounceHeight, 0, animateProgress), model.transform.localPosition.z);
        }
        else if (IsSegmentParentNotNull() && !hitbox.enabled)
        {
            hitbox.enabled = true;
            foreach(TrailRenderer trail in trails)
            {
                trail.emitting = true;
            }
        }

        if (IsSegmentParentNotNull() && !paused)
        {
            transform.position = segmentParent.GetGuidePosition();
            transform.rotation = segmentParent.GetGuideRotation();
        }

        if (animateTimer < bounceAnimationTime && spawnAnimateTimer == spawnAnimateTime)
        {
            animateTimer = Mathf.Clamp(animateTimer + Time.deltaTime, 0, bounceAnimationTime);
            float animateProgress = Mathf.SmoothStep(0, 1, animateTimer / bounceAnimationTime);
            model.transform.localPosition = new Vector3(model.transform.localPosition.x, Utility.QuadraticBezier(0, bounceHeight, 0, animateProgress), model.transform.localPosition.z);
        }
    }

    private void FixedUpdate()
    {
        fixedUpdateCount++;
        // every 2 "frames"
        if (fixedUpdateCount % fixedUpdateSaveInterval == 0 && !paused)
        {
            guideList.Insert(0, new Guide(transform.position, transform.rotation));
            if (guideList.Count > Mathf.FloorToInt(followDelay * (60 / fixedUpdateSaveInterval)))
            {
                guideList.RemoveAt(guideList.Count - 1);
            }
        }  
    }

    public Vector3 GetGuidePosition()
    {
        return guideList[guideList.Count - 1].position;
    }

    public Quaternion GetGuideRotation()
    {
        return guideList[guideList.Count - 1].rotation;
    }

    public void SetSegmentParent(SnakeSegment parent)
    {
        segmentParent = parent;
    }

    public void SetFollowDelay(float delay)
    {
        followDelay = delay;
    }

    public void SetSaveInterval(int interval)
    {
        fixedUpdateSaveInterval = interval;
    }

    public void SetBounceAnimationTime(float duration)
    {
        bounceAnimationTime = duration;
    }

    public void SetBounceHeight(float height)
    {
        bounceHeight = height;
    }

    public void SetTrail(TrailRenderer[] trailArray)
    {
        trails = trailArray;
    }

    public bool IsSegmentParentNotNull()
    {
        return segmentParent != null;
    }

    public bool CanSelfCollide()
    {
        return spawnAnimateTimer == spawnAnimateTime;
    }

    public void AnimateBounce()
    {
        animateTimer = 0f;
    }

    public void Pause(bool pause)
    {
        paused = pause;
    }
}
