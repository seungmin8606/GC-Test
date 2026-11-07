using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float maxHP = 500f;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float exitX = 9f;

    private float currentHP;

    private GameManager gameManager;
    private SpawnManager spawnManager;

    private bool isInitialized;
    private bool isDead;

    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public bool IsAlive => isInitialized && !isDead;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void Initialize(GameManager manager, SpawnManager owner)
    {
        if (manager == null)
        {
            Debug.LogError("Enemy 초기화 실패: GameManager가 없습니다.", this);
            return;
        }

        // 재사용 상황에서도 이전 상태를 정리할 수 있도록 구성한다.
        if (gameManager != null)
        {
            gameManager.UnregisterEnemy(this);
        }

        gameManager = manager;
        spawnManager = owner;

        currentHP = maxHP;
        isDead = false;
        isInitialized = true;

        gameManager.RegisterEnemy(this);
    }

    private void Update()
    {
        if (!isInitialized || isDead)
        {
            return;
        }

        // 적을 오른쪽으로 이동시킨다.
        transform.position +=
            Vector3.right * (moveSpeed * Time.deltaTime);

        // 지정한 좌표에 도달하면 화면 밖으로 이탈한 것으로 처리한다.
        if (transform.position.x >= exitX)
        {
            Escape();
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isInitialized || isDead || damage <= 0f)
        {
            return;
        }

        currentHP -= damage;

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (gameManager != null)
        {
            gameManager.NotifyEnemyKilled(this);
        }

        Despawn();
    }

    private void Escape()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (gameManager != null)
        {
            gameManager.NotifyEnemyEscaped(this);
        }

        Despawn();
    }

    private void Despawn()
    {
        if (spawnManager != null)
        {
            spawnManager.DespawnEnemy(this);
        }
        else
        {
            // SpawnManager를 거치지 않은 경우의 안전장치
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // 외부에서 파괴되더라도 활성 적 목록에서 제거한다.
        if (gameManager != null)
        {
            gameManager.UnregisterEnemy(this);
        }
    }
}