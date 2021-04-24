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
    [SerializeField]
    private int _maxAmmo;
    private int _currentAmmo;
    [SerializeField]
    private AudioClip _noAmmoSound;

    [Header("Ship Settings")]
    [SerializeField]
    private GameObject[] _engineFires;
    [SerializeField]
    private float _thrusterBoost;

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
    private float _laserSpawnYOffset;
    [SerializeField]
    private float _laserFireRate;

    [Header("Wide Laser")]
    [SerializeField]
    private bool _isWideLaserActive;
    [SerializeField]
    private float _wideLaserSpawnYOffset;

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
    [Tooltip("To adjust boost multiplier, edit the bonus value of the SpeedBoost powerup. Default: 1.")]
    private float _speedMultiplier = 1;

    [Header("Shield Settings")]
    [SerializeField]
    private bool _isShieldActive;
    [SerializeField]
    private GameObject _shieldGameObject;
    [SerializeField]
    private Color[] _shieldColors;
    [SerializeField]
    [Tooltip("To adjust shield bonus power, edit the bonus value of the Shield powerup.")]
    private int _shieldPower = 0;
    [SerializeField]
    private SpriteRenderer _shieldRenderer;

    public static Action OnPlayerDeath;
    public static Action<int> OnScoreChange;
    public static Action<int> OnLivesChanged;
    public static Action<int, int> OnAmmoChanged;

    private void Start() {
        transform.position = new Vector3(0, _lowerBindY, 0);
        _currentAmmo = _maxAmmo;
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

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            ActivateThrusters();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            DeactivateThrusters();
        }
    }

    #region --- Player Position ---

    void PlayerMovement(Vector3 input) {

        Vector3 movementVector = input * _speed * _speedMultiplier * Time.deltaTime;

        transform.Translate(movementVector);

        CheckPlayerBounds();
    }

    void ActivateThrusters() {
        _speedMultiplier += _thrusterBoost;
    }

    void DeactivateThrusters() {
        _speedMultiplier -= _thrusterBoost;
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
            // Shoot Tripleshot
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity, _tripleShotParent);
        }
        else {

            Vector3 laserSpawn = transform.position;

            if (!_isWideLaserActive) {
                // Shoot Normal Laser
                if (_currentAmmo > 0) {
                    laserSpawn.y += _laserSpawnYOffset;

                    PoolManager.Instance.RequestPoolMember(laserSpawn, PoolManager.PoolType.Laser);

                    _currentAmmo--;
                    OnAmmoChanged?.Invoke(_currentAmmo, _maxAmmo);
                }
                else {
                    AudioManager.Instance.PlayClip(_noAmmoSound);
                }
            }
            else {
                // Shoot Wide Laser
                laserSpawn.y += _wideLaserSpawnYOffset;

                PoolManager.Instance.RequestPoolMember(laserSpawn, PoolManager.PoolType.WideLaser);
            }
        }
    }

    #endregion

    #region --- Player Info Management ---

    void IncreaseScore(int value) {
        _score += value;
        OnScoreChange?.Invoke(_score);
    }

    void SetShieldAppearance() {
        _shieldRenderer.color = _shieldColors[_shieldPower - 1];
    }

    public void Damage() {

        if (_lives > 0) {

            if (_shieldPower > 0) {
                // Reduce shieldPower and display appropriate color
                _shieldPower--;
                if (_shieldPower > 0)
                    SetShieldAppearance();

                if (_shieldPower <= 0) {
                    // If shield runs out, deactivate
                    _isShieldActive = false;
                    _shieldGameObject.SetActive(false);
                }
            }
            else {
                _lives--;
                DisplayDamage(false);
                OnLivesChanged?.Invoke(_lives);
                if (_lives <= 0) {

                    Debug.Log("Game Over bro!");
                    PlayerDeath();
                }
            }
        }
    }

    void DisplayDamage(bool isRepair) {

        int engineDamaged = UnityEngine.Random.Range(0, 2);

        switch (engineDamaged) {
            case 0:

                if (!isRepair) {
                    if (!_engineFires[0].activeInHierarchy)
                        _engineFires[0].SetActive(true);
                    else if (!_engineFires[1].activeInHierarchy)
                        _engineFires[1].SetActive(true);
                }
                else {
                    if (_engineFires[0].activeInHierarchy)
                        _engineFires[0].SetActive(false);
                    else if (_engineFires[1].activeInHierarchy)
                        _engineFires[1].SetActive(false);
                }
                break;

            case 1:

                if (!isRepair) {
                    if (!_engineFires[1].activeInHierarchy)
                        _engineFires[1].SetActive(true);
                    else if (!_engineFires[0].activeInHierarchy)
                        _engineFires[0].SetActive(true);
                }
                else {
                    if (_engineFires[1].activeInHierarchy)
                        _engineFires[1].SetActive(false);
                    else if (_engineFires[0].activeInHierarchy)
                        _engineFires[0].SetActive(false);
                }
                break;
        }
    }

    void PlayerDeath() {

        PoolManager.Instance.RequestPoolMember(transform.position, PoolManager.PoolType.Explosion);
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
            SetShieldAppearance();
        }
    }

    public void ToggleWideLaser() {
        _isWideLaserActive = !_isWideLaserActive;
    }

    public void AddAmmo(int bonus) {
        _currentAmmo = bonus;
        OnAmmoChanged?.Invoke(_currentAmmo, _maxAmmo);
    }

    public void AddLife(int bonus) {
        _lives += bonus;
        OnLivesChanged?.Invoke(_lives);
        DisplayDamage(true);
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

    public bool WideLaserStatus() {
        return _isWideLaserActive;
    }

    public int AmmoStatus() {
        return _currentAmmo;
    }

    public int LivesStatus() {
        return _lives;
    }

    #endregion
}
