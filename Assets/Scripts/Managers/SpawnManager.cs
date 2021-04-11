using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Spawn Settings")]
    [SerializeField]
    private bool _spawnEnemies = true;
    [SerializeField]
    private float _enemySpawnY;
    [SerializeField]
    private float _enemyMinSpawnX, _enemyMaxSpawnX;
    [SerializeField]
    private float _minEnemySpawnTime, _maxEnemySpawnTime;

    private void Start() {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies() {

        Vector3 enemySpawn = new Vector3(0, 0, 0);
        float enemySpawnX;

        while (_spawnEnemies) {

            yield return new WaitForSeconds(Random.Range(_minEnemySpawnTime, _maxEnemySpawnTime));

            enemySpawnX = Random.Range(_enemyMinSpawnX, _enemyMaxSpawnX);
            enemySpawn.Set(enemySpawnX, _enemySpawnY, 0);

            PoolManager.Instance.RequestPoolMember(enemySpawn, PoolManager.PoolType.Enemy);
        }
    }
}
