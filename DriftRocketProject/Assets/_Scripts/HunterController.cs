using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HunterController : Entity
{   
    protected float distanceToPlayer = 0;
    protected RocketController rocketController;

    private void Start()
    {
        rocketController = GetComponent<RocketController>();
        StartCoroutine("SlowUpdateRoutine");
    }
    private void Update()
    {
        rocketController.SetTarget(PlayerController.instance.transform.position);
        distanceToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
        NormalUpdate();
    }
    protected abstract void NormalUpdate();
    private IEnumerator SlowUpdateRoutine()
    {
        while (true)
        {
            SlowUpdate();
            yield return new WaitForSeconds(0.1f);
        }
    }
    protected abstract void SlowUpdate();
    public override void Disable()
    {
        StopCoroutine("SlowUpdateRoutine");
        rocketController.DisableEffects();
        rocketController.SetActive(false); //disables collider, so this method can only be called once
    }
    public override void GetHit(Hitbox hitbox)
    {
        
    }
}
