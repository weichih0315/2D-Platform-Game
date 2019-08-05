using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int money = 0;
    public int score = 0;

    [SerializeField]
    private Player player;
    public Vector2 currentCheckPoint;

    public static GameManager instance;

    public static event System.Action OnGameOverStatic;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        PlayerSpawn();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameUI.instance.GameOver();
        }
    }

    public void AddMoney(int num)
    {
        money += num;
        GameUI.instance.UpdateMoney();
    }

    public void AddScore(int num)
    {
        score += num;
        GameUI.instance.UpdateScore();
    }

    public void GameOver()
    {
        if (OnGameOverStatic != null)
            OnGameOverStatic();
    }

    public void SetCheckPoint(Vector2 pos)
    {
        currentCheckPoint = pos;
    }

    public void PlayerSpawn()
    {
        Player target = Instantiate(player, currentCheckPoint, Quaternion.identity);
        CameraManager.instance.InitialFollowTarget(target.transform);
    }
}