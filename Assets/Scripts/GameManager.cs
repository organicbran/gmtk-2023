using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Stops")]
    [SerializeField] private int maxStops;
    [SerializeField] private Vector2 stopSpawnInterval;
    [SerializeField] private Vector2 stopSpawnRangeX;
    [SerializeField] private Vector2 stopSpawnRangeZ;

    [Header("Coins")]

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Transform world;
    [SerializeField] private GameObject stopPrefab;
    [SerializeField] private LayerMask stopLayer;

    private int fixedUpdateCount;

    private List<Stop> stopList = new List<Stop>();
    private int stopCount;
    private float stopSpawnTimer;

    private void Start()
    {
        ResetStopSpawnTimer();
    }

    private void Update()
    {
        if (stopCount < maxStops)
        {
            // spawn stops
            stopSpawnTimer = Mathf.Max(stopSpawnTimer - Time.deltaTime, 0);
            if (stopSpawnTimer == 0)
            {
                Vector3 randomPosition = new Vector3(Random.Range(stopSpawnRangeX.x, stopSpawnRangeX.y), 0f, Random.Range(stopSpawnRangeZ.x, stopSpawnRangeZ.y));
                Stop stop = Instantiate(stopPrefab, world).GetComponent<Stop>();
                stop.transform.localPosition = randomPosition;

                stopList.Add(stop);
                stopCount++;
                ResetStopSpawnTimer();
            }
        }
    }

    private void ResetStopSpawnTimer()
    {
        stopSpawnTimer = Random.Range(stopSpawnInterval.x, stopSpawnInterval.y);
    }

    private void FixedUpdate()
    {
        fixedUpdateCount++;
        // every 20 "frames"
        if (fixedUpdateCount % 20 == 0)
        {

        }
    }

    public void SnakeCollectStop(Stop stop)
    {
        stopList.Remove(stop);
        Destroy(stop.gameObject);
        stopCount--;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
    }
}
