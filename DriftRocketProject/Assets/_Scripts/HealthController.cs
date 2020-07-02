using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : Pickup
{
    [SerializeField]
    private int healAmount = 15;

    public override void OnCollide()
    {
        PlayerController.instance.Heal(healAmount);
        GameObject healParticles = ObjectPooler.PoolObject("HealEffect");
        healParticles.transform.position = transform.position;
        healParticles.GetComponent<ParticleSystem>().Play();
        Sound.PlaySound("HealEffectSound");
    }
}
