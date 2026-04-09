using System.Collections;
using UnityEngine;
[System.Serializable]
public class IngredientAnchor : MonoBehaviour
{
    public bool isEmpty = true;
    public Ingredient attachedIngredient;

    public void OnInteract(PickupAndDropHandler pickupAndDropHandler)
    {
        StartCoroutine(InteractWithPickupAndDropHandlerCoroutine(pickupAndDropHandler));
    }

    private IEnumerator InteractWithPickupAndDropHandlerCoroutine(PickupAndDropHandler pickupAndDropHandler)
    {
        attachedIngredient.gameObject.SetActive(true);
        attachedIngredient.rb.MovePosition(transform.position);
        attachedIngredient.rb.MoveRotation(transform.rotation);

        yield return new WaitForFixedUpdate();
        pickupAndDropHandler.PickupObject(attachedIngredient);
        attachedIngredient = null;
        gameObject.SetActive(false);
    }


}
