using UnityEngine;
using UnityEngine.UI;
using DQHieu.Framework;
using DQHieu.Framework.Audio;
using DG.Tweening;

public class SettingScreen : MonoBehaviour
{
    public Button acceptButton;
    public RectTransform settingPanel;
    public RectTransform overlay;
    public ToggleButton musicVolumeToggleButton;
    public ToggleButton sfxVolumeToggleButton;

    void Start()
    {
        if (DataManager.Instance != null)
        {
            var settingData = DataManager.Instance.settingData;
            musicVolumeToggleButton.SetState(settingData.isMusicOn, true);
            sfxVolumeToggleButton.SetState(settingData.isSfxOn, true);
        }
    }

    void OnEnable()
    {
        acceptButton.onClick.AddListener(OnAcceptButtonClicked);
        musicVolumeToggleButton.onValueChanged += OnMusicToggled;
        sfxVolumeToggleButton.onValueChanged += OnSfxToggled;
    }

    void OnDisable()
    {
        acceptButton.onClick.RemoveListener(OnAcceptButtonClicked);
        musicVolumeToggleButton.onValueChanged -= OnMusicToggled;
        sfxVolumeToggleButton.onValueChanged -= OnSfxToggled;
    }

    private void OnMusicToggled(bool isOn)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.ToggleMusic(isOn);
        if (DataManager.Instance != null) DataManager.Instance.settingData.isMusicOn = isOn;
    }

    private void OnSfxToggled(bool isOn)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.ToggleSFX(isOn);
        if (DataManager.Instance != null) DataManager.Instance.settingData.isSfxOn = isOn;
    }

    private void OnAcceptButtonClicked()
    {
        if (DataManager.Instance != null) DataManager.Instance.SaveSettingData();
        HideScreen();
    }

    public void ShowScreen()
    {
        overlay.gameObject.SetActive(true);
        settingPanel.gameObject.SetActive(true);
        settingPanel.DOKill();
        settingPanel.DOScale(1f, 0.3f).From(0f).SetEase(Ease.OutBack).SetLink(gameObject);   
    }

    public void HideScreen()
    {
        overlay.gameObject.SetActive(false);
        settingPanel.gameObject.SetActive(false);
    }
}
