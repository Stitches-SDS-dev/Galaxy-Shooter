using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private bool _isPlayerLaser;
    [SerializeField]
    private float _speed;
    [SerializeField]
    [Tooltip("Default: 7f")]
    private float _offScreenYPos;
    [SerializeField]
    private bool _hasParent;
    [SerializeField]
    private AudioClip _audioClip;

    [Header("Wide Laser Settings")]
    [SerializeField]
    private bool _isWideLaser;
    [SerializeField]
    private float _wideLaserExpansionRate;
    [SerializeField]
    private Vector3 _startingWideLaserScale;

    private bool _isInitialized;

    private void OnEnable() {
        PoolManager.OnPoolMemberCreated += Initialize;

        if (_isInitialized)
            AudioManager.Instance.PlayClip(_audioClip);
    }

    private void OnDisable() {
        PoolManager.OnPoolMemberCreated -= Initialize;

        if (_isWideLaser) {
            transform.localScale = _startingWideLaserScale;
            //_isWideLaser = false;
        }
    }

    void Initialize() {
        _isInitialized = true;
    }

    private void Update() {

        if (_isPlayerLaser) {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);

            if (_isWideLaser)
                XAxisExpansion();

            if (transform.position.y >= _offScreenYPos) {
                if (_hasParent) {
                    Destroy(this.transform.parent.gameObject);
                }
                else {
                    PoolManager.Instance.ReturnPoolMember(this.gameObject);
                }
            }
        }
        else {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

            if (transform.position.y <= _offScreenYPos) {
                Destroy(transform.parent.gameObject);
            }
        }
    }

    void XAxisExpansion() {
        Vector3 currentScale = transform.localScale;
        currentScale.x += _wideLaserExpansionRate;
        transform.localScale = currentScale;
    }

    private void OnTriggerEnter2D(Collider2D other) {

        if (_isPlayerLaser) {
            if (other.TryGetComponent<Enemy>(out Enemy enemy)) {
                enemy.Damage();
                if (!_isWideLaser)
                    StartCoroutine(ReturnToPool());
            }
            else if (other.TryGetComponent<Asteroid>(out Asteroid asteroid)) {
                asteroid.Destroyed();
                if (!_isWideLaser)
                    StartCoroutine(ReturnToPool());
            }
        }
        else {
            if (other.TryGetComponent<Player>(out Player player)) {
                player.Damage();
                Destroy(transform.parent.gameObject);
            }
        }
    }

    IEnumerator ReturnToPool() {

        yield return new WaitForSeconds(0.1f);
        if (_hasParent) {
            Destroy(this.gameObject);
        }
        else {
            PoolManager.Instance.ReturnPoolMember(this.gameObject);
        }
    }
}
