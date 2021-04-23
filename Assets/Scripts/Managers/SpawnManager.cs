using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Spawn Settings")]
    [SerializeField]
    private bool _spawnEnemies;
    [SerializeField]
    private float _enemySpawnY;
    [SerializeField]
    private float _enemyMinSpawnX, _enemyMaxSpawnX;
    [SerializeField]
    private float _minEnemySpawnTime, _maxEnemySpawnTime;

    [Header("Powerup Spawn Settings")]
    [SerializeField]
    private bool _spawnPowerups;
    [SerializeField]
    private GameObject[] _powerupPrefabs;
    [SerializeField]
    private float _powerupSpawnY;
    [SerializeField]
    private float _powerupMinSpawnX, _powerupMaxSpawnX;
    [SerializeField]
    private float _minPowerupSpawnTime, _maxPowerupSpawnTime;

    private void OnEnable() {
        Player.OnPlayerDeath += OnPlayerDeath;
        Asteroid.OnAsteroidDestruction += StartSpawning;
    }

    private void OnDisable() {
        Player.OnPlayerDeath -= OnPlayerDeath;
        Asteroid.OnAsteroidDestruction -= StartSpawning;
    }

    void StartSpawning() {

        _spawnEnemies = _spawnPowerups = true;

        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerups());
    }

    IEnumerator SpawnEnemies() {

        Vector3 enemySpawn = new Vector3(0, 0, 0);
        float enemySpawnX;

        while (_spawnEnemies) {

            yield return new WaitForSeconds(Random.Range(_minEnemySpawnTime, _maxEnemySpawnTime));

            if (!_spawnEnemies) {
                break;
            }

            enemySpawnX = Random.Range(_enemyMinSpawnX, _enemyMaxSpawnX);
            enemySpawn.Set(enemySpawnX, _enemySpawnY, 0);

            PoolManager.Instance.RequestPoolMember(enemySpawn, PoolManager.PoolType.Enemy);
        }
    }

    IEnumerator SpawnPowerups() {

        Vector3 powerupSpawn = new Vector3(0, 0, 0);
        float powerupSpawnX;
        int powerupIndex;

        while (_spawnPowerups) {

            yield return new WaitForSeconds(Random.Range(_minPowerupSpawnTime, _maxPowerupSpawnTime));

            if (!_spawnPowerups) {
                break;
            }

            powerupSpawnX = Random.Range(_powerupMinSpawnX, _powerupMaxSpawnX);
            powerupSpawn.Set(powerupSpawnX, _powerupSpawnY, 0);

            powerupIndex = Random.Range(0, _powerupPrefabs.Length);

            Instantiate(_powerupPrefabs[powerupIndex], powerupSpawn, Quaternion.identity, this.transform);
        }
    }

    void OnPlayerDeath() {
        _spawnEnemies = _spawnPowerups = false;
    }
}
