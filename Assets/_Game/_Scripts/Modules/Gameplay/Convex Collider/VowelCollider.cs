using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class VoxelCollider : MonoBehaviour
{
    [Header("Grid / Voxel (World Units)")]
    [Min(0.0001f)] public float spacingWorld = 0.1f;     // desired voxel size in world units
    [Min(0f)] public float boundsPaddingWorld = 0.0f;    // expand bounds a bit (world units)

    [Header("Box Colliders")]
    [Tooltip("1 = exactly voxel size, <1 shrink, >1 expand.")]
    [Min(0.0001f)] public float boxSizeMultiplier = 1.0f;
    public bool isTrigger = false;
    [Tooltip("Physics Material assigned to generated SphereColliders.")]
    public PhysicsMaterial colliderMaterial = null;

    [Header("Merge")]
    public bool mergeBoxes = true;

    [Header("Safety")]
    [Tooltip("Hard cap so you don't accidentally create millions of colliders.")]
    [Min(1)] public int maxColliders = 20000;

    [Header("Inside Test (Triangle Ray Test)")]
    [Range(1, 3)] public int rayDirections = 3;


    // ------------------------------------------------------------

    [ContextMenu("Bake Voxel Box Colliders (Merged)")]
    public void Bake()
    {
        var mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("VoxelCollider: No MeshFilter / Mesh found on this GameObject.");
            return;
        }

        RemoveVoxelColliders();

        Mesh mesh = mf.sharedMesh;

        // --- Build a LOCAL grid (axis-aligned with the object's local axes)
        // Convert world spacing/padding into local spacing/padding per axis.
        Vector3 ls = transform.lossyScale;
        float sx = Mathf.Max(1e-8f, Mathf.Abs(ls.x));
        float sy = Mathf.Max(1e-8f, Mathf.Abs(ls.y));
        float sz = Mathf.Max(1e-8f, Mathf.Abs(ls.z));

        Vector3 spacingLocal = new Vector3(spacingWorld / sx, spacingWorld / sy, spacingWorld / sz);
        Vector3 paddingLocal = new Vector3(boundsPaddingWorld / sx, boundsPaddingWorld / sy, boundsPaddingWorld / sz);

        Bounds b = mesh.bounds;
        b.Expand(paddingLocal * 2f);

        Vector3 minL = b.min;
        Vector3 sizeL = b.size;

        // number of CELLS (voxel volumes)
        int nx = Mathf.Max(1, Mathf.CeilToInt(sizeL.x / spacingLocal.x));
        int ny = Mathf.Max(1, Mathf.CeilToInt(sizeL.y / spacingLocal.y));
        int nz = Mathf.Max(1, Mathf.CeilToInt(sizeL.z / spacingLocal.z));

        // mesh data local
        Vector3[] v = mesh.vertices;
        int[] t = mesh.triangles;

        bool[,,] filled = new bool[nx, ny, nz];

        int tested = 0;
        int inside = 0;

        // Sample CELL CENTERS: min + (i + 0.5) * spacing
        for (int x = 0; x < nx; x++)
        {
            float px = minL.x + (x + 0.5f) * spacingLocal.x;
            for (int y = 0; y < ny; y++)
            {
                float py = minL.y + (y + 0.5f) * spacingLocal.y;
                for (int z = 0; z < nz; z++)
                {
                    float pz = minL.z + (z + 0.5f) * spacingLocal.z;

                    tested++;
                    Vector3 pLocal = new Vector3(px, py, pz);

                    if (IsPointInsideMeshByTrianglesLocal(v, t, pLocal, rayDirections))
                    {
                        filled[x, y, z] = true;
                        inside++;
                    }
                }
            }
        }

        int created = 0;

        if (!mergeBoxes)
        {
            // One box per filled cell
            Vector3 cellSizeLocal = Vector3.Scale(spacingLocal, Vector3.one * boxSizeMultiplier);
            for (int x = 0; x < nx; x++)
                for (int y = 0; y < ny; y++)
                    for (int z = 0; z < nz; z++)
                    {
                        if (!filled[x, y, z]) continue;

                        var bc = gameObject.AddComponent<BoxCollider>();
                        bc.isTrigger = isTrigger;

                        if (colliderMaterial != null)
                            bc.material = colliderMaterial;

                        bc.center = new Vector3(
                            minL.x + (x + 0.5f) * spacingLocal.x,
                            minL.y + (y + 0.5f) * spacingLocal.y,
                            minL.z + (z + 0.5f) * spacingLocal.z
                        );

                        bc.size = cellSizeLocal;

                        created++;
                        if (created >= maxColliders)
                        {
                            Debug.LogWarning($"VoxelCollider: Reached maxColliders={maxColliders}. Stopping early.");
                            break;
                        }
                    }
        }
        else
        {
            created = CreateMergedBoxCollidersLocal(filled, nx, ny, nz, minL, spacingLocal);
        }


        Debug.Log($"VoxelCollider baked (LOCAL): tested={tested}, insideVoxels={inside}, createdColliders={created}, " +
                  $"spacingWorld={spacingWorld}, spacingLocal={spacingLocal}, merged={mergeBoxes}");

    }

    [ContextMenu("Remove Voxel Box Colliders")]
    public void RemoveVoxelColliders()
    {
        var colliders = GetComponents<BoxCollider>();
        for (int i = 0; i < colliders.Length; i++)
            DestroyImmediate(colliders[i]);
    }

    // ------------------------------------------------------------
    // Greedy merge in LOCAL grid
    // ------------------------------------------------------------
    private int CreateMergedBoxCollidersLocal(bool[,,] filled, int nx, int ny, int nz, Vector3 minL, Vector3 spacingLocal)
    {
        bool[,,] used = new bool[nx, ny, nz];
        int created = 0;

        for (int x = 0; x < nx; x++)
            for (int y = 0; y < ny; y++)
                for (int z = 0; z < nz; z++)
                {
                    if (!filled[x, y, z] || used[x, y, z])
                        continue;

                    // Expand X
                    int dx = 1;
                    while (x + dx < nx && filled[x + dx, y, z] && !used[x + dx, y, z])
                        dx++;

                    // Expand Y for full X-span
                    int dy = 1;
                    while (y + dy < ny && RowAllFreeAndFilledX(filled, used, x, y + dy, z, dx))
                        dy++;

                    // Expand Z for full X*Y area
                    int dz = 1;
                    while (z + dz < nz && SlabAllFreeAndFilledXY(filled, used, x, y, z + dz, dx, dy))
                        dz++;

                    // mark used
                    for (int ix = x; ix < x + dx; ix++)
                        for (int iy = y; iy < y + dy; iy++)
                            for (int iz = z; iz < z + dz; iz++)
                                used[ix, iy, iz] = true;

                    // Local box size in local units
                    Vector3 sizeLocal = new Vector3(
                        dx * spacingLocal.x,
                        dy * spacingLocal.y,
                        dz * spacingLocal.z
                    ) * boxSizeMultiplier;

                    // Local center: block min corner + half size (UNSCALED by multiplier!)
                    // We keep center consistent with the cell grid; multiplier only affects size.
                    Vector3 centerLocal = new Vector3(
                        minL.x + (x + dx * 0.5f) * spacingLocal.x,
                        minL.y + (y + dy * 0.5f) * spacingLocal.y,
                        minL.z + (z + dz * 0.5f) * spacingLocal.z
                    );

                    var bc = gameObject.AddComponent<BoxCollider>();
                    bc.isTrigger = isTrigger;
                    bc.center = centerLocal;
                    bc.size = sizeLocal;

                    if (colliderMaterial != null)
                        bc.material = colliderMaterial;

                    created++;
                    if (created >= maxColliders)
                    {
                        Debug.LogWarning($"VoxelCollider: Reached maxColliders={maxColliders} while merging. Stopping early.");
                        return created;
                    }
                }

        return created;
    }

    private static bool RowAllFreeAndFilledX(bool[,,] filled, bool[,,] used, int x0, int y, int z, int dx)
    {
        for (int x = x0; x < x0 + dx; x++)
            if (!filled[x, y, z] || used[x, y, z])
                return false;
        return true;
    }

    private static bool SlabAllFreeAndFilledXY(bool[,,] filled, bool[,,] used, int x0, int y0, int z, int dx, int dy)
    {
        for (int y = y0; y < y0 + dy; y++)
            for (int x = x0; x < x0 + dx; x++)
                if (!filled[x, y, z] || used[x, y, z])
                    return false;
        return true;
    }

    // ------------------------------------------------------------
    // Point-in-mesh via triangle ray intersection (odd/even rule) - LOCAL
    // (no TransformPoint per triangle!)
    // ------------------------------------------------------------
    private static bool IsPointInsideMeshByTrianglesLocal(Vector3[] vertsLocal, int[] triangles, Vector3 pointLocal, int directionsToVote)
    {
        if (directionsToVote <= 1)
            return IsInsideOddEvenLocal(vertsLocal, triangles, pointLocal, Vector3.right);

        int votes = 0;
        if (IsInsideOddEvenLocal(vertsLocal, triangles, pointLocal, Vector3.right)) votes++;
        if (IsInsideOddEvenLocal(vertsLocal, triangles, pointLocal, Vector3.up)) votes++;
        if (IsInsideOddEvenLocal(vertsLocal, triangles, pointLocal, Vector3.forward)) votes++;

        return votes >= 2;
    }

    private static bool IsInsideOddEvenLocal(Vector3[] vertsLocal, int[] triangles, Vector3 rayOriginLocal, Vector3 rayDirLocal)
    {
        int hits = 0;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertsLocal[triangles[i]];
            Vector3 v1 = vertsLocal[triangles[i + 1]];
            Vector3 v2 = vertsLocal[triangles[i + 2]];

            if (RayIntersectsTriangle(rayOriginLocal, rayDirLocal, v0, v1, v2))
                hits++;
        }

        return (hits & 1) == 1;
    }

    // M�ller�Trumbore ray-triangle intersection
    private static bool RayIntersectsTriangle(Vector3 rayOrigin, Vector3 rayDir, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        const float EPSILON = 1e-8f;

        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;

        Vector3 h = Vector3.Cross(rayDir, edge2);
        float a = Vector3.Dot(edge1, h);

        if (a > -EPSILON && a < EPSILON)
            return false;

        float f = 1.0f / a;
        Vector3 s = rayOrigin - v0;
        float u = f * Vector3.Dot(s, h);

        if (u < 0.0f || u > 1.0f)
            return false;

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(rayDir, q);

        if (v < 0.0f || (u + v) > 1.0f)
            return false;

        float t = f * Vector3.Dot(edge2, q);

        return t > EPSILON;
    }
}