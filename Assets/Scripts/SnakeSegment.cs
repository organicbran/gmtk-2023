using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    private SnakeSegment segmentParent;
    private float followDelay;
    private List<Guide> guideList = new List<Guide>();

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

    private void Update()
    {
        if (segmentParent != null)
        {
            transform.position = segmentParent.GetGuidePosition();
            transform.rotation = segmentParent.GetGuideRotation();
        }
    }

    private void FixedUpdate()
    {
        guideList.Insert(0, new Guide(transform.position, transform.rotation));
        if (guideList.Count > Mathf.RoundToInt(followDelay * 60))
        {
            guideList.RemoveAt(guideList.Count - 1);
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
}
