using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool isHeld;
    public bool isThrown;

    public string interactionText = string.Empty;
    public ProceduralVegetation parentIsland;

    public virtual void Awake()
    {
        gameObject.layer = 7;
    }

    //Method that each interactable will overwrite and call when the player interacts 
    public abstract void OnInteract();

    //Method called when the raycast hits the interactable 
    public abstract void OnFocus();

    //Gets called when we are no longer looking at the interactable
    public abstract void OnLoseFocus();

    public void RemoveObject(GameObject go) {
        parentIsland?.RemoveObjects(go);
        //Destroy(go);
    }
}
