using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum ControlType { pc, andriod};

public enum MobileControlType { buttons = 0, accelerometer = 1, joystick = 2};

public class UIManager : MonoBehaviour
{
    public bool isAndroidBuild = false;
    public static ControlType controlType;
    [Header("Mobile Controls")]
    [SerializeField]
    private GameObject mobileControls;
    [SerializeField]
    private Slider mobileControlSlider;
    [SerializeField]
    private bool testAndriodControls = false;
    private MobileControlType mobileControlType;
    public MobileControlType getMobileControlType
    {
        get
        {
            return mobileControlType;
        }
    }
    [SerializeField]
    private List<GameObject> screens;
    /**
     * 0 is the gameplay menu
     * 1 is the credits screen
     * 2 is the start menu
     * 3 is the pause menu
     * 4 is the options menu
     * 5 is the credits menu
     * 6 is the game over screen
     * **/
    [SerializeField]
    private int[] screenToButton; //how many clickable buttons are there on each UI screen, used for joystick navigation
    [SerializeField]
    private RectTransform playButton;
    [SerializeField]
    private RectTransform selectedText;
    private Animation selectedTextAnimation;
    [SerializeField]
    private RectTransform selectedTextGameOver; //due to UI structure, a second one is needed for this screen
    private Animation selectedTextGameOverAnimation;
    private bool isUsingJoystickForUI = false;
    private bool joystickAxisReset = true;
    private int currentButton = 0;
    private int lastScreen = 0; //could also be called currentScreen, as it holds what's currently active and is used for disableing it when moving to the next screen
    [Header("Other Objects")]
    [SerializeField]
    private RectTransform healthbar;
    private Image healthbarImage;
    [SerializeField]
    private GameObject mainMenuActive;
    [SerializeField]
    private CanvasGroup fadeToBlackCanvasGroup;
    [SerializeField]
    private GameObject soundHolder;
    [Header("Text Objects")]
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text highScoreText;
    [SerializeField]
    private Text mainMenuHighScoreText;
    [SerializeField]
    private Text screenSizeText;
    [SerializeField]
    private Text timerText;
    [SerializeField]
    private Text soundText;
    [SerializeField]
    private Text usingAccelerometerText;
    [Header("UI Ship Settings")]
    [SerializeField]
    private GameObject playerShip;
    [SerializeField]
    private float playerShipSpeed = 2;
    [SerializeField]
    private Transform cameraOrgin;
    private bool IsGameOverScreenActive
    {
        get
        {
            return !playerShip.activeSelf;
        }
    }
    [Header("Color Settings")]
    [SerializeField]
    private Material[] primaryColors;
    [SerializeField]
    private Material[] secondaryColors;

    public static UIManager instance;

    public static bool soundActive = true;

    public delegate void SoundAction();
    public static SoundAction SoundEvent;

    private SaveFloat currentScreenSize;

    [Header("UI Look Settings")]
    public Font mainFont;
    public Sprite buttonSprite;
    public Color textColor;
    public Color buttonColor;

