
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class DecompositionCollider : MonoBehaviour
{
    [Header("Voxel Grid (World Units)")]
    [Min(0.0001f)] public float spacingWorld = 0.25f;
    [Min(0f)] public float boundsPaddingWorld = 0.0f;

    [Header("Generated MeshColliders")]
    public bool convex = false;
    public bool isTrigger = false;
    public PhysicsMaterial colliderMaterial = null;

    [Header("Output / Safety")]
    [Tooltip("Parent object under which generated colliders will be created.")]
    public string outputRootName = "VoxelMeshColliders";
    [Min(1)] public int maxColliders = 2000;

    [Tooltip("Skip groups with less than N triangles (reduces collider count).")]
    [Min(1)] public int minTrianglesPerVoxel = 4; // Minimum 4 triangles to form a closed surface otherwise physx may encounter issues!

    [Tooltip("Optional: if a voxel group would exceed this triangle count, it will still be created but you can use this to detect bad settings in logs.")]
    [Min(1)] public int warnIfTrianglesPerVoxelAbove = 5000;

    // ------------------------------------------------------------

    [ContextMenu("Bake Voxel MeshColliders")]
    public void Bake()
    {
        var mf = GetComponent<MeshFilter>();
        if (!mf || !mf.sharedMesh)
        {
            Debug.LogWarning("VoxelTriangleMeshColliders: Missing MeshFilter/sharedMesh.");
            return;
        }

        Mesh src = mf.sharedMesh;
        var verts = src.vertices;
        var tris = src.triangles;

        if (tris == null || tris.Length < 3)
        {
            Debug.LogWarning("VoxelTriangleMeshColliders: Mesh has no triangles.");
            return;
        }

        Clear();

        // Build LOCAL grid based on mesh.bounds (local space)
        Vector3 ls = transform.lossyScale;
        float sx = Mathf.Max(1e-8f, Mathf.Abs(ls.x));
        float sy = Mathf.Max(1e-8f, Mathf.Abs(ls.y));
        float sz = Mathf.Max(1e-8f, Mathf.Abs(ls.z));

        Vector3 spacingLocal = new Vector3(spacingWorld / sx, spacingWorld / sy, spacingWorld / sz);
        Vector3 paddingLocal = new Vector3(boundsPaddingWorld / sx, boundsPaddingWorld / sy, boundsPaddingWorld / sz);

        Bounds b = src.bounds;
        b.Expand(paddingLocal * 2f);

        Vector3 minL = b.min;
        Vector3 maxL = b.max;
        Vector3 sizeL = b.size;

        int nx = Mathf.Max(1, Mathf.CeilToInt(sizeL.x / spacingLocal.x));
        int ny = Mathf.Max(1, Mathf.CeilToInt(sizeL.y / spacingLocal.y));
        int nz = Mathf.Max(1, Mathf.CeilToInt(sizeL.z / spacingLocal.z));

        // Voxel buckets: voxel -> list of triangle indices (triangle = 3 ints in tris[])
        var buckets = new Dictionary<Vector3Int, List<int>>(4096);

        // Assign triangles to voxels (by triangle AABB overlap)
        int triCount = tris.Length / 3;
        for (int ti = 0; ti < triCount; ti++)
        {
            int i0 = tris[ti * 3 + 0];
            int i1 = tris[ti * 3 + 1];
            int i2 = tris[ti * 3 + 2];

            Vector3 p0 = verts[i0];
            Vector3 p1 = verts[i1];
            Vector3 p2 = verts[i2];

            Vector3 triMin = Vector3.Min(p0, Vector3.Min(p1, p2));
            Vector3 triMax = Vector3.Max(p0, Vector3.Max(p1, p2));

            // Clamp triangle AABB to grid bounds to avoid negative/out-of-range indices
            triMin = Vector3.Max(triMin, minL);
            triMax = Vector3.Min(triMax, maxL);

            // Convert AABB to voxel index range
            int x0 = Mathf.Clamp(WorldToCell(triMin.x, minL.x, spacingLocal.x), 0, nx - 1);
            int y0 = Mathf.Clamp(WorldToCell(triMin.y, minL.y, spacingLocal.y), 0, ny - 1);
            int z0 = Mathf.Clamp(WorldToCell(triMin.z, minL.z, spacingLocal.z), 0, nz - 1);

            int x1 = Mathf.Clamp(WorldToCell(triMax.x, minL.x, spacingLocal.x), 0, nx - 1);
            int y1 = Mathf.Clamp(WorldToCell(triMax.y, minL.y, spacingLocal.y), 0, ny - 1);
            int z1 = Mathf.Clamp(WorldToCell(triMax.z, minL.z, spacingLocal.z), 0, nz - 1);

            for (int x = x0; x <= x1; x++)
                for (int y = y0; y <= y1; y++)
                    for (int z = z0; z <= z1; z++)
                    {
                        var key = new Vector3Int(x, y, z);
                        if (!buckets.TryGetValue(key, out var list))
                        {
                            list = new List<int>(64);
                            buckets.Add(key, list);
                        }
                        list.Add(ti);
                    }
        }

        // Create output root
        Transform root = GetOrCreateOutputRoot();

        int created = 0;
        int skippedSmall = 0;

        foreach (var kvp in buckets)
        {
            List<int> triIndices = kvp.Value;
            if (triIndices == null) continue;

            int triInVoxel = triIndices.Count;
            if (triInVoxel < minTrianglesPerVoxel)
            {
                skippedSmall++;
                continue;
            }

            if (triInVoxel > warnIfTrianglesPerVoxelAbove)
                Debug.LogWarning($"VoxelTriangleMeshColliders: Voxel {kvp.Key} has {triInVoxel} triangles. Consider smaller spacingWorld or different mesh.");

            if (created >= maxColliders)
            {
                Debug.LogWarning($"VoxelTriangleMeshColliders: Reached maxColliders={maxColliders}. Stopping.");
                break;
            }

            // Build mesh for this voxel group (deduplicated vertices)
            Mesh sub = BuildSubMesh(src, verts, tris, triIndices);

            // Create GameObject with MeshCollider
            var go = new GameObject($"Voxel_{kvp.Key.x}_{kvp.Key.y}_{kvp.Key.z}");
            go.transform.SetParent(root, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            var mc = go.AddComponent<MeshCollider>();
            mc.sharedMesh = sub;
            mc.convex = convex;
            mc.isTrigger = isTrigger;
            mc.material = colliderMaterial;


            created++;
        }

        Debug.Log($"VoxelTriangleMeshColliders baked: triCount={triCount}, voxelsWithTris={buckets.Count}, created={created}, skippedSmall={skippedSmall}, spacingWorld={spacingWorld}");
    }

    [ContextMenu("Clear Voxel MeshColliders")]
    public void Clear()
    {
        Transform root = transform.Find(outputRootName);
        if (!root) return;


        // Destroy children immediately in editor
        for (int i = root.childCount - 1; i >= 0; i--)
            DestroyImmediate(root.GetChild(i).gameObject);
        DestroyImmediate(root.gameObject);

    }

    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------

    private Transform GetOrCreateOutputRoot()
    {
        Transform root = transform.Find(outputRootName);
        if (root) return root;

        var go = new GameObject(outputRootName);
        go.transform.SetParent(transform, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        return go.transform;
    }

    private static int WorldToCell(float p, float min, float cell)
    {
        // Convert position into voxel index
        return Mathf.FloorToInt((p - min) / cell);
    }

    private static Mesh BuildSubMesh(Mesh src, Vector3[] verts, int[] tris, List<int> triangleIndices)
    {
        // Map old vertex index -> new vertex index
        var map = new Dictionary<int, int>(triangleIndices.Count * 3);
        var newVerts = new List<Vector3>(triangleIndices.Count * 3);
        var newTris = new List<int>(triangleIndices.Count * 3);

        for (int k = 0; k < triangleIndices.Count; k++)
        {
            int ti = triangleIndices[k];
            int a = tris[ti * 3 + 0];
            int b = tris[ti * 3 + 1];
            int c = tris[ti * 3 + 2];

            newTris.Add(RemappedIndex(a, verts, map, newVerts));
            newTris.Add(RemappedIndex(b, verts, map, newVerts));
            newTris.Add(RemappedIndex(c, verts, map, newVerts));
        }

        var m = new Mesh();
        // If you expect large meshes per voxel:
        if (newVerts.Count > 65535)
            m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        m.SetVertices(newVerts);
        m.SetTriangles(newTris, 0, true);
        m.RecalculateBounds();

        // Normals not required for MeshCollider, but harmless if you want debug rendering later.
        // m.RecalculateNormals();

        return m;
    }

    private static int RemappedIndex(int oldIndex, Vector3[] verts, Dictionary<int, int> map, List<Vector3> newVerts)
    {
        if (map.TryGetValue(oldIndex, out int newIndex))
            return newIndex;

        newIndex = newVerts.Count;
        map.Add(oldIndex, newIndex);
        newVerts.Add(verts[oldIndex]);
        return newIndex;
    }
}
