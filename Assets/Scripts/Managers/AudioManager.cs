using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance {
        get {
            if (_instance == null)
                Debug.Log("AudioManager instance is null!");

            return _instance;
        }
    }

    [SerializeField]
    private AudioSource _source;

    private void Awake() {
        _instance = this;
    }

    public void PlayClip(AudioClip clip) {
        _source.PlayOneShot(clip);
    }
}
