using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnimate : MonoBehaviour
{
    [SerializeField] private float animationTime;
    [SerializeField] private float start;
    [SerializeField] private float height;
    private float timer;

    private void Update()
    {
        if (timer < animationTime)
        {
            timer = Mathf.Clamp(timer + Time.deltaTime, 0, animationTime);
            float animateProgress = Mathf.SmoothStep(0, 1, timer / animationTime);
            transform.position = new Vector3(transform.position.x, Utility.QuadraticBezier(start, height, 0, animateProgress), transform.position.z);
        }
        else
        {
            Destroy(this);
        }
    }
}
