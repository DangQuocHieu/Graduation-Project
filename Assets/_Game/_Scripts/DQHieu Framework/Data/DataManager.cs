namespace DQHieu.Framework
{
    using UnityEngine;

    public class DataManager : Singleton<DataManager>
    {
        public PlayerData playerData;
        public SettingData settingData;


        private const string PLAYER_DATA_KEY = "DQH_PlayerData";
        private const string SETTING_DATA_KEY = "DQH_SettingData";

        
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
    }
}