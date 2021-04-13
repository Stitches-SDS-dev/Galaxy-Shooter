using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    private Vector3 _inputVector = new Vector3(0, 0, 0);

    [Header("Player Settings")]
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _score = 0;
    [SerializeField]
    private int _lives;

    [Header("Player Boundary Settings")]
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

    [Header("Laser Settings")]
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _laserSpawnYOffset;
    [SerializeField]
    private float _laserFireRate;

    private float _lastShot = -5f;

    [Header("Triple Shot Settings")]
    [SerializeField]
    private bool _isTripleShotActive;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private Transform _tripleShotParent;

    [Header("Speed Boost Settings")]
    [SerializeField]
    private bool _isSpeedBoostActive;
    [SerializeField]
    [Tooltip("Too adjust boost multiplier, edit the bonus value of the SpeedBoost powerup. Default: 1.")]
    private float _speedMultiplier = 1;

    [Header("Shield Settings")]
    [SerializeField]
    private bool _isShieldActive;
    [SerializeField]
    private GameObject _shieldGameObject;
    [SerializeField]
    [Tooltip("To adjust shield bonus power, edit the bonus value of the Shield powerup.")]
    private int _shieldPower = 0;

    public static Action OnPlayerDeath;
    public static Action<int> OnScoreChange;
    public static Action<int> OnLivesChanged;

    private void Start() {
        transform.position = new Vector3(0, _lowerBindY, 0);
    }

    private void OnEnable() {
        Enemy.OnEnemyDeath += IncreaseScore;
    }

    private void OnDisable() {
        Enemy.OnEnemyDeath -= IncreaseScore;
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

        if (_inputVector != Vector3.zero) {
            PlayerMovement(_inputVector);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {

            // simple cooldown for firing laser
            if (Time.time > _lastShot + _laserFireRate) {
                _lastShot = Time.time;
                Shoot();
            }
        }
    }

    #region --- Player Position ---

    void PlayerMovement(Vector3 input) {

        Vector3 movementVector = input * _speed * _speedMultiplier * Time.deltaTime;

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

    #region --- Offensive ---

    void Shoot() {

        if (_isTripleShotActive) {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity, _tripleShotParent);
        }
        else {
            Vector3 laserSpawn = transform.position;
            laserSpawn.y += _laserSpawnYOffset;

            PoolManager.Instance.RequestPoolMember(laserSpawn, PoolManager.PoolType.Laser);
        }
    }

    #endregion

    #region --- Player Info Management ---

    void IncreaseScore(int value) {
        _score += value;
        OnScoreChange?.Invoke(_score);
    }

    public void Damage() {

        if (_shieldPower > 0) {
            _shieldPower--;
            if (_shieldPower <= 0) {
                _isShieldActive = false;
                _shieldGameObject.SetActive(false);
            }
        }
        else {
            _lives--;
            OnLivesChanged?.Invoke(_lives);
            if (_lives <= 0) {

                Debug.Log("Game Over bro!");
                PlayerDeath();
            }
        }
    }

    void PlayerDeath() {

        OnPlayerDeath?.Invoke();
        Destroy(this.gameObject);
    }

    #endregion

    #region --- Powerup Management ---

    // All activation / deactivation / query instructions regarding powerups

    public void ToggleTripleShot() {
        _isTripleShotActive = !_isTripleShotActive;
    }

    public void ToggleSpeedBoost(float bonus) {
        _isSpeedBoostActive = !_isSpeedBoostActive;
        if (_isSpeedBoostActive)
            _speedMultiplier += bonus;
        else {
            _speedMultiplier -= bonus;
        }
    }

    public void ToggleShield(int bonus) {
        _isShieldActive = !_isShieldActive;
        if (_isShieldActive) {
            _shieldPower += bonus;
            _shieldGameObject.SetActive(true);
        }
    }

    public bool TripleShotStatus() {
        return _isTripleShotActive;
    }

    public bool SpeedBoostStatus() {
        return _isSpeedBoostActive;
    }

    public bool ShieldStatus() {
        return _isShieldActive;
    }

    #endregion
}
