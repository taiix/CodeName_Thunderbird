using UnityEngine;

[CreateAssetMenu(fileName = "New Reaching Quest", menuName = "Quest/Reaching Quest")]
public class DestinationQuest : BaseSO_Properties
{
    public string playerIdentifier;
    public string destinationIdentifier;

    public float distanceDifference;


    private PlayerHealth playerPos;
    private AirplaneController destinationPos;

    public override void Init() {
        playerPos = FindAnyObjectByType<PlayerHealth>();
        destinationPos = FindAnyObjectByType<AirplaneController>();
    }

    public override void CheckProgress()
    {
        Debug.Log("breeeeeeeeeeeeeeeeeeeeeee");
        float dist = (destinationPos.transform.position - playerPos.transform.position).magnitude;
        if (dist <= distanceDifference)
        {
            this.isCompleted = true;
        }
    }
}
