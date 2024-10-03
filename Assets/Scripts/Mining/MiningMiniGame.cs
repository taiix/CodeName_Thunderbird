using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningMiniGame : MonoBehaviour
{
    [SerializeField] private GameObject miningPointPrefab;
    private float pointSpawnDistance = 0.5f;
    [SerializeField] private int pointsToSpawn = 3; 
    [SerializeField] private float maxPointSize = 0.5f;
    [SerializeField] private float minPointSize = 0.1f;
    [SerializeField] private float pointShrinkTime = 5f; 

    private List<MiningPoint> miningPoints = new List<MiningPoint>();
    private OreInteractable currentOre;

    private int pointsClicked = 0;

    private bool isMining = false;
    private Transform playerTransform;

    private void Start()
    {

    }

    public void StartMining(OreInteractable ore)
    {
        currentOre = ore;

        isMining = true;


        ClearPreviousPoints();
        SpawnMiningPoints(currentOre.transform);
    }

    private void SpawnMiningPoints(Transform oreTransform)
    {
        playerTransform = Camera.main.transform;

        Vector3 directionToPlayer = (playerTransform.position - oreTransform.position).normalized;

        Vector3 spawnCenter = oreTransform.position + directionToPlayer * pointSpawnDistance;


        for (int i = 0; i < pointsToSpawn; i++)
        {
            // Create a random position within a circular area on the visible side of the ore
            Vector2 randomCirclePos = Random.insideUnitCircle * pointSpawnDistance;

            // Calculate the exact position of the mining point based on player's view
            Vector3 pointPosition = spawnCenter + oreTransform.right * randomCirclePos.x + oreTransform.up * randomCirclePos.y;

            // Create the mining point at the calculated position
            GameObject miningPoint = Instantiate(miningPointPrefab, pointPosition, Quaternion.identity);

            // Initialize the mining point with shrinking time and size parameters
            miningPoint.GetComponent<MiningPoint>().Initialize(maxPointSize, pointShrinkTime, this);

            // Adjust the sphere size and appearance
            float randomScale = Random.Range(minPointSize, maxPointSize);
            miningPoint.transform.localScale = Vector3.one * randomScale;

            // Make the sphere face the player by setting the forward direction to the player's view direction
            miningPoint.transform.LookAt(playerTransform.position);

            // Keep track of the spawned points
            miningPoints.Add(miningPoint.GetComponent<MiningPoint>());
        }
    }

    private Vector3 GetRandomPointAroundOre()
    {
        // Generate a random position around the current ore
        return currentOre.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
    }

    private void Update()
    {
        if (miningPoints.Count == 0 && isMining)
        {
            currentOre.BreakOre(pointsClicked);
            isMining = false;
            pointsClicked = 0;
        }
    }

    public void PointClicked(MiningPoint point)
    {
        Debug.Log("Point clicked");
        pointsClicked++;
        miningPoints.Remove(point);
        Destroy(point.gameObject);
  
    }

    public void PointMissed(MiningPoint point)
    {
        miningPoints.Remove(point);
        // Optional: Logic if the player misses a point
        Debug.Log("Point missed! Mining failed.");
    }

    private void ClearPreviousPoints()
    {
        foreach (MiningPoint point in miningPoints)
        {
            Destroy(point);
        }

        miningPoints.Clear();
    }
}
