using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollisionHandler : MonoBehaviour
{
    private Player _player;

    // This script handles all collision detection for the Player

    private void Start() {
        _player = transform.GetComponent<Player>();
        if (_player == null)
            Debug.LogError("CollisionHandler cannot find Player!");
    }

    private void OnTriggerEnter2D(Collider2D other) {

        if (other.TryGetComponent<Enemy>(out Enemy enemy)) {
            enemy.Damage();
            _player.Damage();
        }

        if (other.TryGetComponent<Powerup>(out Powerup powerup)) {
            // Activate appropriate functionality dependant on PowerupType

            float duration = powerup.GetDuration();
            float bonus = powerup.GetBonusValue();
            Powerup.PowerupType type = powerup.GetPowerupType();

            if (type == Powerup.PowerupType.TripleShot && !_player.TripleShotStatus()) {
                _player.ToggleTripleShot();
                Destroy(other.gameObject);
                StartCoroutine(PowerupCooldown(duration, () => {
                    _player.ToggleTripleShot();
                }));
            }
            else if (type == Powerup.PowerupType.SpeedBoost && !_player.SpeedBoostStatus()) {
                _player.ToggleSpeedBoost(bonus);
                Destroy(other.gameObject);
                StartCoroutine(PowerupCooldown(duration, () => {
                    _player.ToggleSpeedBoost(bonus);
                }));
            }
            else if (type == Powerup.PowerupType.Shield && !_player.ShieldStatus()) {
                _player.ToggleShield();
                Destroy(other.gameObject);
                StartCoroutine(PowerupCooldown(duration, () => {
                    _player.ToggleShield();
                }));
            }
        }
    }

    IEnumerator PowerupCooldown(float cooldown, Action onComplete) {

        WaitForSeconds wait = new WaitForSeconds(cooldown);
        yield return wait;

        onComplete?.Invoke();
    }
}
