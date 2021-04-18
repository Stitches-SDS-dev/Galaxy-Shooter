using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private Vector3 _startPoint;
    [SerializeField]
    private float _spinSpeed;

    public static Action OnAsteroidDestruction;

    private void Start() {
        transform.position = _startPoint;
    }

    private void Update() {
        transform.Rotate(Vector3.forward, _spinSpeed * Time.deltaTime);
    }

    public void Destroyed() {

        if (TryGetComponent<Collider2D>(out Collider2D collider)) {

            collider.enabled = false;
            PoolManager.Instance.RequestPoolMember(transform.position, PoolManager.PoolType.Explosion);

            OnAsteroidDestruction?.Invoke();
            Destroy(this.gameObject, 0.5f);
        }
    }
}
