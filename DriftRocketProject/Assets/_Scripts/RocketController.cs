using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    [SerializeField]
    private float acceleration = 5;
    [SerializeField]
    private float maxSpeed = 10;
    public float MaxSpeed
    {
        get
        {
            return maxSpeed;
        }
    }
    [SerializeField]
    private float initialBrakeSpeed = 0.5f;
    private float brakeSpeed = 0.5f;
    [SerializeField]
    private float rotationalSpeed = 5;
    [SerializeField]
    private float mass = 5;
    [HideInInspector]
    public bool canBrake = true;
    [SerializeField]
    private float brakingThreshold = 0.5f;
    [SerializeField]
    private ParticleSystem fireTrail;
    [SerializeField]
    private ParticleSystem[] extraFireTrails;
    [SerializeField]
    private string[] onDisableDrop;
    [SerializeField]
    private ParticleSystem[] onDisablePlay;

    private bool isActive = true;

    private Rigidbody2D rb;
    private MeshRenderer rend;
    private BoxCollider2D boxCollider;
    private AudioSource engineSound;

    private float input = 0;
    private bool isBraking = false;
    private Vector2 currentVelocity = Vector2.zero;
    private float currentVelocityMagnitute = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponentInChildren<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        engineSound = ObjectPooler.PoolObject("Sounds", "RocketSound").GetComponent<AudioSource>();
        engineSound.transform.position = transform.position;
        engineSound.transform.SetParent(transform);
        engineSound.Play();
        rb.mass = mass;
        brakeSpeed = initialBrakeSpeed;
        UpdateSound();
    }
    private void OnEnable()
    {
        GameController.SetEvent += SetActive;
        UIManager.SoundEvent += UpdateSound;
    }
    private void OnDisable()
    {
        GameController.SetEvent -= SetActive;
        UIManager.SoundEvent -= UpdateSound;

    }
    public void DisableEffects()
    {
        if (onDisableDrop.Length != 0)
        {
            foreach (string itemName in onDisableDrop)
            {
                Drop(itemName);
            }
        }
        if (onDisablePlay.Length != 0)
        {
            foreach (ParticleSystem particleName in onDisablePlay)
            {
                particleName.Play();
            }
        }
    }
    private void Drop(string itemName)
    {
        GetComponent<RocketController>().SetActive(false);
        GameObject item = ObjectPooler.PoolObject(itemName);
        if (item == null) return;
        item.transform.position = transform.position;
        Entity itemEntity = item.GetComponent<Entity>();
        if (itemEntity != null) itemEntity.ResetEntity();
        item.SetActive(true);
    }
    public void SetRotationalSpeed(float rotationalSpeed)
    {
        this.rotationalSpeed = rotationalSpeed;
    }
    public void SetInput(float input)
    {
        this.input = input;
    }
    public void SetBraking(bool isBraking)
    {
        this.isBraking = isBraking;
    }
    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
        brakeSpeed = initialBrakeSpeed;
        if (!isActive)
        {
            fireTrail.Stop();
        } else
        {
            fireTrail.Play();
        }
        if (boxCollider != null  || !ReferenceEquals(boxCollider, null)) boxCollider.enabled = isActive;
    }
    public void SetTarget(Vector3 target)
    {
        float input = Vector2.SignedAngle(transform.up, transform.position - target) / 180;
        if (canBrake) SetBraking(Mathf.Abs(input) < brakingThreshold);
        SetInput(input);
    }
    public void SetProfile (float acceleration, float maxSpeed, float rotationalSpeed)
    {
        this.acceleration = acceleration;
        this.maxSpeed = maxSpeed;
        this.rotationalSpeed = rotationalSpeed;
    }
    public Vector3 GetProfile ()
    {
        return new Vector3(acceleration, maxSpeed, rotationalSpeed);
    }
    public void AddForce (Vector2 force)
    {
        rb.velocity += force;
    }
    private void Update()
    {
        DisableWhenInvisible();
        SetSound();
    }
    public void UpdateSound ()
    {
        if (UIManager.soundActive)
        {
            engineSound.gameObject.SetActive(true);
        } else
        {
            engineSound.gameObject.SetActive(false);
        }
    }
    private void SetSound()
    {
        if (!isActive) return;
        engineSound.volume = isBraking ? 0 : 1;
    }
    private void DisableWhenInvisible()
    {
        if (!isActive && !rend.isVisible)
        {
            gameObject.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        if (isActive)
        {
            Rotation();
            VelocityBasedMovement();
        } else
        {
            isBraking = true;
            brakeSpeed = 0.3f;
            VelocityBasedMovement();
        }
    }
    private void VelocityBasedMovement()
    {
        //lerp the current towards the forward
        currentVelocity = rb.velocity;
        currentVelocityMagnitute = currentVelocity.sqrMagnitude;
        Vector2 targetVelocity = transform.up * maxSpeed;
        if (isBraking && fireTrail.isPlaying)
        {
            fireTrail.Stop();
            foreach (ParticleSystem trail in extraFireTrails)
            {
                trail.Stop();
            }
        }
        if (!isBraking && !fireTrail.isPlaying)
        {
            fireTrail.Play();
            foreach (ParticleSystem trail in extraFireTrails)
            {
                trail.Play();
            }
        }
        if (isBraking)
        {
            rb.velocity -= currentVelocity * Time.fixedDeltaTime * brakeSpeed;
        }
        else
        {
            Vector2 newVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration / 200);
            rb.velocity = newVelocity;
        }
    }
    private void Rotation()
    {
        if (input != 0)
        {
            float speedMultiplier = 300 - (maxSpeed / 100 * currentVelocityMagnitute);
            if (speedMultiplier < 50)
            {
                speedMultiplier = 50;
            }
            float brakeBoost = isBraking ? 2 : 1;
            rb.angularVelocity = (-input * speedMultiplier * brakeBoost * rotationalSpeed * Time.fixedDeltaTime);
        }
    }
}
