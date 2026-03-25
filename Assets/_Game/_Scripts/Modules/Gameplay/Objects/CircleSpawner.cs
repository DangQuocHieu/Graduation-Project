using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject boxPrefab;    
    public int numberOfBoxes = 12;  
    public float radius = 5f;       

    void Start()
    {
        SpawnBoxesInCircle();
    }

    public void SpawnBoxesInCircle()
    {
        if (boxPrefab == null)
        {
            Debug.LogWarning("Bạn chưa gán Box Prefab kìa!");
            return;
        }

        for (int i = 0; i < numberOfBoxes; i++)
        {
            // 1. Tính toán góc
            float angle = i * Mathf.PI * 2f / numberOfBoxes;

            // 2. Tính toạ độ X và Z
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            Vector3 spawnPosition = transform.position + new Vector3(x, 0, z);

            // 3. Tính toán rotation 
            Vector3 direction = (spawnPosition - transform.position).normalized;
            
            // Góc xoay cơ bản (hướng thẳng ra ngoài)
            Quaternion baseRotation = Quaternion.LookRotation(direction);

            // MỚI: Cộng thêm 90 độ vào trục Y
            Quaternion finalRotation = baseRotation * Quaternion.Euler(0, 90f, 0);

            // 4. Instantiate box với góc xoay mới
            Instantiate(boxPrefab, spawnPosition, finalRotation, transform);
        }
    }
}