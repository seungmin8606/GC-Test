using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledSpawnManager : SpawnManager
{
    [Header("Pool Settings")]
    [SerializeField] private int initialEnemyPoolSize = 100;
    [SerializeField] private int initialBulletPoolSize = 1200;

    [Header("Pool Statistics")]
    [SerializeField] private int enemyPoolExpansions;
    [SerializeField] private int bulletPoolExpansions;

    private Stack<Enemy> enemyPool;
    private Stack<Bullet> bulletPool;

    private Transform enemyPoolRoot;
    private Transform bulletPoolRoot;

    public int EnemyPoolExpansions => enemyPoolExpansions;
    public int BulletPoolExpansions => bulletPoolExpansions;

    private void Awake()
    {
        if (gameManager == null ||
            enemyPrefab == null ||
            bulletPrefab == null)
        {
            Debug.LogError(
                "PooledSpawnManager: 필수 참조가 없습니다.",
                this
            );

            enabled = false;
            return;
        }

        InitializePools();
    }

    private void InitializePools()
    {
        initialEnemyPoolSize = Mathf.Max(0, initialEnemyPoolSize);
        initialBulletPoolSize = Mathf.Max(0, initialBulletPoolSize);

        enemyPool = new Stack<Enemy>(
            initialEnemyPoolSize
        );

        bulletPool = new Stack<Bullet>(
            initialBulletPoolSize
        );

        enemyPoolRoot = new GameObject("EnemyPool").transform;
        bulletPoolRoot = new GameObject("BulletPool").transform;

        enemyPoolRoot.SetParent(transform);
        bulletPoolRoot.SetParent(transform);

        // 적 풀 사전 생성
        for (int i = 0; i < initialEnemyPoolSize; i++)
        {
            Enemy enemy = CreateEnemy();
            enemyPool.Push(enemy);
        }

        // 총알 풀 사전 생성
        for (int i = 0; i < initialBulletPoolSize; i++)
        {
            Bullet bullet = CreateBullet();
            bulletPool.Push(bullet);
        }
    }

    private Enemy CreateEnemy()
    {
        Enemy enemy = Instantiate(
            enemyPrefab,
            enemyPoolRoot
        );

        enemy.gameObject.SetActive(false);

        return enemy;
    }

    private Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(
            bulletPrefab,
            bulletPoolRoot
        );

        bullet.gameObject.SetActive(false);

        return bullet;
    }

    public override Enemy SpawnEnemy(Vector3 position)
    {
        if (!enabled || enemyPool == null)
        {
            return null;
        }

        Enemy enemy;

        if (enemyPool.Count > 0)
        {
            enemy = enemyPool.Pop();
        }
        else
        {
            // 사전 생성 수량이 부족한 경우
            enemy = CreateEnemy();
            enemyPoolExpansions++;
        }

        enemy.transform.position = position;
        enemy.transform.rotation = Quaternion.identity;

        // 이전 사용 상태 초기화
        enemy.Initialize(gameManager, this);

        enemy.gameObject.SetActive(true);

        return enemy;
    }

    public override void DespawnEnemy(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        // Destroy 대신 비활성화
        enemy.gameObject.SetActive(false);

        // 풀에 반환
        enemyPool.Push(enemy);
    }

    public override Bullet SpawnBullet(
        Vector3 position,
        Enemy target,
        float damage,
        float speed)
    {
        if (!enabled ||
            bulletPool == null ||
            target == null ||
            !target.IsAlive)
        {
            return null;
        }

        Bullet bullet;

        if (bulletPool.Count > 0)
        {
            bullet = bulletPool.Pop();
        }
        else // 사전 생성 수량이 부족한 경우
        {
            bullet = CreateBullet();
            bulletPoolExpansions++;
        }

        bullet.transform.position = position;
        bullet.transform.rotation = Quaternion.identity;

        // 새로운 타깃과 발사 정보 설정
        bullet.Initialize(target, damage, speed, this);
        bullet.gameObject.SetActive(true);

        return bullet;
    }

    public override void DespawnBullet(Bullet bullet)
    {
        if (bullet == null)
        {
            return;
        }

        // Destroy 대신 비활성화
        bullet.gameObject.SetActive(false);

        // 풀에 반환
        bulletPool.Push(bullet);
    }
}