    private void Awake()
    {
        instance = this;
        healthbarImage = healthbar.GetComponent<Image>();
        selectedTextAnimation = selectedText.GetComponent<Animation>();
        selectedTextGameOverAnimation = selectedTextGameOver.GetComponent<Animation>();
        currentScreenSize = new SaveFloat("iCurrentScreenSize");
    }
    private void Start()
    {
        CameraFollow.instance.SetTarget(cameraOrgin);
        StartCoroutine("IntroSequence");
        UpdateScreenSize();
        SetupControls(false);
        SetAccelerometer();
    }
    private void Update()
    {
        if (!IsGameOverScreenActive) //if the UI is active
        {
            playerShip.transform.Rotate(Vector3.up * playerShipSpeed * Time.deltaTime);
            JoystickUI();
        }
        if (lastScreen == 6) JoystickGameOverUI();
        if (IsGameOverScreenActive) timerText.text = ((int)(Time.time - GameController.instance.GetGameStartTime())).ToString();
    }
    private void JoystickGameOverUI ()
    {
        if (!Input.GetButtonDown("JoystickClick")) return;
        selectedTextGameOver.gameObject.SetActive(false);
        ReturnToMainMenu();
    }
    private void JoystickUI ()
    {
        if (Input.GetButtonDown("JoystickClick"))
        {
            //click
            CursorCast(CursorCastType.click);
        }
        if (Input.GetAxisRaw("Vertical") < 0 && joystickAxisReset)
        {
            //down
            CursorCast(CursorCastType.hoverOff);
            CycleCurrentButton(1); //hover on in here
            joystickAxisReset = false;
        } else if (Input.GetAxisRaw("Vertical") > 0 && joystickAxisReset)
        {
            //up
            CursorCast(CursorCastType.hoverOff);
            CycleCurrentButton(-1); //hover on in here
            joystickAxisReset = false;
        } else if (Input.GetAxisRaw("Vertical") == 0)
        {
            joystickAxisReset = true;
        }
    }
    public void SelectedText (int location)
    {
        currentButton = location;
        CycleCurrentButton(0);
    }
    private enum CursorCastType {click, hoverOn, hoverOff};
    private void CursorCast(CursorCastType type) //simulates a click
    {
        PointerEventData cursor = new PointerEventData(EventSystem.current); //creates a fake cursor
        cursor.position = selectedText.position; //sets its position to the mouses position
        List<RaycastResult> objectsHit = new List<RaycastResult>(); //creates a list of hit results
        EventSystem.current.RaycastAll(cursor, objectsHit); //raycasts from the cursor and fills [objectsHit] with the hits
        foreach (RaycastResult i in objectsHit) //goes through all the hits
        {
            Button clickedButton = i.gameObject.GetComponent<Button>();
            if (clickedButton == null) continue;
            switch (type)
            {
                case CursorCastType.click:
                    clickedButton.OnSubmit(cursor);
                    break;
                case CursorCastType.hoverOn:
                    clickedButton.OnSelect(cursor);
                    break;
                case CursorCastType.hoverOff:
                    clickedButton.OnDeselect(cursor);
                    break;
            }
        }
    }
    private float GetCurrentButtonPosition()
    {
        return (currentButton * -55);
    }
    private void SetSelectedTextPosition(float position)
    {
        selectedText.localPosition = new Vector3(0, position, 0);
    }
    private void SetSelectedTextPosition()
    {
        SetSelectedTextPosition(currentButton * -200 - 100);
    }
    private void CycleCurrentButton(int direction)
    {
        currentButton += direction;
        if (currentButton >= screenToButton[lastScreen])
        {
            currentButton -= direction;
        } else if (currentButton < 0)
        {
            currentButton = 0;
        }
        StopCoroutine("LerpSelectedTextPosition");
        StartCoroutine(LerpSelectedTextPosition(currentButton * -200 - 100));
    }
    private IEnumerator LerpSelectedTextPosition(float target)
    {
        float lerpTarget = selectedText.localPosition.y;
        for (int i = 0; i < 25; i++)
        {
            lerpTarget = Mathf.Lerp(selectedText.localPosition.y, target, 0.1f);
            SetSelectedTextPosition(lerpTarget);
            yield return null;
        }
        SetSelectedTextPosition(target);
        CursorCast(CursorCastType.hoverOn);
    }
    private IEnumerator IntroSequence()
    {
        SetScreen(1);
        yield return new WaitForSeconds(2);
        SetHighScoreText(mainMenuHighScoreText);
        mainMenuActive.SetActive(true);
        SetScreen(2);
    }
    private void SetupControls (bool activate)
    {
        if (testAndriodControls || isAndroidBuild)
        {
            mobileControls.SetActive(activate && mobileControlType == MobileControlType.buttons);
            controlType = ControlType.andriod;
            return;
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            mobileControls.SetActive(activate && mobileControlType == MobileControlType.buttons);
            controlType = ControlType.andriod;
        } else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            controlType = ControlType.pc;
        } else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            controlType = ControlType.pc;
        } else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            controlType = ControlType.pc;
        }
    }
    public void MobileControlsUp ()
    {
        mobileControlSlider.value = 0;
    }
    public void MobileControlsInput()
    {
        GameController.playerController.SetMobileInput(mobileControlSlider.value);
    }
    public void MobileControlsInput (float input)
    {
        GameController.playerController.SetMobileInput(input);
    }
    public void MobileControlsBrake (bool isBraking)
    {
        GameController.playerController.SetMobileBraking(isBraking);
    }
    public void SetScreen(int target)
    {
        PlayMouseClick();
        screens[lastScreen].SetActive(false);
        screens[target].SetActive(true);
        lastScreen = target;
        //joystick UI
        currentButton = 0;
        SetSelectedTextPosition();
        StartCoroutine(CursorCastRoutine());
    }
    private IEnumerator CursorCastRoutine ()
    {
        yield return null;
        yield return null;
        CursorCast(CursorCastType.hoverOn);
    }
    public void SetPlayerColor (int style)
    {
        PlayerController.instance.SetColors(primaryColors[style], secondaryColors[style]);
    }
    public void ReturnToMainMenu () //used when going from the stats panel
    {
        playerShip.SetActive(true);
        playerShip.transform.position = new Vector3(-5, 15, 0);
        CameraFollow.instance.SetTarget(cameraOrgin);
        ObjectPooler.DePoolAllPools();
        Time.timeScale = 1;
        SetHighScoreText(mainMenuHighScoreText);
        mainMenuActive.SetActive(true);
        SetScreen(2);
    }
    public void SetAccelerometer ()
    {
        PlayMouseClick();
        if (controlType == ControlType.pc)
        {
            usingAccelerometerText.text = "Keys/Joystick";
            return;
        }
        mobileControlType++;
        if ((int)mobileControlType > 2)
        {
            mobileControlType = 0;
        }
        switch (mobileControlType)
        {
            case MobileControlType.buttons:
                usingAccelerometerText.text = "On Screen Buttons";
                break;
            case MobileControlType.accelerometer:
                usingAccelerometerText.text = "Accelerometer";
                break;
            case MobileControlType.joystick:
                usingAccelerometerText.text = "Joystick";
                break;
        }
    }
    public void SetHighScoreText ()
    {
        SetHighScoreText(highScoreText);
    }
    private void SetHighScoreText (Text text)
    {
        text.text = "High Score: " + FloatWithTwoDecimals(GameController.instance.GetHighScore());
    }
    public void Play()
    {
        mainMenuActive.SetActive(false);
        playerShip.GetComponent<Animation>().Play();
        CameraFollow.screenShakeAmount += 50;
        CameraFollow.instance.SetActive(true);
        Sound.PlaySound("BlastOff");
        Sound.PlaySound("RocketSoundShort");
        Invoke("StartGame", 1);
    }
    private void StartGame ()
    {
        playerShip.SetActive(false);
        SetHealthbar(1, 1);
        SetScreen(0);
        SetupControls(true);
        GameController.instance.GameStart();
    }
    public void SetSound ()
    {
        PlayMouseClick();
        if (soundActive)
        {
            soundActive = false;
            soundHolder.SetActive(false);
            soundText.text = "Sound Off";
        } else
        {
            soundActive = true;
            soundHolder.SetActive(true);
            soundText.text = "Sound On";
        }
        SoundEvent?.Invoke();
    }
    private void PlayMouseClick()
    {
        AudioSource audioSource = ObjectPooler.PoolObject("Sounds", "MouseClick").GetComponent<AudioSource>();
        if (IsGameOverScreenActive)
        {
            selectedTextGameOverAnimation.Play();
        } else
        {
            selectedTextAnimation.Play();
        }
        audioSource.Play();
    }
    public void SetScreenSize ()
    {
        PlayMouseClick();
        currentScreenSize.value++;
        currentScreenSize.value = currentScreenSize.value == 3 ? 0 : currentScreenSize.value;
        UpdateScreenSize();
    }
    private void UpdateScreenSize ()
    {
        if (controlType == ControlType.andriod)
        {
            switch (currentScreenSize.value)
            {
                case 0:
                    Screen.SetResolution(1080, 1920, FullScreenMode.ExclusiveFullScreen);
                    screenSizeText.text = "1080p";
                    break;
                case 1:
                    Screen.SetResolution(720, 1280, FullScreenMode.ExclusiveFullScreen);
                    screenSizeText.text = "720p";
                    break;
                case 2:
                    Screen.SetResolution(480, 858, FullScreenMode.ExclusiveFullScreen);
                    screenSizeText.text = "480p";
                    break;
            }
        } else if (controlType == ControlType.pc)
        {
            switch (currentScreenSize.value)
            {
                case 0:
                    Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
                    screenSizeText.text = "1080p";
                    break;
                case 1:
                    Screen.SetResolution(1280, 720, FullScreenMode.ExclusiveFullScreen);
                    screenSizeText.text = "720p";
                    break;
                case 2:
                    Screen.SetResolution(858, 480, FullScreenMode.ExclusiveFullScreen);
                    screenSizeText.text = "480p";
                    break;
            }
        }
    }
    public void NewHighScore (float newHighScore) //for when there is a new high score
    {
        
    }
    private float FloatWithTwoDecimals(float value)
    {
        return ((int)(value * 100) / 100f);
    }
    public void SetHealthbar (int hp, int maxHp)
    {
        targetAmount = (float)hp / maxHp;
        StopCoroutine("LerpHealhbar");
        StartCoroutine("LerpHealthbar");
    }
    private float initialAmount = 1;
    private float targetAmount = 1;
    private void UpdateHealthbar(float amount)
    {
        Vector3 hpScale = new Vector3(amount, 1, 1);
        Vector3 hpPosition = new Vector3(350 * (amount - 1), 900, 0);
        healthbar.localScale = hpScale;
        healthbar.localPosition = hpPosition;
    }
    private void IncreaseMaxHp()
    {
        StartCoroutine("IncreaseMaxHpRoutine");
    }
    private IEnumerator IncreaseMaxHpRoutine(int maxHp)
    {
        yield return null;
    }
    private IEnumerator LerpHealthbar ()
    {
        healthbarImage.color = Color.red;
        for (int i = 0; i < 25; i++)
        {
            initialAmount = Mathf.Lerp(initialAmount, targetAmount, 0.1f);
            UpdateHealthbar(initialAmount);
            yield return null;
        }
        UpdateHealthbar(targetAmount);
        healthbarImage.color = buttonColor;
    }
    public void FadeToBlack()
    {
        StartCoroutine(FadeToBlackRoutine());
    }
    private IEnumerator FadeToBlackRoutine()
    {
        for (int i = 0; i < 20; i++)
        {
            fadeToBlackCanvasGroup.alpha = i / 20;
            yield return null;
        }
        for (int i = 0; i < 20; i++)
        {
            fadeToBlackCanvasGroup.alpha = 1 - (i / 20);
            yield return null;
        }
        fadeToBlackCanvasGroup.alpha = 0;
    }
    public void GameOver (float score)
    {
        scoreText.text = "Score: " + FloatWithTwoDecimals(score);
        SetupControls(false);
        SetScreen(6);
        selectedTextGameOver.gameObject.SetActive(true);
    }
    public void Quit()
    {
        #if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
