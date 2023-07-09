using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerAnimate : MonoBehaviour
{
    [HideInInspector] public Transform trainHead;
    [SerializeField] private float animationTime;
    [SerializeField] private float height;
    private float timer;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (timer < animationTime)
        {
            timer = Mathf.Clamp(timer + Time.deltaTime, 0, animationTime);
            float animateProgress = Mathf.SmoothStep(0, 1, timer / animationTime);
            transform.localScale = (1f - (animateProgress / 3)) * Vector3.one;
            transform.position = Utility.QuadraticBezier(startPos, new Vector3((startPos.x + trainHead.position.x) / 2, height, (startPos.z + trainHead.position.z) / 2), trainHead.position, animateProgress);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
