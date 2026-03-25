
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PoissonCollider : MonoBehaviour
{
    [Header("Poisson Disk")]
    [Min(0.0001f)]
    public float radius = 0.25f;

    [Min(1)]
    public int targetCount = 500;

    [Tooltip("How many candidate samples are tested in total. More = denser/better, but slower.")]
    [Min(1)]
    public int maxAttempts = 200000;

    [Tooltip("Inset sampled points along triangle normal.")]
    [Min(0f)]
    public float insetDistance = 0.02f;

    private List<Vector3> pointsWorld = new List<Vector3>();

    [Header("Sphere Collider Output")]
    public bool createSphereColliders = true;

    [Tooltip("Radius of the generated SphereColliders (independent from Poisson radius).")]
    public float colliderRadius = 0.05f;

    [Tooltip("Whether generated SphereColliders are triggers.")]
    public bool isTrigger = false;

    [Tooltip("Physics Material assigned to generated SphereColliders.")]
    public PhysicsMaterial colliderMaterial = null;

    // ------------------------------------------------------------

    [ContextMenu("Bake Poisson Points")]
    public void Bake()
    {
        pointsWorld.Clear();

        
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogWarning("MeshPoisson: No MeshFilter or Mesh assigned.");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] v = mesh.vertices;
        int[] t = mesh.triangles;

        if (t == null || t.Length < 3)
        {
            Debug.LogWarning("MeshPoisson: Mesh has no triangles.");
            return;
        }

        // Build area-weighted CDF for triangle picking
        float[] cdf = BuildTriangleAreaCDF(v, t, out float totalArea);
        if (totalArea <= 0f)
        {
            Debug.LogWarning("MeshPoisson: Total mesh surface area is zero.");
            return;
        }

        Transform tr = meshFilter.transform;
        float r2 = radius * radius;

        int attempts = 0;
        while (attempts < maxAttempts && pointsWorld.Count < targetCount)
        {
            attempts++;

            int triIndex = PickTriangleIndex(cdf, UnityEngine.Random.value);

            int i0 = t[triIndex * 3 + 0];
            int i1 = t[triIndex * 3 + 1];
            int i2 = t[triIndex * 3 + 2];

            Vector3 aL = v[i0];
            Vector3 bL = v[i1];
            Vector3 cL = v[i2];

            Vector3 pL = SamplePointInTriangle(aL, bL, cL);
            Vector3 pW = tr.TransformPoint(pL);


            //push inward along triangle normal
            if (insetDistance > 0f)
            {
                Vector3 nL = Vector3.Cross(bL - aL, cL - aL);
                if (nL.sqrMagnitude > 1e-12f)
                {
                    nL.Normalize();
                    Vector3 nW = tr.TransformDirection(nL).normalized;

                    // "Inward" = just go opposite to the triangle normal
                    pW -= nW * insetDistance;
                }
            }


            bool ok = true;
            for (int i = 0; i < pointsWorld.Count; i++)
            {
                if ((pointsWorld[i] - pW).sqrMagnitude < r2)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
                pointsWorld.Add(pW);
        }

        if (createSphereColliders)
            CreateSphereColliders();

        Debug.Log($"Poisson precompute finished: {pointsWorld.Count} points, attempts={attempts}, radius={radius}");
    }

    // ------------------------------------------------------------
    // Collider creation and deletion
    // ------------------------------------------------------------

    private void CreateSphereColliders()
    {
        GameObject go = transform.gameObject;

        // Remove old SphereColliders
        var oldColliders = go.GetComponents<SphereCollider>();
        for (int i = 0; i < oldColliders.Length; i++)
            DestroyImmediate(oldColliders[i]);

        // Create new SphereColliders
        for (int i = 0; i < pointsWorld.Count; i++)
        {
            SphereCollider sc = go.AddComponent<SphereCollider>();

            // Convert world position to local space
            sc.center = transform.InverseTransformPoint(pointsWorld[i]);

            sc.radius = colliderRadius;
            sc.isTrigger = isTrigger;

            if(colliderMaterial != null)
                sc.material = colliderMaterial;
        }
    }

    [ContextMenu("Remove Poisson Sphere Colliders")]
    private void RemoveSphereColliders()
    {
        GameObject go = transform.gameObject;

        var colliders = go.GetComponents<SphereCollider>();
        for (int i = 0; i < colliders.Length; i++)
            DestroyImmediate(colliders[i]);

        Debug.Log($"Removed {colliders.Length} SphereColliders.");
    }


    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------

    /// <summary>
    /// Builds a cumulative distribution function (CDF) over triangle areas
    /// for area-weighted random triangle selection.
    /// </summary>
    private static float[] BuildTriangleAreaCDF(Vector3[] verts, int[] tris, out float totalArea)
    {
        int triCount = tris.Length / 3;
        float[] cdf = new float[triCount];

        totalArea = 0f;
        for (int k = 0; k < triCount; k++)
        {
            Vector3 a = verts[tris[k * 3 + 0]];
            Vector3 b = verts[tris[k * 3 + 1]];
            Vector3 c = verts[tris[k * 3 + 2]];

            float area = 0.5f * Vector3.Cross(b - a, c - a).magnitude;
            totalArea += area;
            cdf[k] = totalArea;
        }

        if (totalArea > 0f)
        {
            for (int k = 0; k < triCount; k++)
                cdf[k] /= totalArea;
        }

        return cdf;
    }

    /// <summary>
    /// Picks a triangle index using a normalized CDF and a random value in [0, 1].
    /// </summary>
    private static int PickTriangleIndex(float[] cdf, float u01)
    {
        int idx = Array.BinarySearch(cdf, u01);
        if (idx < 0)
            idx = ~idx;

        if (idx >= cdf.Length)
            idx = cdf.Length - 1;

        return idx;
    }

    /// <summary>
    /// Uniformly samples a random point inside a triangle using barycentric coordinates.
    /// </summary>
    private static Vector3 SamplePointInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        float u = UnityEngine.Random.value;
        float v = UnityEngine.Random.value;

        if (u + v > 1f)
        {
            u = 1f - u;
            v = 1f - v;
        }

        return a + u * (b - a) + v * (c - a);
    }
}
