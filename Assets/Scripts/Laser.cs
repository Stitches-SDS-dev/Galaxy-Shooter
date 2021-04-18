using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    [Tooltip("Default: 7f")]
    private float _offScreenYPos;
    [SerializeField]
    private bool _hasParent;
    [SerializeField]
    private AudioClip _audioClip;
    
    private bool _isInitialized;

    private void OnEnable() {
        PoolManager.OnPoolMemberCreated += Initialize;

        if (_isInitialized)
            AudioManager.Instance.PlayClip(_audioClip);
    }

    private void OnDisable() {
        PoolManager.OnPoolMemberCreated -= Initialize;
    }

    void Initialize() {
        _isInitialized = true;
    }

    private void Update() {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y >= _offScreenYPos) {
            if (_hasParent) {
                Destroy(this.transform.parent.gameObject);
            }
            else {
                PoolManager.Instance.ReturnPoolMember(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {

        if (other.TryGetComponent<Enemy>(out Enemy enemy)) { 
            enemy.Damage();
            StartCoroutine(ReturnToPool());
        }
        else if (other.TryGetComponent<Asteroid>(out Asteroid asteroid)) {
            asteroid.Destroyed();
            StartCoroutine(ReturnToPool());
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
