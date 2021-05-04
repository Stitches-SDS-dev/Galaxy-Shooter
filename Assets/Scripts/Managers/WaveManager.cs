using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    private static WaveManager _instance;
    public static WaveManager Instance {
        get {
            if (_instance == null)
                Debug.LogError("WaveManager is NULL!");

            return _instance;
        }
    }

    [Header("Wave Settings")]
    [SerializeField]
    private int _initialWaveSize;
    [SerializeField]
    private int _waveModifier;
    private int _waveNumber = 1;
    private int _enemiesThisWave;
    private int _enemiesKilled;
    [SerializeField]
    private GameObject _asteroidPrefab;
    [SerializeField]
    private Vector3 _asteroidSpawnPos;

    public static Action OnWaveComplete;

    private void OnEnable() {
        Enemy.OnEnemyDeath += EnemyKilled;
    }

    private void OnDisable() {
        Enemy.OnEnemyDeath -= EnemyKilled;
    }

    private void Awake() {
        _instance = this;
    }

    public int GetEnemyCountThisWave() {

        _enemiesThisWave = _initialWaveSize + (_waveNumber * _waveModifier);
        return _enemiesThisWave;
    }

    void EnemyKilled(int unusedEnemyValue) {
        _enemiesKilled++;

        if (_enemiesKilled == _enemiesThisWave) {

            _waveNumber++;
            _enemiesKilled = 0;

            OnWaveComplete?.Invoke();
            Instantiate(_asteroidPrefab, _asteroidSpawnPos, Quaternion.identity);
        }
    }
}
