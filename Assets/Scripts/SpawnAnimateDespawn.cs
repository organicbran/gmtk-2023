using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnimateDespawn : MonoBehaviour
{
    [SerializeField] private int animationSegments;
    [SerializeField] private float animationTime;
    [SerializeField] private float start;
    [SerializeField] private float height;
    private float timer;

    private void Update()
    {
        if (timer < animationTime)
        {
            timer = Mathf.Clamp(timer + Time.deltaTime, 0, animationTime);
            float animateProgress = Mathf.SmoothStep(0, animationSegments, timer / animationTime);
            if (animateProgress < 1)
            {
                transform.position = new Vector3(transform.position.x, Utility.QuadraticBezier(start, height, 0, animateProgress), transform.position.z);
            }
            else if (animateProgress > animationSegments - 1)
            {
                transform.position = new Vector3(transform.position.x, Utility.QuadraticBezier(0, height, start, animateProgress - (animationSegments- 1)), transform.position.z);
            }
        }
        else
        {
            Destroy(this);
        }
    }
}
