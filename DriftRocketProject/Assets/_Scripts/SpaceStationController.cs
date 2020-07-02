using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStationController : Entity
{
    [SerializeField]
    private Transform[] toSpin;
    [SerializeField]
    private Vector3[] rotationalAxis;
    [SerializeField]
    private float[] power;

    private void Start()
    {
        for (int i = 0; i < toSpin.Length; i++)
        {
            power[i] = Random.value;
        }
        Spin(1000);
    }
    private void Update()
    {
        Spin(10 * Time.deltaTime);
    }
    private void Spin(float relativePower)
    {
        for (int i = 0; i < toSpin.Length - 1; i++)
        {
            toSpin[i].Rotate(rotationalAxis[i] * power[i] * relativePower);
        }
    }
    public override void Disable()
    {
        
    }
    public override void GetHit(Hitbox hitbox)
    {
        
    }
}
