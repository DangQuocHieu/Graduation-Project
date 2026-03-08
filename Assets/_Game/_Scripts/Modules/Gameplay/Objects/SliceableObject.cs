using UnityEngine;

public class SliceableObject : MonoBehaviour
{
    [SerializeField] private GameObject _piecePrefab;

    public void OnSlice()
    {
        Debug.Log("Slice: " + gameObject.name);
    }
}
