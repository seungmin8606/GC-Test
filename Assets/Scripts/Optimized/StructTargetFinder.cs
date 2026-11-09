using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructTargetFinder : TargetFinder
{
    [Header("Struct Optimization - Test3")]
    [SerializeField] private int initialCapacity = 64;

    // Test2와 동일하게 리스트는 재사용한다.
    // 단, 저장하는 데이터 타입만 Struct로 변경한다.
    private List<TargetInfoStruct> candidates;

    private void Awake()
    {
        // Test2와 동일한 초기 용량 사용
        candidates = new List<TargetInfoStruct>(
            Mathf.Max(0, initialCapacity)
        );
    }

    public override Enemy FindTarget(float range)
    {
        if (gameManager == null)
        {
            return null;
        }

        // Collection Reuse 유지
        candidates.Clear();

        IReadOnlyList<Enemy> activeEnemies =
            gameManager.ActiveEnemies;

        float rangeSquared = range * range;

        // 기존과 동일한 타깃 검색 로직
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
                // Test3 핵심: Struct 데이터 생성
                TargetInfoStruct info =
                    new TargetInfoStruct(
                        enemy,
                        distanceSquared
                    );

                candidates.Add(info);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        // 기존과 동일하게 가장 가까운 적 선택
        TargetInfoStruct closest = candidates[0];

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