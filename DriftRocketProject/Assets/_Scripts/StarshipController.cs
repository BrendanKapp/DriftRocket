using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarshipController : HunterController
{
    protected override void NormalUpdate()
    {
        rocketController.canBrake = distanceToPlayer > 200 ? true : false;
        rocketController.SetRotationalSpeed(distanceToPlayer < 200 ? 0 : 15);
    }
    protected override void SlowUpdate()
    {
        if (distanceToPlayer < 50)
        {
            DropMines();
        }
    }
    [Header("Mines")]
    [SerializeField]
    private Transform mineDrop;
    [SerializeField]
    private float mineDropThreshold = 10;
    private Vector3 previousDrop;
    private void DropMines()
    {
        if (Vector3.Distance(mineDrop.position, previousDrop) < mineDropThreshold) return;
        previousDrop = mineDrop.position;
        GameObject mine = ObjectPooler.PoolObject("Mine");
        mine.transform.position = mineDrop.position;
        mine.GetComponent<Entity>().ResetEntity();
        mine.transform.localScale = Vector3.one * Random.Range(1f, 3f);
    }
}
