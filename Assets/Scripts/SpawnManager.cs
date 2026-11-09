using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected Enemy enemyPrefab;
    [SerializeField] protected Bullet bulletPrefab;

    public virtual Enemy SpawnEnemy(Vector3 position)
    {
        if (gameManager == null || enemyPrefab == null)
        {
            Debug.LogError(
                "SpawnManager: GameManager 또는 Enemy Prefab 누락",
                this
            );
            return null;
        }

        // Test0: 동적 생성
        Enemy enemy = Instantiate(
            enemyPrefab,
            position,
            Quaternion.identity
        );

        enemy.Initialize(gameManager, this);
        return enemy;
    }

    public virtual void DespawnEnemy(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        // Test0: 파괴
        Destroy(enemy.gameObject);
    }

    public virtual Bullet SpawnBullet(
        Vector3 position,
        Enemy target,
        float damage,
        float speed)
    {
        if (bulletPrefab == null ||
            target == null ||
            !target.IsAlive)
        {
            return null;
        }

        // Test0: 동적 생성
        Bullet bullet = Instantiate(
            bulletPrefab,
            position,
            Quaternion.identity
        );

        bullet.Initialize(target, damage, speed, this);
        return bullet;
    }

    public virtual void DespawnBullet(Bullet bullet)
    {
        if (bullet == null)
        {
            return;
        }

        // Test0: 파괴
        Destroy(bullet.gameObject);
    }
}