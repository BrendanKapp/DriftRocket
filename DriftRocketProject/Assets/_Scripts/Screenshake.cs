using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshake : MonoBehaviour
{
    private Vector3 initialPosition;
    private Transform gameCamera;

    private static int screenShakeAmount;
    public static float screenShakeMultiplier;

    private void Start()
    {
        initialPosition = transform.position;
        gameCamera = Camera.main.transform;
    }
    private void Update()
    {
        ShakeScreen();
    }
    private void ShakeScreen()
    {
        if (screenShakeAmount > 0)
        {
            gameCamera.position = initialPosition;
            gameCamera.position += Random.insideUnitSphere * screenShakeAmount * 0.01f;
            screenShakeAmount--;
        }
    }
    /**
     * shakes the screen by [amount]
     * **/
    public static void ScreenShake(int amount)
    {
        screenShakeAmount += (int)(amount * screenShakeMultiplier);
    }
}
