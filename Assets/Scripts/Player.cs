using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 _inputVector = new Vector3(0, 0, 0);

    [SerializeField]
    private float _speed;

    [Header("Player Bounds Settings")]
    [SerializeField]
    [Tooltip("If false will wrap at left edge")]
    private bool _bindLeft;
    [SerializeField]
    [Tooltip("If false will wrap at right edge")]
    private bool _bindRight;
    [SerializeField]
    private float _leftBindX, _rightBindX, _leftWrapX, _rightWrapX;
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

        if (_inputVector.x != 0 || _inputVector.y != 0) {
            PlayerMovement(_inputVector);
        }
    }

    void PlayerMovement(Vector3 input) {

        Vector3 movementVector = input * _speed;

        transform.Translate(movementVector * Time.deltaTime);

        BindPlayerPosition();
    }

    void BindPlayerPosition() {

        Vector3 playerBounds = transform.position;

        if (_bindLeft) {
            if (transform.position.x <= _leftBindX) {
                playerBounds.x = _leftBindX;
            }
        }
        else {
            if (transform.position.x <= _leftWrapX) {
                playerBounds.x = _rightWrapX;
            }
        }

        if (_bindRight) {
            if (transform.position.x >= _rightBindX) {
                playerBounds.x = _rightBindX;
            }
        }
        else {
            if (transform.position.x >= _rightWrapX) {
                playerBounds.x = _leftWrapX;
            }
        }

        playerBounds.y = Mathf.Clamp(playerBounds.y, _lowerBindY, _upperBindY);

        transform.position = playerBounds;
    }
}
