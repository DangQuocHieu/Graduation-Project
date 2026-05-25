using DQHieu.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScreen : MonoBehaviour
{
    public Button startButton;
    public Button settingButton;

    void OnEnable()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        settingButton.onClick.AddListener(OnSettingButtonClicked);
    }

    void OnDisable()
    {
        startButton.onClick.RemoveListener(OnStartButtonClicked);
        settingButton.onClick.RemoveListener(OnSettingButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        if (!DataManager.Instance.playerData.TutorialCompleted)
        {
            SceneTransitionManager.Instance.LoadScene("Tutorial Scene");
        }
        else
        {
            SceneTransitionManager.Instance.LoadScene("GameplayScene");
        }

    }

    private void OnSettingButtonClicked()
    {
        HomeManager.Instance.settingScreen.ShowScreen();
    }
}
