using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    [Tooltip("Default: ")]
    private float _offSceenYPos;

    [Header("Spawn Parameters")]
    [SerializeField]
    private float _spawnYPos;
    [SerializeField]
    private float _minXSpawn, _maxXSpawn;

    private void Update() {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= _offSceenYPos) {
            Respawn();
        }
    }

    void Respawn() {

        // Respawn up top with random x
        float spawnX = Random.Range(_minXSpawn, _maxXSpawn);
        Vector3 spawnVector = new Vector3(spawnX, _spawnYPos, 0);

        transform.position = spawnVector;
    }

    public void Damage() {
        PoolManager.Instance.ReturnPoolMember(this.gameObject);
    }
}
