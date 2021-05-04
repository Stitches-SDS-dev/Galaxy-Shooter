using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public enum EnemyType {
        Standard,
        SideMoving
    }

    [Header("Enemy Configuration")]
    [SerializeField]
    private EnemyType _type;
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
    [SerializeField]
    private float minFireRate;
    [SerializeField]
    private float maxFireRate;
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

    private Vector3 _direction = new Vector3(0, 0, 0);
    private bool _changeDirection = true;                      // Only for use with 'SideMoving' enemy type

    public static Action<int> OnEnemyDeath;

    private void OnEnable() {
        _preFireDelay = new WaitForSeconds(_delayBeforeFiring);
        _direction = Vector3.down;
        _exploding = false;
        StartCoroutine(FiringRoutine());
    }

    private void Update() {

        if (!_exploding) {

            if (_type == EnemyType.SideMoving) {
                NewDirection();
            }

            transform.Translate(_direction * _speed * Time.deltaTime);
            BindMovement();

            if (transform.position.y <= _offSceenYPos) {
                Respawn();
            }
        }
    }

    void BindMovement() {

        Vector3 pos = transform.position;

        if (pos.x < -9f)
            pos.x = -9f;
        if (pos.x > 9f)
            pos.x = 9f;

        transform.position = pos;
    }

    void NewDirection() {

        if (_changeDirection) {
            int directionChooser = UnityEngine.Random.Range(0, 1000);

            if (directionChooser % 2 == 0 && directionChooser > 150) {
                _direction.x = -0.5f;
            }
            else if (directionChooser % 2 != 0 && directionChooser < 850) {
                _direction.x = 0.5f;
            }
            else {
                _direction.x = 0f;
            }

            _changeDirection = false;
            StartCoroutine(ResetMovement(() => {
                _changeDirection = true;
            }));
        }
    }

    IEnumerator ResetMovement(Action onComplete) {
        yield return new WaitForSeconds(0.75f);

        onComplete?.Invoke();
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

        while (!_exploding) {

            Vector3 laserSpawn = transform.position;
            laserSpawn.y += _laserYOffset;
            Instantiate(_laserPrefab, laserSpawn, Quaternion.identity);
            
            yield return new WaitForSeconds(UnityEngine.Random.Range(minFireRate, maxFireRate));
        }
    }
}
