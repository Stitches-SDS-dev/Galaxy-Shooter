using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void OnEnable() {

        this.gameObject.TryGetComponent<Renderer>(out Renderer renderer);
        if (renderer != null)
            renderer.enabled = true;

        StartCoroutine(ReturnExplosion());
    }

    IEnumerator ReturnExplosion() {

        this.gameObject.TryGetComponent<AudioSource>(out AudioSource source);
        if (source != null) {
            yield return new WaitForSeconds(0.1f);
            while (source.isPlaying) {
                yield return null;
            }
            PoolManager.Instance.ReturnPoolMember(this.gameObject);
        }
    }
}
