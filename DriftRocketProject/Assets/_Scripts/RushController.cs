using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushController : Pickup
{
    public override void OnCollide()
    {
        PlayerController.instance.Rush();
        Sound.PlaySound("RushSound");
    }
}
