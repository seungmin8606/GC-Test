using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private float spawnInterval = 0.2f;
    [SerializeField]
    private Vector3 spawnPosition =
        new Vector3(-9f, 0f, 0f);

    private float spawnTimer;

    private void Update()
    {
        if (spawnManager == null)
        {
            return;
        }

        spawnTimer += Time.unscaledDeltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer -= spawnInterval;
            spawnManager.SpawnEnemy(spawnPosition);
        }
    }

    private void OnValidate()
    {
        if (spawnInterval <= 0f)
        {
            spawnInterval = 0.2f;
        }
    }
}