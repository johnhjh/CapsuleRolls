using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager gameMgr;
    public static GameManager Instance
    {
        get
        {
            if (gameMgr == null)
                gameMgr = FindObjectOfType<GameManager>();
            return gameMgr;
        }
    }

    public bool IsGameOver { get; private set; }
    private int teamScoreA = 0;
    private int teamScoreB = 0;

    private void Awake()
    {
        if (gameMgr == null)
            gameMgr = this;
        else if (gameMgr != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        // 플레이어 생성 위치 들어갈 자리

    }

    private void Update()
    {
        // 게임 종료 들어갈 자리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    public void AddScore(bool isTeamA, int newScore)
    {
        if (!IsGameOver)
        {
            if (isTeamA)
                teamScoreA += newScore;
            else
                teamScoreB += newScore;
        }
    }

    public void EndGame()
    {
        IsGameOver = true;
    }

}
