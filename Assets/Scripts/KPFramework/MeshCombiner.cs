using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    private void Awake()
    {
        // Dictionary to hold materials and their corresponding CombineInstances
        Dictionary<Material, List<CombineInstance>> materialToCombineInstances = new();

        foreach (Transform child in transform)
        {
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

            if (meshFilter != null && meshRenderer != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Material[] materials = meshRenderer.sharedMaterials;

                // Iterate through each submesh (in case the mesh has multiple materials)
                for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
                {
                    Material material = materials[subMeshIndex];

                    CombineInstance combineInstance = new CombineInstance
                    {
                        mesh = mesh,
                        subMeshIndex = subMeshIndex,
                        transform = meshFilter.transform.localToWorldMatrix
                    };

                    if (!materialToCombineInstances.ContainsKey(material))
                    {
                        materialToCombineInstances[material] = new List<CombineInstance>();
                    }

                    materialToCombineInstances[material].Add(combineInstance);
                }

                child.gameObject.SetActive(false);
            }
        }

        // Lists to hold the final meshes and materials
        List<CombineInstance> finalCombineInstances = new();
        List<Material> finalMaterials = new();

        foreach (var kvp in materialToCombineInstances)
        {
            Material material = kvp.Key;
            List<CombineInstance> combineInstances = kvp.Value;

            Mesh mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.CombineMeshes(combineInstances.ToArray(), true, true); // Merge submeshes here

            CombineInstance finalCombineInstance = new CombineInstance
            {
                mesh = mesh,
                subMeshIndex = 0,
                transform = Matrix4x4.identity
            };

            finalCombineInstances.Add(finalCombineInstance);
            finalMaterials.Add(material);
        }

        // Combine all meshes into one, with each material as a submesh
        Mesh finalMesh = new Mesh();
        finalMesh.indexFormat = IndexFormat.UInt32;
        finalMesh.CombineMeshes(finalCombineInstances.ToArray(), false); // Do not merge submeshes

        // Assign the combined mesh and materials
        MeshFilter meshFilterCombined = GetComponent<MeshFilter>();
        MeshRenderer meshRendererCombined = GetComponent<MeshRenderer>();

        meshFilterCombined.sharedMesh = finalMesh;
        meshRendererCombined.sharedMaterials = finalMaterials.ToArray();

        gameObject.SetActive(true);
    }
}