using DQHieu.Framework;
using DQHieu.Framework.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

public class HomeManager : Singleton<HomeManager>
{
    [Title("UI")]
    public SettingScreen settingScreen;
    [Title("Sound")]
    public AudioData homeScreenBGM;
    void Start()
    {
        DataManager.Instance.LoadData();
        AudioManager.Instance.PlayBGM(homeScreenBGM, fadeDuration: 1f);
    }
}
