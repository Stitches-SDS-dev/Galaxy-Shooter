using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour 
{
    [SerializeField]
    private PowerupType _type;
    [SerializeField]
    private AudioClip _sFX;
    [SerializeField]
    private float _cooldownDuration;
    [SerializeField]
    private float _bonusEffectValue;
    [SerializeField]
    private float _offScreenYPos;
    [SerializeField]
    private float _speed;

    public enum PowerupType {
        TripleShot,
        SpeedBoost,
        Shield,
        Ammo
    }

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= _offScreenYPos)
            Destroy(this.gameObject);
    }

    public PowerupType GetPowerupType() {
        return _type;
    }

    public float GetDuration() {
        return _cooldownDuration;
    }

    public float GetBonusValue() {
        return _bonusEffectValue;
    }

    public void PlaySFX() {
        AudioManager.Instance.PlayClip(_sFX);
    }
}
