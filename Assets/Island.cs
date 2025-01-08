using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    public Transform respawnPoint;

    public void MarkAsLastIsland()
    {
        GameManager.Instance.SetLastIsland(this); 
    }
}
