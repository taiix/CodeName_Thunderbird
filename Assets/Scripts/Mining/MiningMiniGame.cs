using System.Collections.Generic;
using UnityEngine;

public class MiningMiniGame : MonoBehaviour
{
    [SerializeField] private GameObject miningPointPrefab;
    [SerializeField] private int pointsToSpawn = 3; 
    [SerializeField] private float maxPointSize = 0.5f;
    [SerializeField] private float minPointSize = 0.1f;
    [SerializeField] private float pointShrinkTime = 5f; 
    private float pointSpawnDistance = 1f;

    private List<MiningPoint> miningPoints = new List<MiningPoint>();
    private OreInteractable currentOre;

    private int pointsClicked = 0;

    private bool isMining = false;

    private bool hasMined = false;
    private Transform playerTransform;

    //public void StartMining(OreInteractable ore)
    //{
    //    currentOre = ore;

    //    isMining = true;


    //    ClearPreviousPoints();
    //    SpawnMiningPoints(currentOre.transform);
    //}

    //private void SpawnMiningPoints(Transform oreTransform)
    //{
    //    playerTransform = Camera.main.transform;

    //    Vector3 directionToPlayer = (playerTransform.position - oreTransform.position).normalized;

    //    Vector3 spawnCenter = oreTransform.position + directionToPlayer * pointSpawnDistance;


    //    for (int i = 0; i < pointsToSpawn; i++)
    //    {
    //        Vector2 randomCirclePos = Random.insideUnitCircle * pointSpawnDistance;

    //        Vector3 pointPosition = spawnCenter + oreTransform.right * randomCirclePos.x + oreTransform.up * randomCirclePos.y;

        for (int i = 0; i < pointsToSpawn; i++)
        {
            Vector2 randomCirclePos = Random.insideUnitCircle * pointSpawnDistance;

            Vector3 pointPosition = spawnCenter + oreTransform.right * randomCirclePos.x + oreTransform.up * randomCirclePos.y;
            pointPosition.y += 1;
       
    //        GameObject miningPoint = Instantiate(miningPointPrefab, pointPosition, Quaternion.identity);

    //        miningPoint.GetComponent<MiningPoint>().Initialize(maxPointSize, pointShrinkTime, this);

    //        float randomScale = Random.Range(minPointSize, maxPointSize);
    //        miningPoint.transform.localScale = Vector3.one * randomScale;

    //        miningPoint.transform.LookAt(playerTransform.position);

    //        miningPoints.Add(miningPoint.GetComponent<MiningPoint>());
    //    }
    //}

    private void Update()
    {
        //if (hasMined) return;
        if (miningPoints.Count == 0 && isMining)
        {
            currentOre.BreakOre(pointsClicked);
            isMining = false;
            hasMined = true;
            pointsClicked = 0;
        }
    }

    public void PointClicked(MiningPoint point)
    {
        //Debug.Log("Point clicked");
        miningPoints.Remove(point);
        Destroy(point.gameObject);
        pointsClicked++;
  
    }

    public void PointMissed(MiningPoint point)
    {
        miningPoints.Remove(point);

        //Debug.Log("Point missed! Mining failed.");
    }

    //private void ClearPreviousPoints()
    //{
    //    foreach (MiningPoint point in miningPoints)
    //    {
    //        Destroy(point);
    //    }

    //    miningPoints.Clear();
    //}
}
