using UnityEngine;

public class Test0_Baseline : MonoBehaviour
{
    [Header("Baseline Settings (Unoptimized)")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int spawnCount = 100;       // 한 번에 생성할 개수
    [SerializeField] private float spawnInterval = 0.5f; // 생성 주기 (초)
    [SerializeField] private float lifeTime = 2.0f;      // 오브젝트 생존 시간 (지연 파괴)

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        // 0.5초 주기로 대량 생성 트리거 발생
        if (timer >= spawnInterval)
        {
            timer -= spawnInterval;
            SpawnObjects();
        }
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // 1. 매번 Instantiate를 호출하여 힙 메모리 할당 및 파편화 유발
            GameObject bullet = Instantiate(bulletPrefab);

            // 2. 즉시 파괴하지 않고 lifeTime 뒤에 파괴하여 현실적인 메모리 부하 유지
            Destroy(bullet, lifeTime);
        }
    }
}