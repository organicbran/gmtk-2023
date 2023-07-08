using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float smoothTime;
    [SerializeField] private Transform followTransform;

    private Vector3 startOffset;
    private Vector3 cameraVelocity;

    private void Start()
    {
        startOffset = transform.position;
    }

    private void Update()
    {
        Vector3 targetPosition = new Vector3(followTransform.position.x + startOffset.x, startOffset.y, followTransform.position.z + startOffset.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref cameraVelocity, smoothTime);
    }
}
