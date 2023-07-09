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
    [SerializeField] private float stopSpawnClearanceRadius;

    [Header("Pickups")]
    [SerializeField] private float coinChanceDestroyedProp;

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Transform world;
    [SerializeField] private GameObject[] stopPrefabs;
    [SerializeField] private LayerMask stopLayer;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private LayerMask stopSpawnCheckLayer;

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
                GameObject stopPrefab = stopPrefabs[Random.Range(0, stopPrefabs.Length)];
                Stop stop = Instantiate(stopPrefab, world).GetComponent<Stop>();
                Vector3 randomPosition = new Vector3(Random.Range(stopSpawnRangeX.x, stopSpawnRangeX.y), 0f, Random.Range(stopSpawnRangeZ.x, stopSpawnRangeZ.y));
                stop.transform.localPosition = randomPosition;
                while (Physics.CheckSphere(stop.transform.position, stopSpawnClearanceRadius, stopSpawnCheckLayer, QueryTriggerInteraction.Collide))
                {
                    randomPosition = new Vector3(Random.Range(stopSpawnRangeX.x, stopSpawnRangeX.y), 0f, Random.Range(stopSpawnRangeZ.x, stopSpawnRangeZ.y));
                    stop.transform.localPosition = randomPosition;
                }

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

    public void SnakeCollectStop(Stop stop)
    {
        stopList.Remove(stop);
        Destroy(stop.gameObject);
        stopCount--;
    }

    public void SnakeDestroyProp(Destroyable destroyedObject)
    {
        if (Random.Range(0f, 1f) <= coinChanceDestroyedProp)
        {
            Coin coin = Instantiate(coinPrefab, world).GetComponent<Coin>();
            coin.transform.localPosition = destroyedObject.transform.localPosition;
        }
        destroyedObject.DestroyProp();
    }
}
