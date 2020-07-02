using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineController : Entity, IHitbox
{
    [SerializeField]
    private int damage = 3;
    [SerializeField]
    private ParticleSystem explosionParticles;
    [SerializeField]
    private ParticleSystem particles;
    private MeshRenderer mesh;
    private CircleCollider2D col;

    private void Awake()
    {
        mesh = GetComponentInChildren<MeshRenderer>();
        col = GetComponent<CircleCollider2D>();
    }
    public override void ResetEntity()
    {
        hp = maxHp;
        col.enabled = true;
        mesh.enabled = true;
        particles.gameObject.SetActive(true);
    }
    public int Damage()
    {
        return damage;
    }
    public override void Disable()
    {
        AudioSource explosionSound = ObjectPooler.PoolObject("Sounds", "RocketHitSound").GetComponent<AudioSource>();
        explosionSound.transform.position = transform.position;
        explosionSound.Play();
        GameObject mineParticles = ObjectPooler.PoolObject("MineExplode");
        mineParticles.SetActive(true);
        mineParticles.transform.position = transform.position;
        mineParticles.GetComponent<ParticleSystem>().Play();
        explosionParticles.Play();
        particles.gameObject.SetActive(false);
        col.enabled = false;
        mesh.enabled = false;
        Invoke("DisableEnd", 0.7f);
    }
    private void DisableEnd ()
    {
        gameObject.SetActive(false);
    }
    public override void GetHit(Hitbox hitbox)
    {
        Disable();
    }
    public void OnHit()
    {
        Disable();
    }
}
