using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapGeneratorGPU))]
public class ProceduralVegetation : MonoBehaviour, ISavableData
{
    [SerializeField]private Texture2D tex;
    [SerializeField] private List<GameObject> customAreaObjects;
    [SerializeField] private List<Vegetation> vegetations = new();
    private List<Vector3> availablePositions = new();

    #region References
    private MapGeneratorGPU _mapGenerator;
    private Terrain terrain;
    private TerrainData terrainData;
    #endregion
    [SerializeField] private List<GameObject> spawnedObjects = new();

    private void Start()
    {
        Init();
    }

    void Init() {
        _mapGenerator = GetComponent<MapGeneratorGPU>();
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        tex = _mapGenerator.GetHeightmapTexture();

        if (customAreaObjects.Count > 0) PopulateCustomAreaObjects(customAreaObjects);

        PopulateTreeObjects();
    }

    public void PopulateCustomAreaObjects(List<GameObject> customObjectsPrefab)
    {
        List<Vector3> pos = _mapGenerator.GetCustomAreaPosition();

        int numPositions = Mathf.Min(customObjectsPrefab.Count, pos.Count);

        pos = ShufflePositions(pos);

        for (int i = 0; i < numPositions; i++)
        {
            Vector3 worldPosition = ConvertTerrainToWorldPosition(pos[i]);
            GameObject go = Instantiate(customObjectsPrefab[i], worldPosition, Quaternion.identity, this.transform);
            Vector3 adjustedPosition = go.transform.position;
            adjustedPosition.y += terrain.GetPosition().y;
            go.transform.position = adjustedPosition;
            
            go.transform.localScale = new Vector3(go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);

            spawnedObjects.Add(go);
        }
    }

    private List<Vector3> ShufflePositions(List<Vector3> positions)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            int randIndex = Random.Range(i, positions.Count);
            Vector3 temp = positions[i];
            positions[i] = positions[randIndex];
            positions[randIndex] = temp;
        }
        return positions;
    }

    public void PopulateTreeObjects()
    {
        GetAvailableRegions();
        List<Vector3> placedVegetation = new();

        foreach (var vegetation in vegetations)
        {
            int spawnObjects = 0;
            for (int y = 0; y < terrainData.size.z; y += vegetation.spacing)
            {
                for (int x = 0; x < terrainData.size.x; x += vegetation.spacing)
                {
                    if (spawnObjects >= vegetation.numberOfObjectsToSpawn) continue;
                    //normalized to world height. minHeight = 0.2 and terrainData.size.y = 100 is 20%
                    //returns heights in world space
                    float minHeight = vegetation.minHeight * terrainData.size.y;
                    float maxHeight = vegetation.maxHeight * terrainData.size.y;

                    float radius = vegetation.radius;

                    Vector3 position = GetRandomPosition();

                    Vector3 vegetationWorldPosition = ConvertTerrainToWorldPosition(position);

                    float currentPositionHeight = vegetationWorldPosition.y;

                    if (currentPositionHeight < minHeight || currentPositionHeight > maxHeight)
                        continue;

                    bool canPlace = true;

                    foreach (var oldVegetation in placedVegetation)
                    {
                        float dist = (oldVegetation - vegetationWorldPosition).magnitude;
                        if (dist < vegetation.radius)
                        {
                            canPlace = false;
                            break;
                        }
                    }

                    if (canPlace)
                    {
                        GameObject go = Instantiate(vegetation.prefab, vegetationWorldPosition, Quaternion.identity, this.gameObject.transform);
                        go.name = vegetation.prefab.name;
                        Vector3 adjustedPosition = go.transform.position;
                        adjustedPosition.y += terrain.GetPosition().y;
                        go.transform.position = adjustedPosition;

                        go.transform.localScale = new Vector3(go.transform.localScale.x, vegetation.objectHeight, go.transform.localScale.z);

                        placedVegetation.Add(go.transform.position);
                        spawnObjects++;
                        spawnedObjects.Add(go);

                        if (go.TryGetComponent<Interactable>(out Interactable interactable))
                        {
                            interactable.parentIsland = this;
                            interactable.isSpawnedByIsland = true;
                        }
                    }
                }
            }
        }
    }

    public void RemoveObjects(GameObject go)
    {
        if (spawnedObjects.Contains(go)) { 
            Destroy(go);
            spawnedObjects.Remove(go);
            Debug.Log($"Removed object {go.name} from island {go.name}.");
        }
    }

    private Vector3 GetRandomPosition()
    {
        int randIndex = Random.Range(0, availablePositions.Count);
        Vector3 position = availablePositions[randIndex];

        availablePositions.RemoveAt(randIndex);
        return position;
    }

    void GetAvailableRegions()
    {
        int width = tex.width;
        int height = tex.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currectPixel = tex.GetPixel(x, y, 0).r;

                availablePositions.Add(new Vector3(x, 0, y));
            }
        }
    }

    Vector3 ConvertTerrainToWorldPosition(Vector3 position)
    {
        float worldX = (float)position.x + terrain.transform.position.x;  // + terrain.transform.position.x if there are islands with x pos > 0

        float worldZ = (float)position.z + terrain.transform.position.z;  // + terrain.transform.position.z if there are islands with z pos > 0

        float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)); // + + terrain.transform.position.z if there are islands with y > 0

        Vector3 newVegetationPosition = new Vector3(worldX, worldY, worldZ);

        return newVegetationPosition;
    }

    public List<GameObject> GetSpawnedObjects() { 
        return spawnedObjects;
    }

    public void AddToSpawnedObjects(GameObject go)
    {
        spawnedObjects.Add(go);
    }

    public string ToJson()
    {
        List<VegetationData> data = new List<VegetationData>();

        foreach (var go in spawnedObjects)
        {
            VegetationData vegetationData = new VegetationData(go.name, go.transform.position, go.transform.localScale);

            data.Add(vegetationData);
        }

        VegetationDataWrapper wrapper = new VegetationDataWrapper { data = data };
        return JsonUtility.ToJson(wrapper, true);
    }

    public void FromJson(string json)
    {
        VegetationDataWrapper vegetationData = JsonUtility.FromJson<VegetationDataWrapper>(json);

        foreach (var obj in spawnedObjects)
        {
            Destroy(obj);
        }
        spawnedObjects.Clear();

        foreach (var item in vegetationData.data)
        {
            GameObject prefab = Resources.Load<GameObject>($"IslandObjects/{item.prefabName}");

            if (prefab != null)
            {
                if (prefab.TryGetComponent<Interactable>(out Interactable interactable)) {
                    interactable.parentIsland = this;
                }
                Vector3 pos = new Vector3(item.posX, item.posY, item.posZ);
                Vector3 objScale = new Vector3(item.scaleX, item.scaleY, item.scaleZ);

                GameObject go = Instantiate(prefab, pos, Quaternion.identity, transform);
                go.name = item.prefabName;
                go.transform.localScale = objScale;
                spawnedObjects.Add(go);
            }
            else
            {
                Debug.LogError($"Prefab {item.prefabName} not found in Resources/IslandObjects/");
            }
        }
    }

    [System.Serializable]
    public struct Vegetation
    {
        public int numberOfObjectsToSpawn;
        public GameObject prefab;
        public float minHeight;
        public float maxHeight;
        public float radius;
        public int spacing;
        public float objectHeight;
    }
}
