using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningPoint : MonoBehaviour
{
    private float shrinkTime;
    [SerializeField] private MiningMiniGame miniGame;

    private bool isClicked = false;
    public void Initialize(float initialSize, float shrinkTime, MiningMiniGame miniGame)
    {
        transform.localScale = Vector3.one * initialSize;
        this.shrinkTime = shrinkTime;
        this.miniGame = miniGame;

        StartCoroutine(ShrinkOverTime());
    }

    private IEnumerator ShrinkOverTime()
    {
        float elapsedTime = 0;
        Vector3 initialScale = transform.localScale;

        while (elapsedTime < shrinkTime)
        {
            float t = elapsedTime / shrinkTime;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // If the point fully disappears, consider it as a missed point.
        miniGame.PointMissed(this);
        Destroy(gameObject);
    }
    private void OnMouseDown()
    {
        if (isClicked)
            return;

        // Use raycasting to ensure only the top-most object is clicked
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the object hit by the raycast is this MiningPoint
            if (hit.collider.gameObject == gameObject)
            {
                // Mark the point as clicked to prevent multiple clicks
                isClicked = true;

                // Disable collider to prevent further clicks
                GetComponent<Collider>().enabled = false;

                // Notify the mini-game and destroy the point
                miniGame.PointClicked(this);
            }
        }
    }
}
