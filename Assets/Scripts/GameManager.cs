using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Rounds")]
    [SerializeField] private int coinCostRoundAdd;
    [SerializeField] private int snakeLengthRoundAdd;
    [SerializeField] private int snakeSpeedRoundAdd;
    [SerializeField] private Vector3 snakeSpawnPosition;

    [Header("Stops")]
    [SerializeField] private int maxSpawnAttempts;
    [SerializeField] private int maxStops;
    [SerializeField] private Vector2 stopSpawnInterval;
    [SerializeField] private Vector2 stopSpawnRangeX;
    [SerializeField] private Vector2 stopSpawnRangeZ;
    [SerializeField] private float stopSpawnClearanceRadius;

    [Header("Props")]
    [SerializeField] private int maxProps;
    [SerializeField] private Vector2 propSpawnInterval;
    [SerializeField] private Vector2 propSpawnRangeX;
    [SerializeField] private Vector2 propSpawnRangeZ;
    [SerializeField] private float propSpawnClearanceRadius;

    [Header("Pickups")]
    [SerializeField] private float coinChanceDestroyedProp;
    [SerializeField] private int coinCost;

    [Header("Time")]
    [SerializeField] private float timeAccelTime;
    [SerializeField] private float gameOverTimeScale;
    [SerializeField] private float gameOverCamUpdateDelay;
    [SerializeField] private Vector2 trainRespawnDelay;

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform world;
    [SerializeField] private GameObject[] propPrefabs;
    [SerializeField] private GameObject[] stopPrefabs;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private LayerMask stopSpawnCheckLayer;
    [SerializeField] private GameObject passengerPrefab;
    [SerializeField] private GameObject playerPassengerPrefab;
    [SerializeField] private GameObject snakePrefab;
    [SerializeField] private GameObject tunnelPrefab;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text coinCostText;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private AudioSource coinSpawnSound;
    [SerializeField] private AudioSource propSpawnSound;
    [SerializeField] private AudioSource stopSpawnSound;
    [SerializeField] private AudioSource trainSpawnSound;
    [SerializeField] private AudioSource gameOverSound;
    [SerializeField] private AudioSource gameOverScreenSound;
    [SerializeField] private AudioSource music;

    private List<Stop> stopList = new List<Stop>();
    private int stopCount;
    private float stopSpawnTimer;

    private int propCount;
    private float propSpawnTimer;

    private SnakeHead trainHead;
    private Transform gameOverTrainTail;
    private float targetTimeScale;
    private float timeVelocity;
    private float gameOverCamDelayTimer;
    private bool gameOver;

    private int score;
    private int snakeStartLength;
    private int speedAdd;
    private bool finalSoundPlayed;

    private bool canHitTrain;
    private bool canTrainCollide;

    private void Start()
    {
        //Application.targetFrameRate = 60;

        propCount = 4;
        ResetStopSpawnTimer();
        ResetPropSpawnTimer();
        trainHead = GameObject.Find("Snake").transform.GetChild(0).GetComponent<SnakeHead>();

        Time.timeScale = 1f;
        targetTimeScale = 1f;
        score = 0;
        snakeStartLength = 3;
        speedAdd = 0;
        player.SetCoinCost(coinCost);
        trainSpawnSound.Play();
        music.Play();

        canTrainCollide = true;
        canHitTrain = true;
    }

    private void Update()
    {
        if (stopCount < maxStops)
        {
            // spawn stops
            stopSpawnTimer = Mathf.Max(stopSpawnTimer - Time.deltaTime, 0);
            if (stopSpawnTimer == 0)
            {
                int spawnAttempts = 1;
                GameObject stopPrefab = stopPrefabs[Random.Range(0, stopPrefabs.Length)];
                Stop stop = Instantiate(stopPrefab, world).GetComponent<Stop>();
                Vector3 randomPosition = new Vector3(Random.Range(stopSpawnRangeX.x, stopSpawnRangeX.y), 0f, Random.Range(stopSpawnRangeZ.x, stopSpawnRangeZ.y));
                stop.transform.localPosition = randomPosition;
                while (spawnAttempts <= maxSpawnAttempts && Physics.CheckSphere(stop.transform.position, stopSpawnClearanceRadius, stopSpawnCheckLayer, QueryTriggerInteraction.Collide))
                {
                    spawnAttempts++;
                    randomPosition = new Vector3(Random.Range(stopSpawnRangeX.x, stopSpawnRangeX.y), 0f, Random.Range(stopSpawnRangeZ.x, stopSpawnRangeZ.y));
                    stop.transform.localPosition = randomPosition;
                }

                if (spawnAttempts > maxSpawnAttempts)
                {
                    //Debug.Log("stop spawn failed");
                    Destroy(stop.gameObject);
                }
                else
                {
                    stopList.Add(stop);
                    stopCount++;
                    stopSpawnSound.Play();
                }
                ResetStopSpawnTimer();
            }
        }

        if (propCount < maxProps)
        {
            // spawn props
            propSpawnTimer = Mathf.Max(propSpawnTimer - Time.deltaTime, 0);
            if (propSpawnTimer == 0)
            {
                int spawnAttempts = 1;
                GameObject propPrefab = propPrefabs[Random.Range(0, propPrefabs.Length)];
                GameObject prop = Instantiate(propPrefab, world);
                Vector3 randomPosition = new Vector3(Random.Range(propSpawnRangeX.x, propSpawnRangeX.y), 0f, Random.Range(propSpawnRangeZ.x, propSpawnRangeZ.y));
                prop.transform.localPosition = Vector3Int.RoundToInt(randomPosition);
                while (spawnAttempts <= maxSpawnAttempts && Physics.CheckSphere(prop.transform.position, propSpawnClearanceRadius, stopSpawnCheckLayer, QueryTriggerInteraction.Collide))
                {
                    spawnAttempts++;
                    randomPosition = new Vector3(Random.Range(propSpawnRangeX.x, propSpawnRangeX.y), 0f, Random.Range(propSpawnRangeZ.x, propSpawnRangeZ.y));
                    prop.transform.localPosition = Vector3Int.RoundToInt(randomPosition);
                }

                if (spawnAttempts > maxSpawnAttempts)
                {
                    //Debug.Log("prop spawn failed");
                    Destroy(prop);
                }
                else
                {
                    float randomRotation = Random.Range(0, 4) * 90f;
                    prop.transform.localEulerAngles = randomRotation * Vector3.up;
                    propCount++;
                    propSpawnSound.Play();
                }
                ResetPropSpawnTimer();
            }
        }

        if (gameOver)
        {
            gameOverCamDelayTimer += Time.unscaledDeltaTime;
            if (gameOverCamDelayTimer > gameOverCamUpdateDelay)
            {
                cameraController.GameOverUpdate(gameOverTrainTail);
                if (gameOverCamDelayTimer > gameOverCamUpdateDelay * 3)
                {
                    gamePanel.SetActive(false);
                    endPanel.SetActive(true);
                    finalScoreText.text = score + "";
                    if (!finalSoundPlayed)
                    {
                        gameOverScreenSound.Play();
                        finalSoundPlayed = true;
                        music.Stop();
                    }
                    if (Input.GetButtonDown("Jump"))
                    {
                        SceneManager.LoadSceneAsync(1);
                    }
                }
            }
        }

        scoreText.text = "" + score;
        coinCostText.text = "" + coinCost;
    }

    private void FixedUpdate()
    {
        Time.timeScale = Mathf.SmoothDamp(Time.timeScale, targetTimeScale, ref timeVelocity, timeAccelTime);
    }

    private void ResetStopSpawnTimer()
    {
        stopSpawnTimer = Random.Range(stopSpawnInterval.x, stopSpawnInterval.y);
    }

    private void ResetPropSpawnTimer()
    {
        propSpawnTimer = Random.Range(propSpawnInterval.x, propSpawnInterval.y);
    }

    public void SnakeCollectStop(Stop stop)
    {
        stopList.Remove(stop);
        Destroy(stop.gameObject);
        stopCount--;

        PassengerAnimate passenger = Instantiate(passengerPrefab, stop.transform.position, Quaternion.identity).GetComponent<PassengerAnimate>();
        passenger.trainHead = trainHead.GetHeadModel();
    }

    public void SnakeDestroyProp(Destroyable destroyedObject)
    {
        if (Random.Range(0f, 1f) <= coinChanceDestroyedProp)
        {
            Coin coin = Instantiate(coinPrefab, world).GetComponent<Coin>();
            coin.transform.position = new Vector3(destroyedObject.transform.position.x, 0f, destroyedObject.transform.position.z);
            coinSpawnSound.Play();
        }
        destroyedObject.DestroyProp();
    }
    
    public void PropDestroyed()
    {
        propCount--;
    }

    public void GameOver()
    {
        gameOverSound.Play();
        gameOver = true;
        gameOverTrainTail = trainHead.GameOver();
        cameraController.GameOver(trainHead.GetHeadModel());
        PassengerAnimate deadPlayer = Instantiate(playerPassengerPrefab, player.transform.position, Quaternion.identity).GetComponent<PassengerAnimate>();
        deadPlayer.trainHead = trainHead.GetHeadModel();
        targetTimeScale = gameOverTimeScale;
    }

    public void TrainCrashed(int length)
    {
        if (canTrainCollide)
        {
            canTrainCollide = false;
            canHitTrain = false;
            score += Mathf.FloorToInt((length * length) / 3);
            coinCost += coinCostRoundAdd;
            snakeStartLength += snakeLengthRoundAdd;
            speedAdd += snakeSpeedRoundAdd;
            player.SetCoinCost(coinCost);
            StartCoroutine(RespawnTrain());
        }
    }

    private IEnumerator RespawnTrain()
    {
        yield return new WaitForSeconds(Random.Range(trainRespawnDelay.x, trainRespawnDelay.y));
        SnakeHead head = Instantiate(snakePrefab, snakeSpawnPosition, Quaternion.identity).transform.GetChild(0).GetComponent<SnakeHead>();
        head.Setup(player, this, snakeStartLength, speedAdd);
        trainHead = head;
        trainSpawnSound.Play();
        Instantiate(tunnelPrefab, Vector3.zero, Quaternion.identity);
        canTrainCollide = true;
        canHitTrain = true;
    }

    public bool CanPlayerHitTrain()
    {
        return canHitTrain;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopSpawnClearanceRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, propSpawnClearanceRadius);
    }
}
