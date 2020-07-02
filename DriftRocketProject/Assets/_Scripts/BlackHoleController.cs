using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleController : MonoBehaviour
{
    [SerializeField]
    private int pullPower = 15;
    [SerializeField]
    private int pullSize = 100;
    [SerializeField]
    private int teleportDistance = 300;
    [SerializeField]
    private ParticleSystem ringParticleSystem;

    private void Start()
    {
        GameController.ResetEvent += ResetEntity;
    }
    private void OnApplicationQuit()
    {
        GameController.ResetEvent -= ResetEntity;
    }
    public virtual void ResetEntity()
    {
        ringParticleSystem.Stop();
    }
    private void OnEnable()
    {
        GameObject sound = Sound.PlaySound("BlackHoleSound");
        sound.transform.position = transform.position;
        sound.transform.SetParent(transform);
    }
    private void Update()
    {
        GetPullTargets();
    }
    private void GetPullTargets ()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, pullSize);
        foreach (Collider2D collider in targets)
        {
            PullTarget(collider.gameObject);
        }
    }
    private void PullTarget (GameObject target)
    {
        Vector3 dir = target.transform.position - transform.position;
        float distance = Vector2.SqrMagnitude(dir);
        dir = dir.normalized;
        target.transform.position -= pullPower * 1000 * (dir / distance) * Time.deltaTime;
        if (distance < 500)
        {
            if (target.GetComponent<Rigidbody2D>() != null)
            {
                float val = Random.value;
                float xOffset = val * teleportDistance;
                float yOffset = (1 - val) * teleportDistance;
                Vector2 targetPos = new Vector2(PositionTransform(transform.position.x, xOffset), PositionTransform(transform.position.y, yOffset));
                target.transform.Translate(new Vector2(xOffset, yOffset));
                if (target.GetComponent<PlayerController>() != null)
                {
                    PlayerController.instance.Teleport();
                    CameraFollow.instance.ResetStars();
                    GameObject soundEffect = Sound.PlaySound("WarpSound");
                    soundEffect.transform.position = PlayerController.instance.transform.position;
                } else if (target.GetComponent<RocketController>() != null)
                {
                    //non player rockets
                    if (Random.value > 0.5f)
                    {
                        target.GetComponent<Entity>().Disable();
                    }
                }
            } else
            {
                target.SetActive(false);
            }
        }
    }
    private float PositionTransform(float initialPosition, float offset)
    {
        if (Random.value < 0.5f)
        {
            return initialPosition + offset;
        } else
        {
            return initialPosition - offset;
        }
    }
}
