using UnityEngine;
using UnityEngine.Events;

public class CircleMinigameHandler : MonoBehaviour
{
    public static UnityAction<int, Item, Transform> OnMinigameInteracted;

    private void OnEnable() { OnMinigameInteracted += MinigameUpdate; }
    private void OnDisable() { OnMinigameInteracted -= MinigameUpdate; }

    private void MinigameUpdate(int count, Item item, Transform position)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(item.itemPrefab);
            go.transform.position = position.position + new Vector3(0,2,0);
        }
    }
}
