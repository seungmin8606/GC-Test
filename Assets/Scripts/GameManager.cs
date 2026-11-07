using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int scorePerKill = 10;

    // 현재 활성화된 적 목록
    private readonly List<Enemy> activeEnemies = new List<Enemy>();

    // 게임 상태
    private float elapsedTime;
    private int score;
    private int killCount;

    public IReadOnlyList<Enemy> ActiveEnemies => activeEnemies;
    public float ElapsedTime => elapsedTime;
    public int ElapsedSeconds => Mathf.FloorToInt(elapsedTime);
    public int Score => score;
    public int KillCount => killCount;

    private void Awake()
    {
        activeEnemies.Clear();
        elapsedTime = 0f;
        score = 0;
        killCount = 0;
    }

    private void Update()
    {
        // 실험에서 게임 경과 시간을 측정할 때 사용한다.
        elapsedTime += Time.unscaledDeltaTime;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public void NotifyEnemyKilled(Enemy enemy)
    {
        if (ReferenceEquals(enemy, null))
        {
            return;
        }

        // 활성 목록에 있던 적을 처치한 경우에만 점수를 반영한다.
        if (!activeEnemies.Remove(enemy))
        {
            return;
        }

        killCount++;
        score += scorePerKill;
    }

    public void NotifyEnemyEscaped(Enemy enemy)
    {
        UnregisterEnemy(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (ReferenceEquals(enemy, null))
        {
            return;
        }

        activeEnemies.Remove(enemy);
    }
}