using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Bullet bulletPrefab;

    public Enemy SpawnEnemy(Vector3 position)
    {
        if (gameManager == null || enemyPrefab == null)
        {
            Debug.LogError(
                "SpawnManager 설정 오류: GameManager 또는 Enemy Prefab이 없습니다.",
                this
            );

            return null;
        }

        // Baseline: 적을 새로 생성한다.
        Enemy enemy = Instantiate(
            enemyPrefab,
            position,
            Quaternion.identity
        );

        enemy.Initialize(gameManager, this);

        return enemy;
    }

    public void DespawnEnemy(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        // Baseline: 적을 파괴한다.
        Destroy(enemy.gameObject);
    }

    public Bullet SpawnBullet(
        Vector3 position,
        Enemy target,
        float damage,
        float speed)
    {
        if (bulletPrefab == null)
        {
            Debug.LogError(
                "SpawnManager 설정 오류: Bullet Prefab이 없습니다.",
                this
            );

            return null;
        }

        if (target == null || !target.IsAlive)
        {
            return null;
        }

        // Baseline: 총알을 새로 생성한다.
        Bullet bullet = Instantiate(
            bulletPrefab,
            position,
            Quaternion.identity
        );

        bullet.Initialize(target, damage, speed, this);

        return bullet;
    }

    public void DespawnBullet(Bullet bullet)
    {
        if (bullet == null)
        {
            return;
        }

        // Baseline: 총알을 파괴한다.
        Destroy(bullet.gameObject);
    }
}