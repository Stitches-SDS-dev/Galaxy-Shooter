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
    private GameObject _laserPrefab;
    [SerializeField]
    private float _delayBeforeFiring;
    [SerializeField]
    private float _laserYOffset;
    private WaitForSeconds _preFireDelay;

    [SerializeField]
    [Tooltip("Default: -5.4")]
    private float _offSceenYPos;

    [SerializeField]
    private BoxCollider2D _collider;
    private bool _exploding;

    [Header("Spawn Parameters")]
    [SerializeField]
    [Tooltip("Default: 7.5")]
    private float _spawnYPos;
    [SerializeField]
    private float _minXSpawn, _maxXSpawn;

    

    public static Action<int> OnEnemyDeath;

    private void OnEnable() {
        _preFireDelay = new WaitForSeconds(_delayBeforeFiring);
        StartCoroutine(FiringRoutine());
    }

    private void Update() {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= _offSceenYPos && !_exploding) {
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

        StartCoroutine(DestructionRoutine());
    }

    IEnumerator DestructionRoutine() {

        _collider.enabled = false;
        _exploding = true;
        PoolManager.Instance.RequestPoolMember(transform.position, PoolManager.PoolType.Explosion);

        OnEnemyDeath?.Invoke(_value);

        yield return new WaitForSeconds(0.5f);

        PoolManager.Instance.ReturnPoolMember(this.gameObject);
    }

    IEnumerator FiringRoutine() {

        yield return _preFireDelay;

        while (isActiveAndEnabled) {

            Vector3 laserSpawn = transform.position;
            laserSpawn.y += _laserYOffset;
            Instantiate(_laserPrefab, laserSpawn, Quaternion.identity);
            
            yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 6f));
        }
    }
}
