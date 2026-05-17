
using System.Collections.Generic;
using DQHieu.Framework;
using Sirenix.OdinInspector;
using UnityEngine;



public class GameLoopManager : MonoBehaviour
{
    [Title("Runtime Tracking")]
    public float timer = 0f;

    [Title("Level Data & Customer Spawn")]
    public LevelData[] levelDatas;
    public LevelData currentLevel;
    public int customerSpawned = 0;
    public Transform[] customerSpawnPoint;

    [Title("References")]
    public OrderManager orderManager;

    public void Initialize(OrderManager orderManager, DataManager dataManager)
    {
        currentLevel = levelDatas[dataManager.playerData.CurrentLevelIndex];
        this.orderManager = orderManager;
    }

    void Update()
    {
        UpdateServingState();

    }
    private void UpdateServingState()
    {
        timer += Time.deltaTime;
        if(customerSpawned < currentLevel.customerArrivalConfigs.Count && timer > currentLevel.customerArrivalConfigs[customerSpawned].spawnTime)
        {
        
            var currentConfig = currentLevel.customerArrivalConfigs[customerSpawned];
            Vector3 spawnPos = customerSpawnPoint[UnityEngine.Random.Range(0, customerSpawnPoint.Length)].position;
            Customer spawnedCustomer = Instantiate(currentConfig.customerPrefab, spawnPos, Quaternion.identity, null);
            spawnedCustomer.customerOrder = currentConfig.customerOrder;
            spawnedCustomer.orderManager = orderManager;
            ++customerSpawned;
        }

    }

}
