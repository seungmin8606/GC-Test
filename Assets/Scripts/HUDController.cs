using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text killsText;

    [Header("Optimization")]
    [SerializeField] private bool optimizeUIText = false;

    private int displayedSeconds = -1;
    private int displayedScore = -1;
    private int displayedKills = -1;

    private void Start()
    {
        if (gameManager == null ||
            timeText == null ||
            scoreText == null ||
            killsText == null)
        {
            Debug.LogError(
                "HUDController 설정 오류: GameManager 또는 UI 참조가 없습니다.",
                this
            );

            enabled = false;
        }
    }

    private void Update()
    {
        int currentSeconds = gameManager.ElapsedSeconds;
        int currentScore = gameManager.Score;
        int currentKills = gameManager.KillCount;

        // 게임 시간은 초가 변경될 때만 갱신한다.
        if (currentSeconds != displayedSeconds)
        {
            UpdateTimeText(currentSeconds);
            displayedSeconds = currentSeconds;
        }

        // 점수는 값이 변경될 때만 갱신한다.
        if (currentScore != displayedScore)
        {
            UpdateScoreText(currentScore);
            displayedScore = currentScore;
        }

        // 처치 수는 값이 변경될 때만 갱신한다.
        if (currentKills != displayedKills)
        {
            UpdateKillsText(currentKills);
            displayedKills = currentKills;
        }
    }

    private void UpdateTimeText(int seconds)
    {
        timeText.text = "Time: " + seconds.ToString();
    }

    private void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    private void UpdateKillsText(int kills)
    {
        killsText.text = "Kills: " + kills.ToString();
    }
}