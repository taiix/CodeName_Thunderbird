using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.AI;

public class NavMeshModifier : MonoBehaviour
{  
    public float waterHeight = 8.19f;

    public Terrain terrain;

    public NavMeshSurface navMeshSurface;

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned!");
            return;
        }

        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface is not assigned!");
            return;
        }

        CreateUnderwaterNavMeshModifier();
        RebuildNavMesh();
    }

    void CreateUnderwaterNavMeshModifier()
    {
        Vector3 terrainSize = terrain.terrainData.size;

        GameObject underwaterArea = new GameObject("UnderwaterNavMeshModifier");
        underwaterArea.transform.parent = this.transform;

        NavMeshModifierVolume modifierVolume = underwaterArea.AddComponent<NavMeshModifierVolume>();

        modifierVolume.size = new Vector3(terrainSize.x, Mathf.Abs(waterHeight), terrainSize.z);

        modifierVolume.center = new Vector3(terrain.transform.position.x + terrainSize.x / 2,
                                    waterHeight / 2,
                                    terrain.transform.position.z + terrainSize.z / 2);

        modifierVolume.area = NavMesh.GetAreaFromName("Not Walkable");
    }

    void RebuildNavMesh()
    {
        // Force a NavMesh rebuild to take the new Modifier into account
        navMeshSurface.BuildNavMesh();
    }
}