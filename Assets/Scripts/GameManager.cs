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
    [SerializeField] private float stopActivateRadius;

    [Header("References")]
    [SerializeField] private Player player;
    //[SerializeField] private SnakeHead snake;
    [SerializeField] private Transform world;
    [SerializeField] private GameObject stopPrefab;
    [SerializeField] private LayerMask stopLayer;

    private int fixedUpdateCount;

    private List<Stop> stopList = new List<Stop>();
    private List<Stop> activeStopList = new List<Stop>();
    //private List<Transform> snakeTargetList = new List<Transform>();
    private int stopCount;
    private float stopSpawnTimer;

    private void Start()
    {
        ResetStopSpawnTimer();
        ActivateStops();

        //snake.SetTarget(player.transform);
        //SnakeRetarget();
    }

    private void Update()
    {
        if (stopCount < maxStops)
        {
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
            ActivateStops();
            //SnakeRetarget();
        }
    }

    private void ActivateStops()
    {
        foreach (Stop stop in stopList)
        {
            stop.Activate(false);
        }

        // check which stops are near the player
        activeStopList.Clear();
        Collider[] hitStops = Physics.OverlapSphere(player.transform.position, stopActivateRadius, stopLayer);
        foreach (Collider hitStop in hitStops)
        {
            Stop stop = hitStop.GetComponent<Stop>();
            stop.Activate(true);
            activeStopList.Add(stop);
        }
    }

    /*
    private void SnakeRetarget()
    {
        // give snake its target
        snakeTargetList.Clear();
        snakeTargetList.Add(player.transform);
        foreach (Stop stop in activeStopList)
        {
            snakeTargetList.Add(stop.transform);
        }

        int targetIndex = 0;
        float shortestDistance = Vector3.Distance(snake.transform.position, snakeTargetList[0].position);
        for (int i = 1; i < snakeTargetList.Count; i++)
        {
            if (Vector3.Distance(snake.transform.position, snakeTargetList[i].position) < shortestDistance)
            {
                targetIndex = i;
            }
        }
        snake.SetTarget(snakeTargetList[targetIndex]);
    }
    */

    public void SnakeCollectStop(Stop stop)
    {
        activeStopList.Remove(stop);
        stopList.Remove(stop);
        Destroy(stop.gameObject);
        stopCount--;
        //SnakeRetarget();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(player.transform.position, stopActivateRadius);
    }
}
