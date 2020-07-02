using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncherController : Entity
{
    [SerializeField]
    private int range = 30;
    [SerializeField]
    private int rechargeTime = 30;
    private int rechargeTimeRemaining = 0;

    private void Update()
    {
        if (PlayerController.instance == null) return;
        TargetPlayer();
    }
    private void Rotate (Transform rotateObject)
    {
        Vector3 vectorToTarget = PlayerController.instance.transform.position - rotateObject.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg + 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 15);
    }
    private void TargetPlayer ()
    {
        rechargeTimeRemaining--;
        float playerDistance = (transform.position - PlayerController.instance.transform.position).sqrMagnitude;
        if (playerDistance < range * 25 && rechargeTimeRemaining < rechargeTime / 2) Rotate(transform);
        if (playerDistance < range * 25 && rechargeTimeRemaining < 0)
        {
            rechargeTimeRemaining = rechargeTime;
            Shoot();
        }
    }
    [SerializeField]
    private Transform[] shootPoints;
    private int currentCycle = 0;
    private void Shoot()
    {
        foreach (Transform t in shootPoints)
        {
            t.gameObject.SetActive(true);
        }
        shootPoints[currentCycle].gameObject.SetActive(false);
        MiniMissileController miniMissile = ObjectPooler.PoolObject("MiniMissile").GetComponent<MiniMissileController>();
        miniMissile.ResetEntity();
        miniMissile.transform.position = shootPoints[currentCycle].position;
        miniMissile.transform.rotation = shootPoints[currentCycle].transform.rotation;
        currentCycle++;
        currentCycle = currentCycle >= shootPoints.Length ? 0 : currentCycle;
    }
    public override void Disable()
    {
        gameObject.SetActive(false);
    }
    public override void GetHit(Hitbox hitbox)
    {
        
    }
}
