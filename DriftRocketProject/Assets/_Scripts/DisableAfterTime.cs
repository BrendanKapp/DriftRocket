using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    [SerializeField]
    private float time = 1;

    private void OnEnable()
    {
        Invoke("Disable", time);    
    }
    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
