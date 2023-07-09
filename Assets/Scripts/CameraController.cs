using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float smoothTime;
    [SerializeField] private Transform followTransform;
    [SerializeField] private Transform camChild;
    [SerializeField] private Vector2 camLimitX;
    [SerializeField] private Vector2 camLimitZ;
    [SerializeField] private float startZoom;
    [SerializeField] private float zoomInDistance;
    [SerializeField] private float zoomInTime;

    private Vector3 cameraVelocity;
    private float zoomVelocity;
    private float currentZoom;
    private float zoomTarget;

    private void Start()
    {
        zoomTarget = camChild.localPosition.z;
        currentZoom = startZoom;
    }

    private void Update()
    {
        if (followTransform != null)
        {
            Vector3 targetPosition = followTransform.position;
            if (targetPosition.x > camLimitX.x)
                targetPosition = new Vector3(camLimitX.x, targetPosition.y, targetPosition.z);
            else if (targetPosition.x < camLimitX.y)
                targetPosition = new Vector3(camLimitX.y, targetPosition.y, targetPosition.z);
            if (targetPosition.z > camLimitZ.x)
                targetPosition = new Vector3(targetPosition.x, targetPosition.y, camLimitZ.x);
            else if (targetPosition.z < camLimitZ.y)
                targetPosition = new Vector3(targetPosition.x, targetPosition.y, camLimitZ.y);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref cameraVelocity, smoothTime);

            camChild.localPosition = new Vector3(0, 0, currentZoom);
            currentZoom = Mathf.SmoothDamp(currentZoom, zoomTarget, ref zoomVelocity, zoomInTime);
        }
    }

    public void GameOver(Transform trainHead)
    {
        followTransform = trainHead;
        zoomTarget = zoomInDistance;
    }

    public void GameOverUpdate(Transform trainTail)
    {
        followTransform = trainTail;
    }
}
