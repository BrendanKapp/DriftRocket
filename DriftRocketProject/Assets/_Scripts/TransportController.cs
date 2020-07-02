using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportController : Entity, IHitbox
{
    private RocketController rocketController;
    private Vector3 target;

    private void Start()
    {
        rocketController = GetComponent<RocketController>();
        StartCoroutine("ResetTarget");
    }
    private void Update()
    {
        rocketController.SetTarget(target);
    }
    private IEnumerator ResetTarget()
    {
        while (true)
        {
            target = FindRandomPosition();
            float distanceToTarget = ((target - transform.position).sqrMagnitude) / (rocketController.MaxSpeed * 250);
            yield return new WaitForSeconds((int)distanceToTarget);
        }
    }
    private Vector3 FindRandomPosition()
    {
        Vector3 returnVector = transform.position + Random.insideUnitSphere * 100;
        returnVector.z = 0;
        return returnVector;
    }
    public override void Disable()
    {
        rocketController.DisableEffects();
        rocketController.SetActive(false); //disables collider, so this method can only be called once
    }
    public override void GetHit(Hitbox hitbox)
    {
        OnHit();
    }
    public void OnHit()
    {
        GameObject mineParticles = ObjectPooler.PoolObject("HitSparks");
        mineParticles.SetActive(true);
        mineParticles.transform.position = transform.position;
        mineParticles.GetComponent<ParticleSystem>().Play();
    }
}
