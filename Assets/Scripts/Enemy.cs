using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Sprite _defaultSprite;
    [SerializeField]
    private SpriteRenderer _renderer;
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

    //[SerializeField]
    //private Animator _anim;
    [SerializeField]
    private BoxCollider2D _collider;
    private bool _exploding;

    public static Action<int> OnEnemyDeath;

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
        //_anim.SetTrigger("Explode");

        OnEnemyDeath?.Invoke(_value);

        yield return new WaitForSeconds(0.5f);

        // Reset the sprite ready to be recycled
        //_renderer.sprite = _defaultSprite;
        PoolManager.Instance.ReturnPoolMember(this.gameObject);
    }
}
