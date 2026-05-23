using UnityEngine;

public enum GrabbableObjectType
{
    BambooTray, FryingPan, SauceBowl
}
public class GrabbableObjectSpawner : MonoBehaviour
{
    public GrabbableObjectType objectType;
    public GrabbableObject objectToSpawn;

    public void OnInteract(PickupAndDropHandler pickupAndDropHandler)
    {
        var spawnedObject = Instantiate(objectToSpawn, transform.position, Quaternion.identity, null);
        pickupAndDropHandler.PickupObject(spawnedObject);
    }
}
