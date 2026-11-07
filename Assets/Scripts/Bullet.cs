using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float hitRadius = 0.15f;
    [SerializeField] private float maxLifetime = 2f;

    private Enemy target;
    private SpawnManager spawnManager;

    private float damage;
    private float moveSpeed;
    private float elapsedLifetime;

    private bool isInitialized;

    public void Initialize(
        Enemy targetEnemy,
        float bulletDamage,
        float bulletSpeed,
        SpawnManager manager)
    {
        target = targetEnemy;
        damage = bulletDamage;
        moveSpeed = bulletSpeed;
        spawnManager = manager;

        elapsedLifetime = 0f;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized)
        {
            return;
        }

        elapsedLifetime += Time.unscaledDeltaTime;

        // 수명이 끝난 총알은 제거한다.
        if (elapsedLifetime >= maxLifetime)
        {
            Despawn();
            return;
        }

        // 타깃이 없거나 이미 처치됐다면 총알을 제거한다.
        if (target == null || !target.IsAlive)
        {
            Despawn();
            return;
        }

        Vector3 targetPosition = target.transform.position;
        Vector3 currentPosition = transform.position;

        float distance = Vector3.Distance(
            currentPosition,
            targetPosition
        );

        float moveDistance = moveSpeed * Time.deltaTime;

        // 이번 프레임의 이동으로 타깃에 도달할 수 있는지 확인한다.
        if (distance <= moveDistance + hitRadius)
        {
            target.TakeDamage(damage);
            Despawn();
            return;
        }

        // 타깃 방향으로 이동한다.
        transform.position = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            moveDistance
        );
    }

    private void Despawn()
    {
        if (!isInitialized)
        {
            return;
        }

        isInitialized = false;

        if (spawnManager != null)
        {
            spawnManager.DespawnBullet(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}