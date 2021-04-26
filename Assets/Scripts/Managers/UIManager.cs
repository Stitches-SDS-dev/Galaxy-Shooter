using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Score Display")]
    [SerializeField]
    private TMP_Text _scoreText;
    
    [Header("Game Over Display")]
    [SerializeField]
    private TMP_Text _gameOverText;
    [SerializeField]
    private float _letterDisplayDelay;
    [SerializeField]
    private float _flashDelay;
    [SerializeField]
    private int _flashCount;
    [SerializeField]
    private TMP_Text _restartText;
    [SerializeField]
    private TMP_Text _menuText;

    [Header("Lives Display")]
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Sprite[] _livesImages;

    [Header("Ammo Display")]
    [SerializeField]
    private Image _ammoImage;

    [Header("Thruster Guage")]
    [SerializeField]
    private Image _thrusterImage;
    [SerializeField]
    private float _fillRate;
    [SerializeField]
    private float _drainRate;
    [SerializeField]
    private float _drainBlinkRate;
    private Color _colorChanger;
    private bool _fillGuage, _drainGuage, _waitForFullDrain;
    private WaitForSeconds _fillWait;
    private WaitForSeconds _drainWait;
    private WaitForSeconds _fullDrainBlinkWait;

    public static Action OnGameOver;

    private void OnEnable() {
        Player.OnScoreChange += UpdateScoreDisplay;
        Player.OnLivesChanged += UpdateLivesDisplay;
        Player.OnPlayerDeath += DisplayGameOver;
        Player.OnAmmoChanged += UpdateAmmoDisplay;
        Player.OnThrusterActivityChanged += UpdateThrusterGuage;
        Player.OnQueryGuageState += QueryGuageState;
        Player.OnQueryGuageDrainState += QueryDrainState;
    }

    private void OnDisable() {
        Player.OnScoreChange -= UpdateScoreDisplay;
        Player.OnLivesChanged -= UpdateLivesDisplay;
        Player.OnPlayerDeath -= DisplayGameOver;
        Player.OnAmmoChanged -= UpdateAmmoDisplay;
        Player.OnThrusterActivityChanged -= UpdateThrusterGuage;
        Player.OnQueryGuageState -= QueryGuageState;
        Player.OnQueryGuageDrainState -= QueryDrainState;
    }

    void UpdateScoreDisplay(int score) {
        _scoreText.text = "Score: " + score;
    }

    void UpdateLivesDisplay(int lives) {
        _livesImage.sprite = _livesImages[lives];
    }

    void UpdateAmmoDisplay(int currentAmmo, int maxAmmo) {
        float ammoPercentage = (float)currentAmmo / maxAmmo;
        _ammoImage.fillAmount = ammoPercentage;
    }

    #region --- Game Over ---

    void DisplayGameOver() {

        _gameOverText.enabled = true;

        string msg = _gameOverText.text;
        _gameOverText.text = null;
        StartCoroutine(GameOverRoutine(msg));
    }

    IEnumerator GameOverRoutine(string msg) {

        WaitForSeconds letterDelay = new WaitForSeconds(_letterDisplayDelay);
        WaitForSeconds flashDelay = new WaitForSeconds(_flashDelay);

        for (int i = 0; i < msg.Length; i++) {

            _gameOverText.text += msg[i].ToString();
            yield return letterDelay;
        }

        bool flashGameOver = true;
        int flashCount = 0;

        while (flashGameOver) {

            yield return flashDelay;
            _gameOverText.enabled = false;
            yield return flashDelay;
            _gameOverText.enabled = true;

            flashCount++;
            if (flashCount >= _flashCount) {
                flashGameOver = false;
            }
        }

        _restartText.enabled = true;
        _menuText.enabled = true;
        OnGameOver?.Invoke();
    }

    #endregion

    #region --- Thruster Temp Guage ---

    void UpdateThrusterGuage(bool active) {

        if (active) {
            _fillGuage = true;
            _drainGuage = false;
            StartCoroutine(FillGuage());
        }
        else {
            _drainGuage = true;
            _fillGuage = false;
            StartCoroutine(DrainGuage());
        }
    }

    bool QueryGuageState() {
        return _waitForFullDrain;
    }

    bool QueryDrainState() {
        return _drainGuage;
    }

    IEnumerator FillGuage() {

        _fillWait = new WaitForSeconds(_fillRate);

        _colorChanger = _thrusterImage.color;

        float rValue = _colorChanger.r;
        float gValue = _colorChanger.g;

        while (_fillGuage) {

            if (_thrusterImage.fillAmount < 1f) {
                _thrusterImage.fillAmount += 0.01f;

                // While filling guage, change color first from green to yellow

                if (_colorChanger.r < 1) {

                    rValue += 0.02f;
                }
                else if (_colorChanger.r >= 1) {

                    // After reaching yellow color, continue change to red

                    rValue = 1;
                    gValue -= 0.02f;
                    if (gValue <= 0)
                        gValue = 0;
                }

                _colorChanger.r = rValue;
                _colorChanger.g = gValue;

                _thrusterImage.color = _colorChanger;
            }
            else {

                _waitForFullDrain = true;
                _fillGuage = false;
                StartCoroutine(BlinkGuage());
            }

            yield return _fillWait;
        }
    }

    IEnumerator BlinkGuage() {

        _fullDrainBlinkWait = new WaitForSeconds(_drainBlinkRate);
        bool blink = true;

        UpdateThrusterGuage(false);

        while (blink) {

            _colorChanger.a = 0;
            yield return _fullDrainBlinkWait;

            _colorChanger.a = 0.5f;
            yield return _fullDrainBlinkWait;

            if (!_drainGuage)
                blink = false;
        }
    }

    IEnumerator DrainGuage() {

        _drainWait = new WaitForSeconds(_drainRate);

        _colorChanger = _thrusterImage.color;

        float rValue = _colorChanger.r;
        float gValue = _colorChanger.g;

        while (_drainGuage) {

            if (_thrusterImage.fillAmount > 0) {
                _thrusterImage.fillAmount -= 0.005f;

                // While draining guage, change color from red to yellow

                if (_colorChanger.g < 1) {

                    gValue += 0.01f;
                }
                else if (_colorChanger.g >= 1) {

                    // After reaching yellow color, continue change to green

                    gValue = 1;
                    rValue -= 0.01f;
                    if (rValue <= 0)
                        rValue = 0;
                }

                _colorChanger.r = rValue;
                _colorChanger.g = gValue;

                _thrusterImage.color = _colorChanger;
            }
            else {

                _colorChanger.a = 0.5f;
                _thrusterImage.color = _colorChanger;

                _waitForFullDrain = false;
                _drainGuage = false;
            }

            yield return _drainWait;
        }
    }

    #endregion
}
