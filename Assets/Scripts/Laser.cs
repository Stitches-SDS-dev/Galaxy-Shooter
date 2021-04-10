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
            Destroy(this.gameObject);
    }
}
