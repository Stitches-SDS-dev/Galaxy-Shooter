using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isPlayerAlive = true;

    private void OnEnable() {
        Player.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnDisable() {
        Player.OnPlayerDeath -= OnPlayerDeath;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R) && !_isPlayerAlive) {
            SceneManager.LoadScene(0);
        }
    }

    void OnPlayerDeath() {
        _isPlayerAlive = false;
    }
}
