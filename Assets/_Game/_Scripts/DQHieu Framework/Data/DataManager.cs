using UnityEngine;
// Thêm namespace của Odin Inspector
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace DQHieu.Framework
{
    public class DataManager : Singleton<DataManager>
    {
        public PlayerData playerData;
        public SettingData settingData;

        private const string PLAYER_DATA_KEY = "DQH_PlayerData";
        private const string SETTING_DATA_KEY = "DQH_SettingData";

        void OnEnable()
        {
            EventBus.Subcribe<LevelComplete>(HandleLevelCompleteEvent);
        }

        void OnDisable()
        {
            EventBus.UnSubcribe<LevelComplete>(HandleLevelCompleteEvent);
        }

        public void SavePlayerData()
        {
            string jsonData = JsonUtility.ToJson(playerData);
            PlayerPrefs.SetString(PLAYER_DATA_KEY, jsonData);
            PlayerPrefs.Save();
        }

        public void LoadPlayerData()
        {
            playerData = new PlayerData();
            if (PlayerPrefs.HasKey(PLAYER_DATA_KEY))
            {
                string jsonData = PlayerPrefs.GetString(PLAYER_DATA_KEY);
                JsonUtility.FromJsonOverwrite(jsonData, playerData);
            }
        }

        public void SaveSettingData()
        {
            string jsonData = JsonUtility.ToJson(settingData);
            PlayerPrefs.SetString(SETTING_DATA_KEY, jsonData);
            PlayerPrefs.Save();
        }

        public void LoadSettingData()
        {
            settingData = new SettingData();
            if (PlayerPrefs.HasKey(SETTING_DATA_KEY))
            {
                string jsonData = PlayerPrefs.GetString(SETTING_DATA_KEY);
                JsonUtility.FromJsonOverwrite(jsonData, settingData);
            }
        }

        public void LoadData()
        {
            LoadPlayerData();
            LoadSettingData();
            Debug.Log("All data loaded successfully!");
        }

        public void SaveData()
        {
            SavePlayerData();
            SaveSettingData();
            Debug.Log("All data saved successfully!");
        }

        // --- ĐOẠN THAY ĐỔI Ở ĐÂY ---
#if ODIN_INSPECTOR
        [Button("Reset All Data", ButtonSizes.Large)] // Tạo nút bấm kích thước lớn
        [GUIColor(1f, 0.3f, 0.3f)] // Đổi màu nút thành màu đỏ để cảnh báo nguy hiểm
        [PropertyOrder(10)] // Đẩy nút xuống dưới cùng của Inspector
#endif
        public void ResetData()
        {
            PlayerPrefs.DeleteKey(PLAYER_DATA_KEY);
            PlayerPrefs.DeleteKey(SETTING_DATA_KEY);
            LoadData();
            SaveData();
            Debug.Log("Data has been reset.");
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveData();
            }
        }

        void OnApplicationQuit()
        {
            SaveData();
        }

        private void HandleLevelCompleteEvent(LevelComplete evt)
        {
            if(!playerData.TutorialCompleted)
            {
                playerData.CompleteTutorial();
            }
            else if (evt.overallScore > 50)
            {
                ++playerData.CurrentLevelIndex;
            }
            SaveData();
        }
    }
}