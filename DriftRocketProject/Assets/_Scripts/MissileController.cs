using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : Entity, IHitbox
{
    private RocketController rocketController;

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        rocketController = GetComponent<RocketController>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if (PlayerController.instance != null) rocketController.SetTarget(PlayerController.instance.transform.position);
    }
    public override void ResetEntity ()
    {
        base.ResetEntity();
        boxCollider.enabled = true;
    }
    public override void GetHit(Hitbox hitbox)
    {
        Disable();
    }
    [SerializeField]
    private ParticleSystem explosionParticles;
    public override void Disable()
    {
        AudioSource explosionSound = ObjectPooler.PoolObject("Sounds", "RocketHitSound").GetComponent<AudioSource>();
        explosionSound.transform.position = transform.position;
        explosionSound.Play();
        explosionParticles.Play();
        boxCollider.enabled = false;
        rocketController.SetActive(false);
    }
    public void OnHit()
    {
        Disable();
    }
}
