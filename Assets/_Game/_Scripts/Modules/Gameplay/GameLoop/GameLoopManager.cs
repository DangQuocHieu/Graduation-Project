using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public enum GameplayState
{
    Preparation,
    Serving,
    Cleaning
}

public class GameLoopManager : MonoBehaviour
{
    [Title("Gameplay State")]
    public float timer = 0f;
    public float preparationDuration;
    public float cleaningDuration;
    public GameplayState currentState = GameplayState.Preparation;

    [Title("Level Data & Customer Spawn")]
    public LevelData[] levelDatas;
    public LevelData currentLevel;
    public int customerSpawned = 0;
    public Transform[] customerSpawnPoint;

    [Title("References")]
    public OrderManager orderManager;

    void Start()
    {
        EnterState(GameplayState.Preparation);
    }
    void Update()
    {
        UpdateState();
    }

    public void Initialize(OrderManager orderManager)
    {
        this.orderManager = orderManager;
    }

    private void ChangeState(GameplayState state)
    {
        ExitState(currentState);
        currentState = state;
        EnterState(currentState);
    }

    private void EnterState(GameplayState state)
    {
        
    }

    private void ExitState(GameplayState state)
    {
        
    }

    private void UpdateState()
    {
        switch(currentState)
        {
            case GameplayState.Preparation:
                UpdatePreparationState();
                break;
            case GameplayState.Serving:
                UpdateServingState();
                break;
            case GameplayState.Cleaning:
                UpdateCleaningState();
                break;
        }
    }
    
    private void EnterPreparationState()
    {

    }

    private void UpdatePreparationState()
    {
        timer += Time.deltaTime;
        if(timer >= preparationDuration)
        {
            ChangeState(GameplayState.Serving);
        }
    }

    private void ExitPreparationState()
    {

    }

    private void EnterServingState()
    {
        ResetTimer();
    }

    private void UpdateServingState()
    {
        timer += Time.deltaTime;
        if(customerSpawned < currentLevel.customerArrivalConfigs.Count && timer > currentLevel.customerArrivalConfigs[customerSpawned].spawnTime)
        {
            var currentConfig = currentLevel.customerArrivalConfigs[customerSpawned];
            Vector3 spawnPos = customerSpawnPoint[UnityEngine.Random.Range(0, customerSpawnPoint.Length)].position;
            Customer spawnedCustomer = Instantiate(currentConfig.customerPrefab, spawnPos, Quaternion.identity, null);
            spawnedCustomer.orderIngredients = currentConfig.order;
            spawnedCustomer.orderManager = orderManager;
            ++customerSpawned;
        }

    }

    private void ExitServingState()
    {
        
    }

    private void EnterCleaningState()
    {
        
    }

    private void UpdateCleaningState()
    {
       
    }

    private void ExitCleaningState()
    {
        
    }

    private void ResetTimer()
    {
        timer = 0f;
    }
}
