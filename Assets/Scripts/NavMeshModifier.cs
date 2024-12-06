using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshModifier : MonoBehaviour
{
    public float waterHeight = 8.19f;

    public float maxHeight = 20.0f;

    public Terrain terrain;

    public NavMeshSurface navMeshSurface;
    public GameObject go;
    void Start()
    {
        //go.SetActive(false);

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

        //CreateUnderwaterNavMeshModifier();
        //CreateAboveMaxHeightNavMeshModifier();
        RebuildNavMesh();
    }

    void CreateUnderwaterNavMeshModifier()
    {
        Vector3 terrainSize = terrain.terrainData.size;

        GameObject underwaterArea = new GameObject("UnderwaterNavMeshModifier");
        underwaterArea.transform.parent = this.transform;
        underwaterArea.transform.position = Vector3.zero;

        NavMeshModifierVolume modifierVolume = underwaterArea.AddComponent<NavMeshModifierVolume>();

        modifierVolume.size = new Vector3(terrainSize.x, Mathf.Abs(waterHeight), terrainSize.z);

        modifierVolume.center = new Vector3(terrain.transform.position.x + terrainSize.x / 2,
                                    waterHeight / 2,
                                    terrain.transform.position.z + terrainSize.z / 2);

        modifierVolume.area = NavMesh.GetAreaFromName("Not Walkable");
    }

    void CreateAboveMaxHeightNavMeshModifier()
    {
        Vector3 terrainSize = terrain.terrainData.size;

        GameObject aboveHeightArea = new GameObject("AboveMaxHeightNavMeshModifier");
        aboveHeightArea.transform.parent = this.transform;
        aboveHeightArea.transform.position = Vector3.zero;
        NavMeshModifierVolume modifierVolume = aboveHeightArea.AddComponent<NavMeshModifierVolume>();

        // Modifier for area above maxHeight
        modifierVolume.size = new Vector3(terrainSize.x, Mathf.Abs(terrainSize.y - maxHeight), terrainSize.z);
        modifierVolume.center = new Vector3(
            terrain.transform.position.x + terrainSize.x / 2,
            maxHeight + (terrainSize.y - maxHeight) / 2,
            terrain.transform.position.z + terrainSize.z / 2
        );

        modifierVolume.area = NavMesh.GetAreaFromName("Not Walkable");
    }

    void RebuildNavMesh()
    {
        // Force a NavMesh rebuild to take the new Modifier into account
        //navMeshSurface.RemoveData();
        navMeshSurface.BuildNavMesh();
        //go.SetActive(true);
    }
}