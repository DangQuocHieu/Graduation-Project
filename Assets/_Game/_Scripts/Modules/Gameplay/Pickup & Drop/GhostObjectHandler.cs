using UnityEngine;
using System.Collections.Generic;

public class GhostCollisionTracker : MonoBehaviour
{
    public GrabbableObject ignoredObject;
    public int overlapCount = 0;

    private int _interactableLayer = -1;

    private void Awake()
    {
        _interactableLayer = LayerMask.NameToLayer("Interactable Object");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_interactableLayer != -1 && other.gameObject.layer == _interactableLayer)
        {
            if (ignoredObject != null && other.transform.IsChildOf(ignoredObject.transform))
            {
                return;
            }
            overlapCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactableLayer != -1 && other.gameObject.layer == _interactableLayer)
        {
            if (ignoredObject != null && other.transform.IsChildOf(ignoredObject.transform))
            {
                return;
            }
            overlapCount--;
        }
    }
    
    private void OnDisable()
    {
        overlapCount = 0;
    }
}

public class GhostObjectHandler
{
    private GameObject _ghostObject;
    private GhostCollisionTracker _collisionTracker;
    private List<MeshRenderer> _ghostRenderers = new List<MeshRenderer>();
    
    private Material _validMaterial;
    private Material _invalidMaterial;
    private bool _isCurrentlyValid = true;
    private bool _wasActive = false;

    public bool IsPlacementValid
    {
        get
        {
            if (_ghostObject == null || !_ghostObject.activeInHierarchy) return false;
            return _collisionTracker != null && _collisionTracker.overlapCount == 0;
        }
    }

    public void CreateGhostObject(GrabbableObject original, Material validMaterial, Material invalidMaterial)
    {
        if (_ghostObject != null) DestroyGhostObject();

        _validMaterial = validMaterial;
        _invalidMaterial = invalidMaterial;
        _ghostRenderers.Clear();

        _ghostObject = new GameObject(original.gameObject.name + "_Ghost");
        _ghostObject.transform.localScale = original.transform.localScale;

        Rigidbody rb = _ghostObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        
        _collisionTracker = _ghostObject.AddComponent<GhostCollisionTracker>();
        _collisionTracker.ignoredObject = original;

        CopyVisualsAndColliders(original.transform, _ghostObject.transform);

        Collider[] colliders = _ghostObject.GetComponentsInChildren<Collider>(true);
        foreach (var col in colliders)
        {
            col.isTrigger = true;
        }
        
        _ghostObject.SetActive(false);
        _isCurrentlyValid = true;
        _wasActive = false;
    }

    private void CopyVisualsAndColliders(Transform source, Transform destination)
    {
        destination.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        var mf = source.GetComponent<MeshFilter>();
        if (mf != null)
        {
            var newMf = destination.gameObject.AddComponent<MeshFilter>();
            newMf.sharedMesh = mf.sharedMesh;
        }
        
        var mr = source.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            var newMr = destination.gameObject.AddComponent<MeshRenderer>();
            _ghostRenderers.Add(newMr);
            
            if (_validMaterial != null)
            {
                Material[] newMats = new Material[mr.sharedMaterials.Length];
                for (int i = 0; i < newMats.Length; i++)
                {
                    newMats[i] = _validMaterial;
                }
                newMr.sharedMaterials = newMats;
            }
            else
            {
                newMr.sharedMaterials = mr.sharedMaterials;
            }
        }

        var colliders = source.GetComponents<Collider>();
        foreach (var col in colliders)
        {
            if (col is BoxCollider box)
            {
                var b = destination.gameObject.AddComponent<BoxCollider>();
                b.center = box.center; b.size = box.size;
            }
            else if (col is SphereCollider sphere)
            {
                var s = destination.gameObject.AddComponent<SphereCollider>();
                s.center = sphere.center; s.radius = sphere.radius;
            }
            else if (col is CapsuleCollider capsule)
            {
                var c = destination.gameObject.AddComponent<CapsuleCollider>();
                c.center = capsule.center; c.radius = capsule.radius; c.height = capsule.height; c.direction = capsule.direction;
            }
            else if (col is MeshCollider meshCol)
            {
                var m = destination.gameObject.AddComponent<MeshCollider>();
                m.sharedMesh = meshCol.sharedMesh; m.convex = meshCol.convex;
            }
        }

        foreach (Transform child in source)
        {
            GameObject newChild = new GameObject(child.name);
            newChild.transform.SetParent(destination);
            newChild.transform.localPosition = child.localPosition;
            newChild.transform.localRotation = child.localRotation;
            newChild.transform.localScale = child.localScale;
            
            CopyVisualsAndColliders(child, newChild.transform);
        }
    }

    public void DestroyGhostObject()
    {
        if (_ghostObject != null)
        {
            Object.Destroy(_ghostObject);
            _ghostObject = null;
        }
    }

    public void UpdateGhostPosition(GrabbableObject objectInHand, Transform camera, float pickUpRange)
    {
        if (objectInHand == null)
        {
            if (_ghostObject != null)
            {
                DestroyGhostObject();
            }
            return;
        }

        if (_ghostObject == null) return;

        RaycastHit[] hits = Physics.RaycastAll(camera.position, camera.forward, pickUpRange);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        bool foundSurface = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == objectInHand.gameObject)
                continue;

            if (hit.collider.TryGetComponent<KitchenArea>(out var kitchenArea))
            {
                PlaceableSurface surface = kitchenArea.placeableSurface;
                Vector3 targetPosition = surface.SnapPoint == null ? hit.point : surface.SnapPoint.position;
            
                _ghostObject.transform.position = targetPosition;
                _ghostObject.transform.rotation = objectInHand.transform.rotation;

                if (!_ghostObject.activeSelf)
                {
                    _ghostObject.SetActive(true); 
                }
                foundSurface = true;
            }
            break;
        }

        if (!foundSurface && _ghostObject.activeSelf)
        {
            _ghostObject.SetActive(false);
            _wasActive = false;
        }

        if (_ghostObject.activeInHierarchy)
        {
            bool isValid = IsPlacementValid;
            if (isValid != _isCurrentlyValid || !_wasActive)
            {
                _isCurrentlyValid = isValid;
                _wasActive = true;
                UpdateMaterials();
            }
        }
    }

    private void UpdateMaterials()
    {
        Material matToApply = _isCurrentlyValid ? _validMaterial : _invalidMaterial;
        if (matToApply == null) return;

        foreach(var mr in _ghostRenderers)
        {
            if (mr != null)
            {
                Material[] mats = new Material[mr.sharedMaterials.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = matToApply;
                }
                mr.sharedMaterials = mats;
            }
        }
    }
}
