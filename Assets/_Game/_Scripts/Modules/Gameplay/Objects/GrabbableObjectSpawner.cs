using UnityEngine;

public class GrabbableObjectSpawner : MonoBehaviour
{
    public GrabbableObject objectToSpawn;

    public void OnInteract(PickupAndDropHandler pickupAndDropHandler)
    {
        var spawnedObject = Instantiate(objectToSpawn, transform.position, Quaternion.identity, null);
        pickupAndDropHandler.PickupObject(spawnedObject);
    }
}
