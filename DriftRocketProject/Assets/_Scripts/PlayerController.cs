using System.Collections;
using UnityEngine.PostProcessing;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity
{
    [SerializeField]
    private ParticleSystem healParticles;
    [SerializeField]
    private MeshRenderer hullRenderer;
    [SerializeField]
    private Material startMaterial;
    [SerializeField]
    private Material getHitMaterial;
    [SerializeField]
    private PostProcessingProfile postProcessingProfile;
    [SerializeField]
    private ParticleSystem zoomParticleSystem;
    [SerializeField]
    private MeshRenderer[] primaryMesh;
    [SerializeField]
    private MeshRenderer[] secondaryMesh;

    private RocketController rocketController;
    private float initialAcceleration;
    private float initialMaxSpeed;
    private float initialRotationalSpeed;

    public static PlayerController instance;

    private float input = 0;
    private bool braking = false;

    private void Awake()
    {
        rocketController = GetComponent<RocketController>();
        initialAcceleration = rocketController.GetProfile().x;
        initialMaxSpeed = rocketController.GetProfile().y;
        initialRotationalSpeed = rocketController.GetProfile().z;
        instance = this;
    }
    private void Update()
    {
        SetDesktopInput();
        SetAccelerameterInput();
        rocketController.SetInput(input);
        rocketController.SetBraking(braking);
    }
    public void SetAccelerameterInput()
    {
        if (UIManager.controlType != ControlType.andriod || UIManager.instance.getMobileControlType != MobileControlType.accelerometer) return;
        Vector3 tilt = Input.acceleration;
        tilt = tilt.normalized;
        input = tilt.x * 5;
        input = Mathf.Clamp(input, -1, 1);
        braking = Input.GetMouseButton(0);
    }
    public void SetDesktopInput() //also being used to set andriod joystick controls
    {
        if (UIManager.controlType == ControlType.andriod && UIManager.instance.getMobileControlType != MobileControlType.joystick) return;
        input = Input.GetAxisRaw("Horizontal");
        braking = Input.GetButton("Brake");
    }
    public void SetMobileInput(float input)
    {
        this.input = input;
    }
    public void SetMobileBraking(bool braking)
    {
        this.braking = braking;
    }
    public override void Disable()
    {
        StopCoroutine("FreezeFrame");
        Time.timeScale = 1;
        rocketController.SetActive(false);
        GameController.instance.GameOver();
    }
    public void Heal(int amount)
    {
        if (hp == maxHp)
        {
            //do something else here (maybe increase max hp?)
            //maxHp += 10;
        }
        hp += amount;
        hp = hp > maxHp ? maxHp : hp;
        FreezeFrame(amount * 2);
        UIManager.instance.SetHealthbar(hp, maxHp);
    }
    public void Rush ()
    {
        CameraFollow.screenShakeAmount = 70;
        canTakeDamage = false;
        zoomParticleSystem.Play();
        rocketController.SetProfile(55, 55, 35);
        StartCoroutine(RushRoutine());
    }
    private IEnumerator RushRoutine()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(1);
            CameraFollow.screenShakeAmount += 20;
        }
        canTakeDamage = true;
        zoomParticleSystem.Stop();
        rocketController.SetProfile(initialAcceleration, initialMaxSpeed, initialRotationalSpeed);
    }
    public void Teleport ()
    {
        GameObject flash = ObjectPooler.PoolObject("Flash");
        flash.transform.position = transform.position;
        flash.transform.rotation = transform.rotation;
        flash.SetActive(true);
        CameraFollow.screenShakeAmount += 100;
    }
    public void SetColors (Material primary, Material secondary)
    {
        foreach (MeshRenderer mesh in primaryMesh)
        {
            mesh.material = primary;
        }
        foreach (MeshRenderer mesh in secondaryMesh)
        {
            mesh.material = secondary;
        }
    }
    public void SetVignette(float value)
    {
        VignetteModel.Settings vignetteSettings = postProcessingProfile.vignette.settings;
        vignetteSettings.intensity = value;
        postProcessingProfile.vignette.settings = vignetteSettings;
    }
    public override void GetHit(Hitbox hitbox)
    {
        CameraFollow.screenShakeAmount += 10 * GetDamage(hitbox);
        FreezeFrame((GetDamage(hitbox)));
        UIManager.instance.SetHealthbar(hp, maxHp);
        StartCoroutine("ChangeColor", GetDamage(hitbox));
        //knockback
        Vector2 direction = transform.position - hitbox.transform.position;
        rocketController.AddForce(GetKnockback(hitbox) * direction);
    }
    private IEnumerator ChangeColor(int flashes)
    {
        for (int r = 0; r < flashes; r++)
        {
            hullRenderer.material = getHitMaterial;
            yield return null;
            hullRenderer.material = startMaterial;
            yield return null;
        }
    }
    private void FreezeFrame(int frames)
    {
        StopCoroutine("FreezeFrame");
        StartCoroutine("FrameFreeze", frames);
    }
    private IEnumerator FrameFreeze(int frames)
    {
        Time.timeScale = 0.001f;
        for (int r = 0; r < frames; r++)
        {
            yield return null;
        }
        Time.timeScale = 1;
    }
}
