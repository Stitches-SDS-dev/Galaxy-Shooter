using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Player))]
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

            Powerup.PowerupType type = powerup.GetPowerupType();
            float duration = powerup.GetDuration();
            float bonus = powerup.GetBonusValue();         

            PowerupSelection(type, powerup, duration, bonus);
        }
    }

    void PowerupSelection(Powerup.PowerupType type, Powerup powerup, float duration, float bonus) {

        // Activate appropriate functionality dependant on PowerupType
        
        switch (type) {
            case Powerup.PowerupType.TripleShot:

                if (!_player.TripleShotStatus()) {

                    powerup.PlaySFX();
                    _player.ToggleTripleShot();
                    Destroy(powerup.gameObject);

                    StartCoroutine(PowerupCooldown(duration, () => {
                        _player.ToggleTripleShot();
                    }));
                }
                break;

            case Powerup.PowerupType.SpeedBoost:

                if (!_player.SpeedBoostStatus()) {

                    powerup.PlaySFX();
                    _player.ToggleSpeedBoost(bonus);
                    Destroy(powerup.gameObject);

                    StartCoroutine(PowerupCooldown(duration, () => {
                        _player.ToggleSpeedBoost(bonus);
                    }));
                }
                break;

            case Powerup.PowerupType.Shield:

                if (!_player.ShieldStatus()) {

                    powerup.PlaySFX();
                    _player.ToggleShield((int)bonus);
                    Destroy(powerup.gameObject);

                    // Shield depletes with damage so no cooldown required
                }
                break;

            case Powerup.PowerupType.Ammo:
                
                if (_player.AmmoStatus() < 15) {

                    powerup.PlaySFX();
                    _player.AddAmmo((int)bonus);
                    Destroy(powerup.gameObject);

                    // Only adds ammo to the Player so no cooldown required
                }
                break;

            case Powerup.PowerupType.Health:

                if (_player.LivesStatus() < 3) {

                    powerup.PlaySFX();
                    _player.AddLife((int)bonus);
                    Destroy(powerup.gameObject);

                    // Only adds a life to the Player so no cooldown required
                }
                break;

            default:
                Debug.Log("No such Powerup: " + type);
                break;
        }
    }

    IEnumerator PowerupCooldown(float cooldown, Action onComplete) {

        WaitForSeconds wait = new WaitForSeconds(cooldown);
        yield return wait;

        onComplete?.Invoke();
    }
}
