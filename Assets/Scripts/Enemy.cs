using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _value;

    [SerializeField]
    [Tooltip("Default: -5.4")]
    private float _offSceenYPos;

    [Header("Spawn Parameters")]
    [SerializeField]
    [Tooltip("Default: 7.5")]
    private float _spawnYPos;
    [SerializeField]
    private float _minXSpawn, _maxXSpawn;

    public static Action<int> OnEnemyDeath;

    private void Update() {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= _offSceenYPos) {
            Respawn();
        }
    }

    void Respawn() {

        // Respawn up top with random x
        float spawnX = UnityEngine.Random.Range(_minXSpawn, _maxXSpawn);
        Vector3 spawnVector = new Vector3(spawnX, _spawnYPos, 0);

        transform.position = spawnVector;
    }

    public void Damage() {

        OnEnemyDeath?.Invoke(_value);
        PoolManager.Instance.ReturnPoolMember(this.gameObject);
    }
}