using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField]
    protected int maxHp;
    [SerializeField]
    protected int hp;
    protected bool canTakeDamage = true;

    private void Start()
    {
        hp = maxHp;
        GameController.ResetEvent += ResetEntity;
    }
    private void OnApplicationQuit()
    {
        GameController.ResetEvent -= ResetEntity;
    }
    public virtual void ResetEntity()
    {
        hp = maxHp;
    }
    private void TakeDamage(Hitbox hitbox) //this is called by the entity when it collides with the hitbox
    {
        if (!canTakeDamage) return;
        hp -= GetDamage(hitbox);
        GetHit(hitbox);
        if (hp < 1)
        {
            Disable();
        }
    }
    protected int GetDamage(Hitbox hitbox)
    {
        Rigidbody2D hitRB = hitbox.GetComponent<Rigidbody2D>();
        return hitbox.Damage(hitRB == null ? 0 : (int)hitRB.velocity.sqrMagnitude);
    }
    protected int GetKnockback(Hitbox hitbox)
    {
        return hitbox.Knockback;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hitbox")
        {
            Hitbox hitbox = collision.gameObject.GetComponent<Hitbox>();
            hitbox.OnHit();
            TakeDamage(hitbox);
        }
    }
    public abstract void GetHit(Hitbox hitbox); //this is called when you are hit by a hitbox
    public abstract void Disable(); //this is called when your health is below 0 after a hitbox hits you
}
