using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerHandler : MonoBehaviour
{
    [SerializeField]
    private Player _player;

    // This script handles all trigger detections for the Player

    private void OnTriggerEnter2D(Collider2D other) {

        if (other.TryGetComponent<Enemy>(out Enemy enemy)) {
            enemy.Damage();
            _player.Damage();
        }
        else if (other.TryGetComponent<Powerup>(out Powerup powerup)) {
            // Activate appropriate functionality dependant on PowerupType

            Powerup.PowerupType type = powerup.GetPowerupType();
            float duration = powerup.GetDuration();
            float bonus = powerup.GetBonusValue();

            switch (type) {
                case Powerup.PowerupType.TripleShot:

                    if (!_player.TripleShotStatus()) {
                        _player.ToggleTripleShot();
                        Destroy(other.gameObject);
                        StartCoroutine(PowerupCooldown(duration, () => {
                            _player.ToggleTripleShot();
                        }));
                    }
                    break;

                case Powerup.PowerupType.SpeedBoost:

                    if (!_player.SpeedBoostStatus()) {
                        _player.ToggleSpeedBoost(bonus);
                        Destroy(other.gameObject);
                        StartCoroutine(PowerupCooldown(duration, () => {
                            _player.ToggleSpeedBoost(bonus);
                        }));
                    }
                    break;

                case Powerup.PowerupType.Shield:

                    if (!_player.ShieldStatus()) {
                        _player.ToggleShield((int)bonus);
                        Destroy(other.gameObject);
                        // Shield depletes with damage so no cooldown required
                    }
                    break;

                default:
                    Debug.Log("No such Powerup: " + type);
                    break;
            }
        }
    }

    IEnumerator PowerupCooldown(float cooldown, Action onComplete) {

        WaitForSeconds wait = new WaitForSeconds(cooldown);
        yield return wait;

        onComplete?.Invoke();
    }
}
