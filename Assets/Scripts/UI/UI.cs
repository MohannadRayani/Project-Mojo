using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Image fadeImageUI;
    [SerializeField] private GameObject[] uiElements;

    private UI_Settings settingsUI;
    private UI_MainMenu mainMenuUI;
    private IUIState mainMenuState;
    private IUIState inGameState;
    private IUIState hiddenState;
    private IUIState currentState;


    public UI_InGame inGameUI { get; private set; }
    public UI_Animator uiAnim { get; private set; }
    public UI_BuildButtonsHolder buildButtonsUI { get; private set; }

    [Header("UI SFX")]
    public AudioSource onHoverSfx;
    public AudioSource onClickSfx;

    private void Awake()
    {
        buildButtonsUI = GetComponentInChildren<UI_BuildButtonsHolder>(true);
        settingsUI = GetComponentInChildren<UI_Settings>(true);
        mainMenuUI = GetComponentInChildren<UI_MainMenu>(true);
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        uiAnim = GetComponent<UI_Animator>();
        mainMenuState = new ScreenState(this, mainMenuUI.gameObject);
        inGameState = new ScreenState(this, inGameUI.gameObject);
        hiddenState = new ScreenState(this, null);

        ActivateFadeEffect(true);

        SwitchTo(settingsUI.gameObject);
        SetState(mainMenuState);

        if(GameManager.instance.IsTestingLevel())
            SetState(inGameState);
    }


    public void SwitchTo(GameObject uiToEnable)
    {
        if (uiElements != null)
        {
            foreach (GameObject ui in uiElements)
            {
                if (ui == null) continue;
                ui.SetActive(false);
            }
        }

        if (uiToEnable != null)
        {
            uiToEnable.SetActive(true);

            // If opening the level selection UI, also ensure the camera moves to the level-select view
            if (uiToEnable.GetComponent<UI_LevelSelection>() != null)
            {
                CameraEffects cam = FindObjectOfType<CameraEffects>();
                if (cam != null)
                    cam.SwitchToLevelSelectView();
            }
        }
    }

    public void EnableMainMenuUI(bool enable)
    {
        if (enable)
            SetState(mainMenuState);
        else
            SetState(hiddenState);
    }

    public void EnableInGameUI(bool enable)
    {
        if(enable)
            SetState(inGameState);
        else
        {
            inGameUI.SnapTimerToDefaultPosition();
            SetState(hiddenState);
        }
    }

    public void QuitButton()
    {
        if (EditorApplication.isPlaying)
            EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }

    public void ActivateFadeEffect(bool fadeIn)
    {
        if (fadeImageUI.gameObject.activeSelf == false)
            return;

        if (fadeIn)
            uiAnim.ChangeColor(fadeImageUI, 0, 1.5f);
        else
            uiAnim.ChangeColor(fadeImageUI, 1, 1.5f);
    }

    private void SetState(IUIState newState)
    {
        currentState = newState;
        currentState.Enter();
    }

    private interface IUIState
    {
        void Enter();
    }

    private sealed class ScreenState : IUIState
    {
        private readonly UI ui;
        private readonly GameObject screenToShow;

        public ScreenState(UI ui, GameObject screenToShow)
        {
            this.ui = ui;
            this.screenToShow = screenToShow;
        }

        public void Enter()
        {
            ui.SwitchTo(screenToShow);
        }
    }
}
