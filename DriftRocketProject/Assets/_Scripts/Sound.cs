using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public static GameObject PlaySound (string soundID)
    {
        AudioSource audioSource = ObjectPooler.PoolObject("Sounds", soundID).GetComponent<AudioSource>();
        audioSource.Play();
        return audioSource.gameObject;
    }
}
