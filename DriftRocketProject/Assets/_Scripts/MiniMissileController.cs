using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMissileController : Entity, IHitbox
{

    [SerializeField]
    private int countdownMax = 15;
    [SerializeField]
    private int countdownMin = 10;

    private RocketController rocketController;

    private void Awake()
    {
        rocketController = GetComponent<RocketController>();
    }
    public override void ResetEntity()
    {
        base.ResetEntity();
        rocketController.SetActive(true);
        StopCoroutine("Countdown");
        if (gameObject.activeSelf)
        {
            StartCoroutine("Countdown");
        }
    }
    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(Random.Range(countdownMin, countdownMax));
        rocketController.SetActive(false);
        yield return new WaitForSeconds(2);
        Disable();
    }
    public override void GetHit(Hitbox hitbox)
    {
        Disable();
    }
    public override void Disable()
    {
        GameObject explosion = ObjectPooler.PoolObject("Explosion");
        explosion.transform.position = transform.position;
        gameObject.SetActive(false);
    }
    public void OnHit()
    {
        Disable();
    }
}
