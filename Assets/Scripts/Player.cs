using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 _inputVector = new Vector3(0, 0, 0);

    [Header("Player Settings")]
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _lives;

    [Header("Laser Settings")]
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _laserSpawnYOffset;
    [SerializeField]
    private float _laserFireRate;

    private float _lastShot = -5f;

    [Header("Player Bound Settings")]
    [SerializeField]
    [Tooltip("If false will wrap at left edge")]
    private bool _bindLeft;
    [SerializeField]
    [Tooltip("If false will wrap at right edge")]
    private bool _bindRight;
    [SerializeField]
    private float _leftBindX, _rightBindX;
    [SerializeField]
    private float _leftWrapX, _rightWrapX;
    [SerializeField]
    private float _lowerBindY, _upperBindY;

    private void Start() {
        transform.position = new Vector3(0, -3, 0);
    }

    private void Update() {

        PlayerInput();
    }

    void PlayerInput() {

        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        _inputVector.Set(xInput, yInput, 0);        

        // If input is detected, run movement code
        // In place to prevent unnecessary code running when recieving no input

        if (_inputVector.x != 0 || _inputVector.y != 0) {
            PlayerMovement(_inputVector);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {

            // simple cooldown for firing laser
            if (Time.time > _lastShot + _laserFireRate) {
                _lastShot = Time.time;
                Shoot();
            }
        }
    }

    #region --- Player Position ---

    void PlayerMovement(Vector3 input) {

        Vector3 movementVector = input * _speed * Time.deltaTime;

        transform.Translate(movementVector);

        CheckPlayerBounds();
    }

    void CheckPlayerBounds() {

        Vector3 playerBounds = transform.position;

        if (_bindLeft) {
            if (playerBounds.x <= _leftBindX) {
                playerBounds.x = _leftBindX;
            }
        }
        else {
            if (playerBounds.x <= _leftWrapX) {
                playerBounds.x = _rightWrapX;
            }
        }

        if (_bindRight) {
            if (playerBounds.x >= _rightBindX) {
                playerBounds.x = _rightBindX;
            }
        }
        else {
            if (playerBounds.x >= _rightWrapX) {
                playerBounds.x = _leftWrapX;
            }
        }

        playerBounds.y = Mathf.Clamp(playerBounds.y, _lowerBindY, _upperBindY);

        transform.position = playerBounds;
    }

    #endregion

    void Shoot() {

        Vector3 laserSpawn = transform.position;
        laserSpawn.y += _laserSpawnYOffset;

        PoolManager.Instance.RequestPoolMember(laserSpawn, PoolManager.PoolType.Laser);
    }

    void Damage() {

        _lives--;
        if (_lives <= 0) {
            //game over bro
            Debug.Log("Game Over bro!");
        }
    }

    private void OnTriggerEnter(Collider other) {

        if (other.TryGetComponent<Enemy>(out Enemy enemy)) {
            enemy.Damage();
            Damage();
        }
    }
}
