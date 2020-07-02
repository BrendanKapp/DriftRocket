using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Vector3 spawnpoint = Vector3.zero;

    [SerializeField]
    private float horizontalSpawnDistanceMin, verticalSpawnDistanceMin;
    public float HorizontalSpawnDistanceMin
    {
        get
        {
            return horizontalSpawnDistanceMin;
        }
    }
    public float VerticalSpawnDistanceMin
    {
        get
        {
            return verticalSpawnDistanceMin;
        }
    }

    private float gameStartTime;
    public float GetGameStartTime ()
    {
        return gameStartTime;
    }
    private float gameStopTime;

    public static GameController instance;

    public delegate void StartSpawnAction();
    public static event StartSpawnAction StartSpawnEvent;
    public delegate void StopSpawnAction();
    public static event StopSpawnAction StopSpawnEvent;
    public delegate void SetAction(bool isActive);
    public static event SetAction SetEvent;

    public delegate void ResetAction();
    public static event ResetAction ResetEvent;

    public static PlayerController playerController;

    private AudioSource music;

    private SaveFloat highScore;
    public float GetHighScore ()
    {
        return highScore.value;
    }

    private void Awake()
    {
        highScore = new SaveFloat("iHighScore");
        instance = this;
    }
    private void Start()
    {
        music = ObjectPooler.PoolObject("Sounds", "Music").GetComponent<AudioSource>();
    }
    public void GameStart()
    {
        RocketController player = ObjectPooler.PoolObject("Player").GetComponent<RocketController>();
        playerController = player.GetComponent<PlayerController>();
        player.gameObject.SetActive(true);
        player.transform.position = spawnpoint;
        player.SetActive(true);
        music.gameObject.SetActive(true);
        music.Play();
        StartSpawnEvent();
        gameStartTime = Time.time;
        CameraFollow.instance.SetTarget(player.transform);
        CameraFollow.instance.SetActive(true);
    }
    public void GameOver()
    {
        gameStopTime = Time.time;
        Time.timeScale = 0.1f;
        SetHighScore();
        UIManager.instance.GameOver(gameStopTime - gameStartTime);
        ResetEvent();
        StopSpawnEvent();
        SetEvent(false);
    }
    private void SetHighScore ()
    {
        float score = gameStopTime - gameStartTime;
        if (highScore.value < score)
        {
            highScore.value = score;
            //new high score!
            UIManager.instance.NewHighScore(highScore.value);
        }
        UIManager.instance.SetHighScoreText();
    }
}
