using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    // 자식 클래스에서도 사용할 수 있도록 protected로 변경
    protected GameManager gameManager;

    public void Initialize(GameManager manager)
    {
        gameManager = manager;
    }

    // 자식 클래스에서 재정의할 수 있도록 virtual 추가
    public virtual Enemy FindTarget(float range)
    {
        if (gameManager == null)
        {
            return null;
        }

        // Test0, Test1: 매 검색마다 새로운 리스트 생성
        List<TargetInfoClass> candidates =
            new List<TargetInfoClass>();

        IReadOnlyList<Enemy> activeEnemies =
            gameManager.ActiveEnemies;

        float rangeSquared = range * range;

        // 사거리 안의 적 탐색
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            Enemy enemy = activeEnemies[i];

            if (enemy == null || !enemy.IsAlive)
            {
                continue;
            }

            Vector3 difference =
                enemy.transform.position - transform.position;

            float distanceSquared = difference.sqrMagnitude;

            if (distanceSquared <= rangeSquared)
            {
                // Test3에서 최적화할 Class 객체 생성
                TargetInfoClass info =
                    new TargetInfoClass(enemy, distanceSquared);

                candidates.Add(info);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        // 가장 가까운 적 선택
        TargetInfoClass closest = candidates[0];

        for (int i = 1; i < candidates.Count; i++)
        {
            if (candidates[i].DistanceSquared <
                closest.DistanceSquared)
            {
                closest = candidates[i];
            }
        }

        return closest.Enemy;
    }
}