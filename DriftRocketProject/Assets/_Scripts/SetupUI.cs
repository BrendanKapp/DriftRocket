using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupUI : MonoBehaviour
{
    [SerializeField]
    private bool customSprite = false;

    private void Start()
    {
        SetupTextAndFont();
    }
    private void SetupTextAndFont()
    {
        Text text = GetComponent<Text>();
        if (text == null) text = GetComponentInChildren<Text>();
        if (text != null)
        {
            text.color = UIManager.instance.textColor;
            text.font = UIManager.instance.mainFont;
        }
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.color = UIManager.instance.buttonColor;
            if (!customSprite) image.sprite = UIManager.instance.buttonSprite;
        }
    }
}
