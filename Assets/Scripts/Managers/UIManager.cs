using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
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

    [Header("Lives Display")]
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Sprite[] _livesImages;

    private void OnEnable() {
        Player.OnScoreChange += UpdateScoreDisplay;
        Player.OnLivesChanged += UpdateLivesDisplay;
        Player.OnPlayerDeath += DisplayGameOver;
    }

    private void OnDisable() {
        Player.OnScoreChange -= UpdateScoreDisplay;
        Player.OnLivesChanged -= UpdateLivesDisplay;
        Player.OnPlayerDeath -= DisplayGameOver;
    }

    void UpdateScoreDisplay(int score) {
        _scoreText.text = "Score: " + score;
    }

    void UpdateLivesDisplay(int lives) {
        _livesImage.sprite = _livesImages[lives];
    }

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
    }
}
