using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    private GameManager gameManager;

    public void Initialize(GameManager manager)
    {
        gameManager = manager;
    }

    public Enemy FindTarget(float range)
    {
        if (gameManager == null)
        {
            return null;
        }

        // Baseline: 검색할 때마다 새 리스트를 생성한다.
        List<TargetInfoClass> candidates =
            new List<TargetInfoClass>();

        IReadOnlyList<Enemy> activeEnemies =
            gameManager.ActiveEnemies;

        float rangeSquared = range * range;

        // 사거리 안에 있는 적을 후보 리스트에 등록한다.
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
                // Baseline: 후보 적마다 class 객체를 새로 생성한다.
                TargetInfoClass info =
                    new TargetInfoClass(enemy, distanceSquared);

                candidates.Add(info);
            }
        }

        // 후보가 없다면 타깃을 찾지 못한 것이다.
        if (candidates.Count == 0)
        {
            return null;
        }

        // 가장 가까운 적을 선택한다.
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