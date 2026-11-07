using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private TargetFinder targetFinder;

    [Header("Target Settings")]
    [SerializeField] private float attackRange = 8.5f;
    [SerializeField] private float searchInterval = 0.1f;

    [Header("Attack Settings")]
    [SerializeField] private float fireInterval = 0.02f;
    [SerializeField] private float bulletDamage = 5f;
    [SerializeField] private float bulletSpeed = 12f;

    private Enemy currentTarget;

    private float searchTimer;
    private float fireTimer;

    private void Awake()
    {
        if (targetFinder == null)
        {
            targetFinder = GetComponent<TargetFinder>();
        }
    }

    private void Start()
    {
        if (gameManager == null ||
            spawnManager == null ||
            targetFinder == null)
        {
            Debug.LogError(
                "Tower 설정 오류: 필요한 참조가 연결되지 않았습니다.",
                this
            );

            enabled = false;
            return;
        }

        targetFinder.Initialize(gameManager);
    }

    private void Update()
    {
        // 지정한 주기마다 타깃을 다시 검색한다.
        searchTimer += Time.unscaledDeltaTime;

        if (searchTimer >= searchInterval)
        {
            searchTimer -= searchInterval;
            currentTarget = targetFinder.FindTarget(attackRange);
        }

        // 유효한 타깃이 없으면 공격하지 않는다.
        if (currentTarget == null || !currentTarget.IsAlive)
        {
            fireTimer = 0f;
            return;
        }

        // 공격 간격을 관리한다.
        fireTimer += Time.unscaledDeltaTime;

        if (fireTimer >= fireInterval)
        {
            fireTimer -= fireInterval;

            spawnManager.SpawnBullet(
                transform.position,
                currentTarget,
                bulletDamage,
                bulletSpeed
            );
        }
    }
}