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

    void Update()
    {
        bool wasHot = isHot;
        isHot = attachedFryingPan != null && attachedFryingPan.isHot;

        if (wasHot && !isHot)
        {
            if (fryingSfxEmitter != null)
            {
                AudioManager.Instance.StopSFX(fryingSfxEmitter, fadeDuration: 2f);
                fryingSfxEmitter = null;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CookableObject>(out _) && isHot)
        {
            if (fryingSfxEmitter == null)
            {
                fryingSfxEmitter = AudioManager.Instance.PlaySFX(fryingSfx, transform.position, fadeInDuration: 2f);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CookableObject>(out _))
        {
            if (fryingSfxEmitter != null)
            {
                AudioManager.Instance.StopSFX(fryingSfxEmitter, fadeDuration: 2f);
                fryingSfxEmitter = null;
            }
        }
    }

}
