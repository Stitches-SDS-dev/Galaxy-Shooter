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

    private void Update() {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y >= _offScreenYPos)
            PoolManager.Instance.ReturnPoolMember(this.gameObject);
    }

    private void OnTriggerEnter(Collider other) {

        other.TryGetComponent<Enemy>(out Enemy enemy);
        if (enemy != null) {
            Destroy(other.gameObject);
            StartCoroutine(ReturnToPool());
        }
    }

    IEnumerator ReturnToPool() {
        yield return new WaitForSeconds(0.1f);
        PoolManager.Instance.ReturnPoolMember(this.gameObject);
    }
}
