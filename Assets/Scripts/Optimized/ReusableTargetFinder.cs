using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReusableTargetFinder : TargetFinder
{
    [Header("Collection Reuse - Test2")]
    [SerializeField] private int initialCapacity = 64;

    // 검색할 때마다 생성하지 않고 계속 재사용
    private List<TargetInfoClass> candidates;

    private void Awake()
    {
        // 리스트는 한 번만 생성
        candidates = new List<TargetInfoClass>(
            Mathf.Max(0, initialCapacity)
        );
    }

    public override Enemy FindTarget(float range)
    {
        if (gameManager == null)
        {
            return null;
        }

        // Test2 핵심: 기존 리스트를 비우고 재사용
        candidates.Clear();

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
                // 아직 Class 객체 생성은 유지
                TargetInfoClass info =
                    new TargetInfoClass(enemy, distanceSquared);

                candidates.Add(info);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        // 기존과 동일하게 가장 가까운 적 선택
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