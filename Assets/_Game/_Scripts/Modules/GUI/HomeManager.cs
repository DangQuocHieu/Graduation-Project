using DQHieu.Framework;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    void Start()
    {
        DataManager.Instance.LoadData();
    }
}
