using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [SerializeField]
    private int screenShakeMultiplier = 1;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private ParticleSystem starParticleSystem;
    private Transform target;
    public void SetTarget (Transform target)
    {
        this.target = target;
    }
    private bool isActive = false;
    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }

    public static int screenShakeAmount;

    public static CameraFollow instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (UIManager.instance.isAndroidBuild)
        {
            offset.z = -15;
        } else
        {
            offset.z = -10;
        }
    }
    private void FixedUpdate()
    {
        if (isActive) Move();
    }
    private void Move()
    {
        Vector3 movement = Vector3.Lerp(transform.position, target.position, 0.1f);
        transform.position = movement + offset;
        transform.position += Random.insideUnitSphere * screenShakeAmount * screenShakeMultiplier * 0.02f;
        screenShakeAmount--;
        screenShakeAmount = Mathf.Clamp(screenShakeAmount, 0, 100);
    }
    public void ResetStars ()
    {
        StartCoroutine(ResetStarsRoutine());
    }
    private IEnumerator ResetStarsRoutine ()
    {
        yield return null;
        transform.position = target.position + offset;
        starParticleSystem.Stop();
        starParticleSystem.Play();
    }
}
