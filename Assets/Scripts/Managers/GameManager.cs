using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver;

    [Header("Game Background Settings")]
    [SerializeField]
    private bool _isScrolling;
    [SerializeField]
    private float _largeStarScrollSpeed, _smallStarScrollSpeed;
    [SerializeField]
    private Renderer _largeStarsRenderer, _smallStarsRenderer;
    private Vector2 _largeStarOffset = new Vector2(0, 0), _smallStarOffset = new Vector2(0, 0);

    [Header("Camera Shake Settings")]
    [SerializeField]
    private float _shakeDelay;
    [SerializeField]
    private float _maxShake;

    private void OnEnable() {
        UIManager.OnGameOver += OnGameOver;
        Player.OnPlayerDamage += CameraShake;
    }

    private void OnDisable() {
        UIManager.OnGameOver -= OnGameOver;
        Player.OnPlayerDamage -= CameraShake;
    }

    private void Start() {
        //StartCoroutine(ScrollBackgroundStars());
    }

    private void Update() {

        ScrollBackgroundStars();

        if (_isGameOver) {
            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene(1); // Game Scene
            }
            if (Input.GetKeyDown(KeyCode.M)) {
                SceneManager.LoadScene(0); // Menu Scene
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    void OnGameOver() {
        _isGameOver = true;
    }

    #region --- Scrolling Background ---

    void ScrollBackgroundStars() {
         _largeStarOffset.y -= _largeStarScrollSpeed * Time.deltaTime;
         _smallStarOffset.y -= _smallStarScrollSpeed * Time.deltaTime;

        _largeStarsRenderer.sharedMaterial.SetTextureOffset("_MainTex", _largeStarOffset);
        _smallStarsRenderer.sharedMaterial.SetTextureOffset("_MainTex", _smallStarOffset);
    }

    #endregion

    #region --- Camera Shake ---

    void CameraShake(float duration) {
        StartCoroutine(CameraShakeRoutine(duration));
    }

    IEnumerator CameraShakeRoutine(float duration) {

        Transform main = Camera.main.transform;
        WaitForSeconds wait = new WaitForSeconds(_shakeDelay);
        Vector3 rotation = main.localEulerAngles;
        rotation.z = _maxShake;

        while (duration >= 0) {

            main.eulerAngles = rotation;
            rotation.z -= 0.1f;
            yield return wait;

            main.eulerAngles = -rotation;
            yield return wait;
            duration -= 0.1f;

            Debug.Log(duration);
        }

        main.eulerAngles = Vector3.zero;
    }

    #endregion
}
