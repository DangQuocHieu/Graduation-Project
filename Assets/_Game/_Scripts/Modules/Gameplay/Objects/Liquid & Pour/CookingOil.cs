using System.Collections.Generic;
using DQHieu.Framework.Audio;
using UnityEngine;

public class CookingOil : MonoBehaviour
{
    public FryingPan attachedFryingPan;

    public bool isHot = false;
    public MeshCollider meshCollider;

    public AudioData fryingSfx;
    private AudioEmitter fryingSfxEmitter;

    void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
    }

    private List<CookableObject> objectsInOil = new List<CookableObject>();

    void Update()
    {
        isHot = attachedFryingPan != null && attachedFryingPan.isHot;

        objectsInOil.RemoveAll(item => item == null);

        bool shouldPlaySound = isHot && objectsInOil.Count > 0;

        if (shouldPlaySound && fryingSfxEmitter == null)
        {
            fryingSfxEmitter = AudioManager.Instance.PlaySFX(fryingSfx, transform.position, fadeInDuration: 2f);
        }
        else if (!shouldPlaySound && fryingSfxEmitter != null)
        {
            AudioManager.Instance.StopSFX(fryingSfxEmitter, fadeDuration: 2f);
            fryingSfxEmitter = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CookableObject>(out var cookableObj))
        {
            if (!objectsInOil.Contains(cookableObj))
            {
                objectsInOil.Add(cookableObj);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CookableObject>(out var cookableObj))
        {
            objectsInOil.Remove(cookableObj);
        }
    }

}
