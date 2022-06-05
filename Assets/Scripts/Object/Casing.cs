using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField] float deactivateTime = 5.0f;
    [SerializeField] float casingSpin = 1.0f;
    [SerializeField] AudioClip[] audioClips;

    Rigidbody rigidbody;
    AudioSource audioSource;
    MemoryPool memoryPool;

    public void Setup(MemoryPool pool, Vector3 dir)
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        memoryPool = pool;

        rigidbody.velocity = new Vector3(dir.x, 1.0f, dir.z);
        rigidbody.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
            Random.Range(-casingSpin, casingSpin),
            Random.Range(-casingSpin, casingSpin));

        StartCoroutine("DeacrtivateAfterTime");
    }

    private void OnCollisionEnter(Collision collision)
    {
        int index = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    IEnumerator DeacrtivateAfterTime()
    {
        yield return new WaitForSeconds(deactivateTime);

        memoryPool.DeactivatePoolItem(this.gameObject);
    }
}
