using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver;

    private void OnEnable() {
        UIManager.OnGameOver += OnGameOver;
    }

    private void OnDisable() {
        UIManager.OnGameOver -= OnGameOver;
    }

    private void Update() {

        if (_isGameOver) {
            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene(1); // Game Scene
            }
            if (Input.GetKeyDown(KeyCode.M)) {
                SceneManager.LoadScene(0); // Menu Scene
            }
        }
    }

    void OnGameOver() {
        _isGameOver = true;
    }
}
