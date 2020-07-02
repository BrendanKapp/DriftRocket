using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerController : HunterController
{
    protected override void NormalUpdate()
    {
        Rotate();
    }
    protected override void SlowUpdate()
    {
        if (distanceToPlayer < 100)
        {
            currentWait--;
            MissileShoot();
        }
    }
    [Header("Missiles")]
    [SerializeField]
    private Transform missileShotPoint;
    private int currentWait;
    private void MissileShoot()
    {
        if (currentWait > 0) return;
        float angle = Vector3.Angle(cannon.right, transform.right);
        angle = Mathf.Abs(angle);
        if (angle > 90) angle -= 90;
        if (angle > 70 || angle < 30) return;
        currentWait = 5;
        MiniMissileController missile = ObjectPooler.PoolObject("MiniMissile").GetComponent<MiniMissileController>();
        missile.transform.position = missileShotPoint.transform.position;
        missile.transform.rotation = Quaternion.Euler(0, 0, cannon.rotation.eulerAngles.z - 90);
        //print(cannon.rotation.eulerAngles.z);
        missile.gameObject.SetActive(true);
        missile.ResetEntity();
    }
    [SerializeField]
    private Transform cannon;
    private void Rotate()
    {
        Vector3 vectorToTarget = PlayerController.instance.transform.position - cannon.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        cannon.transform.rotation = Quaternion.Slerp(cannon.transform.rotation, q, Time.deltaTime * 15);
    }
}
