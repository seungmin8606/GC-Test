using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptimizedHUDController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text killsText;

    // 이전에 출력한 값 저장
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
                "OptimizedHUDController: UI 참조가 연결되지 않았습니다.",
                this
            );

            enabled = false;
            return;
        }

        // 최초 표시값은 Update에서 갱신
        // 불필요하게 추가 갱신하지 않는다.
    }

    private void Update()
    {
        int currentSeconds = gameManager.ElapsedSeconds;
        int currentScore = gameManager.Score;
        int currentKills = gameManager.KillCount;

        // 게임 시간: 초가 변경될 때만 갱신
        if (currentSeconds != displayedSeconds)
        {
            UpdateTimeText(currentSeconds);
            displayedSeconds = currentSeconds;
        }

        // 점수: 값이 변경될 때만 갱신
        if (currentScore != displayedScore)
        {
            UpdateScoreText(currentScore);
            displayedScore = currentScore;
        }

        // 처치 수: 값이 변경될 때만 갱신
        if (currentKills != displayedKills)
        {
            UpdateKillsText(currentKills);
            displayedKills = currentKills;
        }
    }

    private void UpdateTimeText(int seconds)
    {
        timeText.SetText("Time: {0}", (float)seconds);
    }

    private void UpdateScoreText(int score)
    {
        scoreText.SetText("Score: {0}", (float)score);
    }

    private void UpdateKillsText(int kills)
    {
        killsText.SetText("Kills: {0}", (float)kills);
    }
}