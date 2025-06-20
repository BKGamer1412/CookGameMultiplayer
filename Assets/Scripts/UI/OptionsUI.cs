using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] TextMeshProUGUI sfxVolumePercentage;
    [SerializeField] TextMeshProUGUI musicVolumePercentage;
    [SerializeField] Button returnButton;


    [SerializeField] Button moveUpButton;
    [SerializeField] Button moveDownButton;
    [SerializeField] Button moveLeftButton;
    [SerializeField] Button moveRightButton;
    [SerializeField] Button interactButton;
    [SerializeField] Button interactAltButton;
    [SerializeField] Button pauseButton;
    [SerializeField] Button gamepad_InteractButton;
    [SerializeField] Button gamepad_InteractAltButton;
    [SerializeField] Button gamepad_PauseButton;
    [SerializeField] TextMeshProUGUI moveUpText;
    [SerializeField] TextMeshProUGUI moveDownText;
    [SerializeField] TextMeshProUGUI moveLeftText;
    [SerializeField] TextMeshProUGUI moveRightText;
    [SerializeField] TextMeshProUGUI interactText;
    [SerializeField] TextMeshProUGUI interactAltText;
    [SerializeField] TextMeshProUGUI pauseText;
    [SerializeField] TextMeshProUGUI gamepad_InteractText;
    [SerializeField] TextMeshProUGUI gamepad_InteractAltText;
    [SerializeField] TextMeshProUGUI gamepad_PauseText;

    [SerializeField] Transform pressToRebindKeyTransform;

    private Action OnReturnButtonAction;

    private void Awake()
    {

        Instance = this;
        musicVolumeSlider.onValueChanged.AddListener(delegate { UpdateMusicVolume(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { UpdateSFXVolume(); });
        returnButton.onClick.AddListener(() =>
        {
            Hide();
            OnReturnButtonAction();
        });

        //key rebind
        moveUpButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Up));
        moveDownButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Down));
        moveLeftButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Left));
        moveRightButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Right));
        interactButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Interact));
        interactAltButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.InteractAlt));
        pauseButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Pause));
        gamepad_InteractButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.GamePad_Interact));
        gamepad_InteractAltButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.GamePad_InteractAlt));
        gamepad_PauseButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.GamePad_Pause));

    }
    private void Start()
    {
        sfxVolumeSlider.value = SoundManager.Instance.GetVolume();
        musicVolumeSlider.value = MusicManager.Instance.GetVolume();
        KitchenGameManager.Instance.OnGameUnPause += KitchenGameManager_OnGameUnPause;

        UpdateVisual();
        HidePressToRebindKey();
        Hide();
    }

    private void UpdateVisual()
    {
        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        gamepad_InteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePad_Interact);
        gamepad_InteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePad_InteractAlt);
        gamepad_PauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePad_Pause);
    }


    private void KitchenGameManager_OnGameUnPause(object sender, EventArgs e)
    {
        //hide when player press esc
        Hide();
    }

    private void UpdateMusicVolume()
    {
        MusicManager.Instance.ChangeVolume(musicVolumeSlider.value);
        musicVolumePercentage.text = Mathf.Ceil(MusicManager.Instance.GetVolume() / 1f * 100f) + "%";
    }

    private void UpdateSFXVolume()
    {
        SoundManager.Instance.ChangeVolume(sfxVolumeSlider.value);
        sfxVolumePercentage.text = Mathf.Ceil(SoundManager.Instance.GetVolume() / 1f * 100f) + "%";
    }

    public void Show(Action OnReturnButtonAction)
    {
        this.OnReturnButtonAction = OnReturnButtonAction;
        gameObject.SetActive(true);
        returnButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();

        //delegate included and lambda function
        GameInput.Instance.RebindBinding(binding, () =>
        {
            HidePressToRebindKey();
            UpdateVisual();
        });
            
    }
}
