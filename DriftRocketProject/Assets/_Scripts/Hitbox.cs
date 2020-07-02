using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField]
    private int damage = 5;
    [SerializeField]
    private int knockback = 0;
    public int Knockback
    {
        get
        {
            return knockback;
        }
    }
    public int Damage (int velocity)
    {
        if (!dealVelocityBasedDamage) return damage;
        return velocity / 15;
    }
    [SerializeField]
    private bool dealVelocityBasedDamage = false;
    [SerializeField]
    private IHitbox hitController;
    [SerializeField]
    private bool useHitSparks = true;

    private void Start()
    {
        gameObject.tag = "Hitbox";
        if(hitController == null) hitController = GetComponent<IHitbox>();
    }
    public IHitbox GetHitController ()
    {
        return hitController;
    }
    public void OnHit () //gets played when something collides with the hitbox
    {
        if (hitController != null) hitController.OnHit();
        if (useHitSparks)
        {
            ParticleSystem sparks = ObjectPooler.PoolObject("HitSparks").GetComponent<ParticleSystem>();
            sparks.gameObject.transform.position = transform.position;
            sparks.gameObject.SetActive(true);
            sparks.Play();
        }
    }
}
